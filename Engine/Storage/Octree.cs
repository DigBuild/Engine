using System.Collections;
using System.Collections.Generic;
using DigBuild.Engine.Math;

namespace DigBuild.Engine.Storage
{
    public sealed class Octree<T> : IOctree<T>
    {
        private readonly Level _topLevel;

        public uint Levels { get; }

        public Octree(byte levels, T defaultValue)
        {
            _topLevel = new Level(levels, defaultValue);
            Levels = levels;
        }

        public T this[int x, int y, int z]
        {
            get => _topLevel.Get(x, y, z);
            set => _topLevel.Set(x, y, z, value);
        }

        public IEnumerable<KeyValuePair<Vector3I, T>> EnumerateNonNull() => _topLevel.EnumerateNonNull();

        public IEnumerator<KeyValuePair<Vector3I, T>> GetEnumerator() => _topLevel.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class Level
        {
            // 7 bits (level) + 1 bit (subdivided)
            private byte _flags;
            private object? _value;

            public Level(byte level, T? value)
            {
                _flags = (byte) (level << 1);
                _value = value;
            }

            private bool GetDirect(out T? value)
            {
                var subdivided = _flags & 1;
                if (subdivided == 0)
                {
                    value = (T?) _value;
                    return true;
                }
                value = default;
                return false;
            }

            public T Get(int x, int y, int z)
            {
                if (_value == null!)
                    return default!;

                var subdivided = _flags & 1;
                if (subdivided == 0)
                    return (T) _value;

                var nextLevel = (_flags >> 1) - 1;
                var mask = ~(~0 << nextLevel);

                var children = (Level[,,]) _value;
                var child = children[x >> nextLevel, y >> nextLevel, z >> nextLevel];
                return child.Get(x & mask, y & mask, z & mask);
            }

            public bool Set(int x, int y, int z, T? value)
            {
                var level = _flags >> 1;

                // If this is the lowest level, update the value
                if (level == 0)
                {
                    if (_value?.Equals(value) ?? value == null)
                        return false;
                    _value = value;
                    return true;
                }

                var nextLevel = level - 1;
                var subdivided = _flags & 1;
                var mask = ~(~0 << nextLevel);

                // If this level is subdivided, pass it on
                if (subdivided == 1)
                {
                    var children = (Level[,,]) _value!;
                    var child = children[x >> nextLevel, y >> nextLevel, z >> nextLevel];
                    
                    var collapsed = child.Set(x & mask, y & mask, z & mask, value);

                    // If the previous level collapsed, check if this one should too
                    if (!collapsed)
                        return false;

                    foreach (var c in children)
                    {
                        if (!c.GetDirect(out var v) || !(v?.Equals(value) ?? value == null))
                            return false;
                    }

                    // All the children had the same value, so we'll take on that value too
                    _value = value;
                    _flags &= 0xFE; // Remove subdivided bit

                    return true;
                }

                // This level is not subdivided

                // If this level has the same value we're setting, we're done
                if (_value?.Equals(value) ?? value == null)
                    return false;
                
                // It's not the same value - subdivide the current level
                var childrenLevel = (byte) nextLevel;
                var childrenValue = (T?) _value;
                var newChildren = new Level[2, 2, 2];

                _value = newChildren;
                _flags |= 1; // Set subdivided bit
                
                for (var i = 0; i <= 1; i++)
                for (var j = 0; j <= 1; j++)
                for (var k = 0; k <= 1; k++)
                    newChildren[i, j, k] = new Level(childrenLevel, childrenValue);
                
                var modifiedChild = newChildren[x >> nextLevel, y >> nextLevel, z >> nextLevel];
                modifiedChild.Set(x & mask, y & mask, z & mask, value);

                return false;
            }

            public IEnumerable<KeyValuePair<Vector3I, T>> EnumerateNonNull()
            {
                if (_value == default)
                    yield break;

                var subdivided = _flags & 1;
                var level = _flags >> 1;

                if (subdivided == 0)
                {
                    var levelSize = 1 << level;
                    for (var i = 0; i < levelSize; i++)
                    for (var j = 0; j < levelSize; j++)
                    for (var k = 0; k < levelSize; k++)
                        yield return new KeyValuePair<Vector3I, T>(new Vector3I(i, j, k), (T) _value!);
                }
                else
                {
                    var nextLevel = level - 1;

                    var children = (Level[,,]) _value!;

                    for (var i = 0; i <= 1; i++)
                    {
                        var iOff = i << nextLevel;
                        for (var j = 0; j <= 1; j++)
                        {
                            var jOff = j << nextLevel;
                            for (var k = 0; k <= 1; k++)
                            {
                                var kOff = k << nextLevel;
                                foreach (var ((x, y, z), value) in children[i, j, k].EnumerateNonNull())
                                {
                                    yield return new KeyValuePair<Vector3I, T>(
                                        new Vector3I(iOff | x, jOff | y, kOff | z),
                                        value
                                    );
                                }
                            }
                        }
                    }
                }
            }

            public IEnumerator<KeyValuePair<Vector3I, T>> GetEnumerator()
            {
                var subdivided = _flags & 1;
                var level = _flags >> 1;

                if (subdivided == 0)
                {
                    var levelSize = 1 << level;
                    for (var i = 0; i < levelSize; i++)
                    for (var j = 0; j < levelSize; j++)
                    for (var k = 0; k < levelSize; k++)
                        yield return new KeyValuePair<Vector3I, T>(new Vector3I(i, j, k), (T) _value!);
                }
                else
                {
                    var nextLevel = level - 1;

                    var children = (Level[,,]) _value!;

                    for (var i = 0; i <= 1; i++)
                    {
                        var iOff = i << nextLevel;
                        for (var j = 0; j <= 1; j++)
                        {
                            var jOff = j << nextLevel;
                            for (var k = 0; k <= 1; k++)
                            {
                                var kOff = k << nextLevel;
                                foreach (var ((x, y, z), value) in children[i, j, k])
                                {
                                    yield return new KeyValuePair<Vector3I, T>(
                                        new Vector3I(iOff | x, jOff | y, kOff | z),
                                        value
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}