using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Impl.Worlds
{
    public sealed class ProgressiveChunkLoader
    {
        private readonly object _lockObject = new();
        private readonly Thread _generationThread;
        private bool _generationThreadActive = true;
        private HashSet<ChunkPos> _requestedChunks = new(), _requestedChunks2 = new();
        private readonly ConcurrentQueue<Chunk> _generatedChunks = new();
        private readonly ManualResetEventSlim _generatedChunksUpdateEvent = new();

        public ProgressiveChunkLoader(Action<ChunkPos> loadFunc)
        {
            _generationThread = new Thread(() =>
            {
                while (_generationThreadActive)
                {
                    lock (_lockObject)
                    {
                        _generatedChunksUpdateEvent.Reset();
                        (_requestedChunks2, _requestedChunks) = (_requestedChunks, _requestedChunks2);
                    }
                    
                    Parallel.ForEach(_requestedChunks2, loadFunc);
                    _requestedChunks2.Clear();
                    
                    _generatedChunksUpdateEvent.Wait();
                }
            });
            _generationThread.Start();
        }

        public void Dispose()
        {
            _generationThreadActive = false;
            _generatedChunksUpdateEvent.Set();
            _generationThread.Join();
        }

        public void Request(IEnumerable<ChunkPos> chunks)
        {
            foreach (var chunk in chunks)
            {
                _requestedChunks.Add(chunk);
            }

            lock (_lockObject)
            {
                _generatedChunksUpdateEvent.Set();
            }
        }
    }
}