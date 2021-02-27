using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DigBuildEngine.Render.GeneratedUniforms;
using DigBuildEngine.Voxel;
using DigBuildPlatformCS.Render;
using DigBuildPlatformCS.Resource;
using DigBuildPlatformCS.Util;

namespace DigBuildEngine.Render
{
    public readonly struct CrdVertex
    {
        public readonly Vector3 Pos;
        public readonly Vector3 Normal;
        public readonly Vector2 Uv;

        public CrdVertex(Vector3 pos, Vector3 normal, Vector2 uv)
        {
            Pos = pos;
            Normal = normal;
            Uv = uv;
        }
    }
    
    internal interface ICrdChunkUniform : IUniform<CrdChunkUniform>
    {
        public Matrix4x4 Matrix { get; set; }
    }
    
    internal interface ICrdProjectionUniform : IUniform<CrdProjectionUniform>
    {
        public Matrix4x4 Matrix { get; set; }
    }

    public class ChunkRenderData
    {
        private static PooledNativeBuffer<CrdVertex> _vertexNativeBuffer = null!;

        private static UniformBuffer<CrdChunkUniform> _uniformBuffer = null!;
        private static PooledNativeBuffer<CrdChunkUniform> _uniformNativeBuffer = null!;

        private static UniformBuffer<CrdProjectionUniform> _projectionUniformBuffer = null!;
        private static PooledNativeBuffer<CrdProjectionUniform> _projectionUniformNativeBuffer = null!;

        private static TextureBinding _overlayTextureBinding = null!;

        private static RenderPipeline<CrdVertex>? _pipeline;

        public static void Initialize(RenderContext context, RenderStage renderStage, ResourceManager resourceManager, NativeBufferPool bufferPool)
        {
            if (_pipeline != null)
                throw new Exception($"{nameof(ChunkRenderData)} has already been initialized.");
            
            _vertexNativeBuffer = bufferPool.Request<CrdVertex>();
            
            var vsResource = resourceManager.GetResource(new ResourceName("digbuild", "shaders/test.vert.spv"))!;
            var fsResource = resourceManager.GetResource(new ResourceName("digbuild", "shaders/test.frag.spv"))!;

            VertexShader vs = context.CreateVertexShader(vsResource)
                .WithUniform<CrdChunkUniform>(out var uniform)
                .WithUniform<CrdProjectionUniform>(out var projectionUniform);
            FragmentShader fs = context.CreateFragmentShader(fsResource)
                .WithSampler(out var textureHandle);
            
            _uniformBuffer = context.CreateUniformBuffer(uniform);
            _uniformNativeBuffer = bufferPool.Request<CrdChunkUniform>();

            _projectionUniformNativeBuffer = bufferPool.Request<CrdProjectionUniform>();
            _projectionUniformNativeBuffer.Add(new CrdProjectionUniform{ Matrix = Matrix4x4.Identity });
            _projectionUniformBuffer = context.CreateUniformBuffer(projectionUniform, _projectionUniformNativeBuffer);

            _pipeline = context.CreatePipeline<CrdVertex>(
                vs, fs, 
                renderStage,
                Topology.Triangles,
                depthTest: new DepthTest(true, CompareOperation.LessOrEqual, true)
            );
            
            IResource materialTexRes = resourceManager.GetResource(new ResourceName("digbuild", "textures/materials.png"))!;
            Texture materialTex = context.CreateTexture(new Bitmap(materialTexRes.OpenStream()));
            TextureSampler sampler = context.CreateTextureSampler(TextureFiltering.Linear, TextureFiltering.Nearest);
            _overlayTextureBinding = context.CreateTextureBinding(
                textureHandle,
                sampler,
                materialTex
            );
        }

        public static void BeginSubmit(CommandBufferRecorder cmd, float aspectRatio)
        {
            _projectionUniformNativeBuffer[0].Matrix =
                Matrix4x4.CreatePerspectiveFieldOfView(
                    MathF.PI * 90 / 180f, aspectRatio, 0.001f, 10000.0f
                ) *
                Matrix4x4.CreateScale(1, -1, 1);
            _projectionUniformBuffer.Write(_projectionUniformNativeBuffer);

            cmd.Using(_pipeline!, _projectionUniformBuffer, 0);
            cmd.Using(_pipeline!, _overlayTextureBinding);
        }

