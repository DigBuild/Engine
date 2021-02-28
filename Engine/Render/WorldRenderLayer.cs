using System;
using System.Numerics;
using DigBuildPlatformCS.Render;
using DigBuildPlatformCS.Resource;
using DigBuildPlatformCS.Util;

namespace DigBuildEngine.Render
{
    public sealed class WorldRenderLayer<TVertex> : IWorldRenderLayer where TVertex : unmanaged
    {
        public static WorldRenderLayer<TVertex> Create<TRes>(
            LinearTransformerFactory transformerFactory,
            RenderStageProvider renderStageProvider,
            RenderResourceFactory<TRes> renderResourceFactory,
            Func<TRes, RenderPipeline<TVertex>> pipelineGetter,
            Func<TRes, NativeBufferPool, IWorldRenderLayerUniforms> uniformFactory
        ) where TRes : notnull
            => new WorldRenderLayer<TVertex>(
            transformerFactory,
            renderStageProvider,
            (ctx, rm, stage) => renderResourceFactory(ctx, rm, stage),
            (res) => pipelineGetter((TRes)res),
            (res, pool) => uniformFactory((TRes)res, pool)
        );

        public delegate IVertexConsumer<TVertex> LinearTransformerFactory(IVertexConsumer<TVertex> next, Matrix4x4 transform);
        public delegate RenderStage RenderStageProvider(RenderContext context);
        public delegate TRes RenderResourceFactory<out TRes>(RenderContext context, ResourceManager resourceManager, RenderStage stage);
        
        private readonly LinearTransformerFactory _transformerFactory;
        private readonly RenderStageProvider _renderStageProvider;
        private readonly RenderResourceFactory<object> _renderResourceFactory;
        private readonly Func<object, RenderPipeline<TVertex>> _pipelineGetter;
        private readonly Func<object, NativeBufferPool, IWorldRenderLayerUniforms> _uniformFactory;

        private object? _renderResources;
        private RenderPipeline<TVertex>? _renderPipeline;

        internal RenderPipeline<TVertex> Pipeline => _renderPipeline!;

        private WorldRenderLayer(
            LinearTransformerFactory transformerFactory,
            RenderStageProvider renderStageProvider,
            RenderResourceFactory<object> renderResourceFactory,
            Func<object, RenderPipeline<TVertex>> pipelineGetter,
            Func<object, NativeBufferPool, IWorldRenderLayerUniforms> uniformFactory
        )
        {
            _transformerFactory = transformerFactory;
            _renderStageProvider = renderStageProvider;
            _renderResourceFactory = renderResourceFactory;
            _pipelineGetter = pipelineGetter;
            _uniformFactory = uniformFactory;
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

        public IWorldRenderLayerUniforms CreateUniforms(NativeBufferPool pool)
        {
            return _uniformFactory(_renderResources!, pool);
        }
    }
    
    public interface IWorldRenderLayer
    {
        void Initialize(RenderContext context, ResourceManager resourceManager);

        IWorldRenderLayerUniforms CreateUniforms(NativeBufferPool pool);
    }

    public interface IWorldRenderLayerUniforms : IDisposable
    {
        void Setup(RenderContext context, CommandBufferRecorder cmd);

        void PushAndUseTransform(RenderContext context, CommandBufferRecorder cmd, Matrix4x4 transform);

        void Finalize(RenderContext context, CommandBufferRecorder cmd);

        void Clear();
    }
}