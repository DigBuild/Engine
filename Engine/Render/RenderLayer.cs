using System;
using System.Numerics;
using DigBuild.Platform.Render;
using DigBuild.Platform.Resource;
using DigBuild.Platform.Util;

namespace DigBuild.Engine.Render
{
    public sealed class RenderLayer<TVertex> : IRenderLayer where TVertex : unmanaged
    {
        public static RenderLayer<TVertex> Create<TRes>(
            LinearTransformerFactory transformerFactory,
            RenderStageProvider renderStageProvider,
            RenderResourceFactory<TRes> renderResourceFactory,
            Func<TRes, RenderPipeline<TVertex>> pipelineGetter,
            Func<TRes, NativeBufferPool, IRenderLayerUniforms> uniformFactory,
            CommandInitializer<TRes> commandInitializer
        ) where TRes : notnull => new(
            transformerFactory,
            renderStageProvider,
            (ctx, rm, stage) => renderResourceFactory(ctx, rm, stage),
            (res) => pipelineGetter((TRes)res),
            (res, pool) => uniformFactory((TRes)res, pool),
            (res, cmd) => commandInitializer((TRes) res, cmd)
        );

        public delegate IVertexConsumer<TVertex> LinearTransformerFactory(IVertexConsumer<TVertex> next, Matrix4x4 transform);
        public delegate RenderStage RenderStageProvider(RenderContext context);
        public delegate TRes RenderResourceFactory<out TRes>(RenderContext context, ResourceManager resourceManager, RenderStage stage);
        public delegate void CommandInitializer<in TRes>(TRes res, CommandBufferRecorder cmd);
        
        private readonly LinearTransformerFactory _transformerFactory;
        private readonly RenderStageProvider _renderStageProvider;
        private readonly RenderResourceFactory<object> _renderResourceFactory;
        private readonly Func<object, RenderPipeline<TVertex>> _pipelineGetter;
        private readonly Func<object, NativeBufferPool, IRenderLayerUniforms> _uniformFactory;
        private readonly CommandInitializer<object> _commandInitializer;

        private object? _renderResources;
        private RenderPipeline<TVertex>? _renderPipeline;

        private RenderLayer(
            LinearTransformerFactory transformerFactory,
            RenderStageProvider renderStageProvider,
            RenderResourceFactory<object> renderResourceFactory,
            Func<object, RenderPipeline<TVertex>> pipelineGetter,
            Func<object, NativeBufferPool, IRenderLayerUniforms> uniformFactory,
            CommandInitializer<object> commandInitializer
        )
        {
            _transformerFactory = transformerFactory;
            _renderStageProvider = renderStageProvider;
            _renderResourceFactory = renderResourceFactory;
            _pipelineGetter = pipelineGetter;
            _uniformFactory = uniformFactory;
            _commandInitializer = commandInitializer;
        }

        public void Initialize(RenderContext context, ResourceManager resourceManager)
        {
            var stage = _renderStageProvider(context);
            _renderResources = _renderResourceFactory(context, resourceManager, stage);
            _renderPipeline = _pipelineGetter(_renderResources);
        }

        public IVertexConsumer<TVertex> LinearTransformer(IVertexConsumer<TVertex> next, Matrix4x4 transform)
        {
            return _transformerFactory(next, transform);
        }

        public IRenderLayerUniforms CreateUniforms(NativeBufferPool pool)
        {
            return _uniformFactory(_renderResources!, pool);
        }

        public void InitializeCommand(CommandBufferRecorder cmd)
        {
            _commandInitializer(_renderResources!, cmd);
        }

        public void Draw(CommandBufferRecorder cmd, VertexBuffer<TVertex> vertexBuffer)
        {
            cmd.Draw(_renderPipeline!, vertexBuffer);
        }
    }
    
    public interface IRenderLayer
    {
        void Initialize(RenderContext context, ResourceManager resourceManager);

        IRenderLayerUniforms CreateUniforms(NativeBufferPool pool);

        void InitializeCommand(CommandBufferRecorder cmd);
    }

    public interface IRenderLayerUniforms : IDisposable
    {
        void Setup(RenderContext context, CommandBufferRecorder cmd);

        void PushAndUseTransform(RenderContext context, CommandBufferRecorder cmd, Matrix4x4 transform);

        void Finalize(RenderContext context, CommandBufferRecorder cmd);

        void Clear();
    }
}