        public static void CompleteSubmit()
        {
            _uniformBuffer.Write(_uniformNativeBuffer);
        }

        private readonly uint _id;
        private readonly Chunk _chunk;
        private readonly VertexBuffer<CrdVertex> _vertexBuffer;
        private readonly VertexBufferWriter<CrdVertex> _vertexBufferWriter;
        
        private bool _draw;

        public ChunkRenderData(RenderContext renderContext, Chunk chunk)
        {
            _id = _uniformNativeBuffer.Count;
            _uniformNativeBuffer.Add(new CrdChunkUniform());

            _chunk = chunk;
            _vertexBuffer = renderContext.CreateVertexBuffer(out _vertexBufferWriter);
        }

        public void UpdateGeometry()
        {
            var buffer = _vertexNativeBuffer;
            buffer.Clear();

            var chunkOffset = _chunk.Position.GetOrigin();

            for (var x = 0; x < 16; x++)
            {
                for (var y = 0; y < 16; y++)
                {
                    for (var z = 0; z < 16; z++)
                    {
                        var b = _chunk.BlockStorage.Blocks[x, y, z];
                        if (b == null) continue;

                        BlockFaceFlags faces = BlockFaceFlags.None;
                        if (x == 0 || _chunk.BlockStorage.Blocks[x - 1, y, z] == null)
                            faces |= BlockFaceFlags.NegX;
                        if (x == 15 || _chunk.BlockStorage.Blocks[x + 1, y, z] == null)
                            faces |= BlockFaceFlags.PosX;
                        if (y == 0 || _chunk.BlockStorage.Blocks[x, y - 1, z] == null)
                            faces |= BlockFaceFlags.NegY;
                        if (y == 15 || _chunk.BlockStorage.Blocks[x, y + 1, z] == null)
                            faces |= BlockFaceFlags.PosY;
                        if (z == 0 || _chunk.BlockStorage.Blocks[x, y, z - 1] == null)
                            faces |= BlockFaceFlags.NegZ;
                        if (z == 15 || _chunk.BlockStorage.Blocks[x, y, z + 1] == null)
                            faces |= BlockFaceFlags.PosZ;

                        // var color = Color.FromArgb((int) b.Color);
                        buffer.Add(CreateCubeGeometry(
                            new Vector3(x, y, z),
                            b.Id,
                            faces
                        ).ToArray());
                    }
                }
            }

            // Console.WriteLine($"Updating chunk {_chunk.Position} with {buffer.Count} vertices.");

            _draw = buffer.Count > 0;
            if (_draw)
                _vertexBufferWriter.Write(buffer);
        }

        public void SubmitGeometry(ICamera camera, CommandBufferRecorder cmd, float partialTick)
        {
            if (!_draw)
                return;

            _uniformNativeBuffer[_id].Matrix =
                Matrix4x4.CreateTranslation(_chunk.Position.GetOrigin())
                * camera.GetInterpolatedTransform(partialTick);
            cmd.Using(_pipeline!, _uniformBuffer, _id);
            cmd.Draw(_pipeline!, _vertexBuffer);
        }

        private static Vector2 CalculateUv(uint id, uint corner)
        {
            const uint texMapSize = 2;

            var cu = corner % 2;
            var cv = corner / 2;

            var u = ((id % texMapSize) + cu) / (float) texMapSize;
            var v = ((id / texMapSize) + cv) / (float) texMapSize;

            return new Vector2(u, v);
        }

        private static IEnumerable<CrdVertex> CreateCubeGeometry(Vector3 offset, uint id, BlockFaceFlags faces)
        {
            var nx = Vector3.Zero;
            var ny = Vector3.Zero;
            var nz = Vector3.Zero;
            var px = Vector3.UnitX;
            var py = Vector3.UnitY;
            var pz = Vector3.UnitZ;
            
            if (faces.HasFlag(BlockFaceFlags.NegX))
            {
                // Negative X
                yield return new CrdVertex(offset + nx + ny + nz, -px, CalculateUv(id, 0));
                yield return new CrdVertex(offset + nx + py + nz, -px, CalculateUv(id, 1));
                yield return new CrdVertex(offset + nx + py + pz, -px, CalculateUv(id, 2));

                yield return new CrdVertex(offset + nx + ny + pz, -px, CalculateUv(id, 3));
                yield return new CrdVertex(offset + nx + ny + nz, -px, CalculateUv(id, 0));
                yield return new CrdVertex(offset + nx + py + pz, -px, CalculateUv(id, 2));
            }
            if (faces.HasFlag(BlockFaceFlags.PosX))
            {
                // Positive X
                yield return new CrdVertex(offset + px + ny + nz, px, CalculateUv(id, 0));
                yield return new CrdVertex(offset + px + py + pz, px, CalculateUv(id, 1));
                yield return new CrdVertex(offset + px + py + nz, px, CalculateUv(id, 2));

                yield return new CrdVertex(offset + px + py + pz, px, CalculateUv(id, 2));
                yield return new CrdVertex(offset + px + ny + nz, px, CalculateUv(id, 0));
                yield return new CrdVertex(offset + px + ny + pz, px, CalculateUv(id, 3));
            }

            if (faces.HasFlag(BlockFaceFlags.NegY))
            {
                // Negative Y
                yield return new CrdVertex(offset + nx + ny + nz, -py, CalculateUv(id, 0));
                yield return new CrdVertex(offset + px + ny + pz, -py, CalculateUv(id, 1));
                yield return new CrdVertex(offset + px + ny + nz, -py, CalculateUv(id, 2));

                yield return new CrdVertex(offset + px + ny + pz, -py, CalculateUv(id, 1));
                yield return new CrdVertex(offset + nx + ny + nz, -py, CalculateUv(id, 0));
                yield return new CrdVertex(offset + nx + ny + pz, -py, CalculateUv(id, 3));
            }

            if (faces.HasFlag(BlockFaceFlags.PosY))
            {
                // Positive Y
                yield return new CrdVertex(offset + nx + py + nz, py, CalculateUv(id, 0));
                yield return new CrdVertex(offset + px + py + nz, py, CalculateUv(id, 1));
                yield return new CrdVertex(offset + px + py + pz, py, CalculateUv(id, 2));

                yield return new CrdVertex(offset + px + py + pz, py, CalculateUv(id, 2));
                yield return new CrdVertex(offset + nx + py + pz, py, CalculateUv(id, 3));
                yield return new CrdVertex(offset + nx + py + nz, py, CalculateUv(id, 0));
            }
            
            if (faces.HasFlag(BlockFaceFlags.NegZ))
            {
                // Negative Z
                yield return new CrdVertex(offset + nx + ny + nz, -pz, CalculateUv(id, 0));
                yield return new CrdVertex(offset + px + ny + nz, -pz, CalculateUv(id, 1));
                yield return new CrdVertex(offset + px + py + nz, -pz, CalculateUv(id, 2));

                yield return new CrdVertex(offset + px + py + nz, -pz, CalculateUv(id, 2));
                yield return new CrdVertex(offset + nx + py + nz, -pz, CalculateUv(id, 3));
                yield return new CrdVertex(offset + nx + ny + nz, -pz, CalculateUv(id, 0));
            }

            if (faces.HasFlag(BlockFaceFlags.PosZ))
            {
                // Positive Z
                yield return new CrdVertex(offset + px + py + pz, pz, CalculateUv(id, 0));
                yield return new CrdVertex(offset + px + ny + pz, pz, CalculateUv(id, 1));
                yield return new CrdVertex(offset + nx + ny + pz, pz, CalculateUv(id, 2));

                yield return new CrdVertex(offset + nx + ny + pz, pz, CalculateUv(id, 2));
                yield return new CrdVertex(offset + nx + py + pz, pz, CalculateUv(id, 3));
                yield return new CrdVertex(offset + px + py + pz, pz, CalculateUv(id, 0));
            }
        }
    }

    [Flags]
    internal enum BlockFaceFlags
    {
        None = 0,
        NegX = 1 << 0,
        PosX = 1 << 1,
        NegY = 1 << 2,
        PosY = 1 << 3,
        NegZ = 1 << 4,
        PosZ = 1 << 5,
    }
}