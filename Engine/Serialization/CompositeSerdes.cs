using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace DigBuild.Engine.Serialization
{
    public sealed class CompositeSerdes<T> : ISerdes<T>, IEnumerable
    {
        private readonly Func<T> _factory;
        private readonly Dictionary<uint, IMemberSerializer> _serializers = new();
        private readonly SortedSet<IMemberSerializer> _sortedSerializers = new(
            Comparer<IMemberSerializer>.Create((a, b) => a.Index.CompareTo(b.Index))
        );
        private readonly Dictionary<uint, IMemberDeserializer> _deserializers = new();

        public CompositeSerdes(Func<T> factory)
        {
            _factory = factory;
        }

        public void Add<TVal>(uint index, Expression<Func<T, TVal>> member, ISerdes<TVal> serdes)
        {
            var ser = new MemberSerializer<TVal>(index, member, serdes);
            _serializers[index] = ser;
            _sortedSerializers.Add(ser);
            _deserializers[index] = new MemberDeserializer<TVal>(member, serdes);
        }

        public void Add<TVal>(int index, Expression<Func<T, TVal>> member, IDeserializer<TVal> serdes)
        {
            _deserializers[(uint) System.Math.Abs(index)] = new MemberDeserializer<TVal>(member, serdes);
        }

        public void Serialize(Stream stream, T obj)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(_sortedSerializers.Count);
            foreach (var serializer in _sortedSerializers)
            {
                writer.Write(serializer.Index);
                serializer.Serialize(obj, stream);
            }
        }

        public T Deserialize(Stream stream)
        {
            var obj = _factory();

            var reader = new BinaryReader(stream);
            var members = reader.ReadInt32();
            for (var i = 0u; i < members; i++)
            {
                var member = reader.ReadUInt32();
                _deserializers[member].Deserialize(obj, stream);
            }

            return obj;
        }

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        private interface IMemberSerializer
        {
            internal uint Index { get; }

            void Serialize(T target, Stream stream);
        }

        private interface IMemberDeserializer
        {
            void Deserialize(T target, Stream stream);
        }

        private sealed class MemberSerializer<TVal> : IMemberSerializer
        {
            private readonly uint _index;
            private readonly Func<T, TVal> _getter;
            private readonly ISerdes<TVal> _serdes;

            uint IMemberSerializer.Index => _index;

            public MemberSerializer(uint index, Expression<Func<T, TVal>> member, ISerdes<TVal> serdes)
            {
                if (member.Body is not MemberExpression mexp || mexp.Expression is not ParameterExpression)
                    throw new ArgumentException("Expression must access a member directly.");

                _index = index;
                _getter = member.Compile();
                _serdes = serdes;
            }

            public void Serialize(T target, Stream stream)
            {
                _serdes.Serialize(stream, _getter(target));
            }
        }

        private sealed class MemberDeserializer<TVal> : IMemberDeserializer
        {
            private readonly Action<T, TVal> _setter;
            private readonly IDeserializer<TVal> _serdes;

            public MemberDeserializer(Expression<Func<T, TVal>> member, IDeserializer<TVal> serdes)
            {
                if (member.Body is not MemberExpression mexp || mexp.Expression is not ParameterExpression p)
                    throw new ArgumentException("Expression must access a member directly.");

                if (mexp.Member is PropertyInfo {SetMethod: null})
                    throw new ArgumentException($"Property is not settable: {mexp.Member.Name}");
                if (mexp.Member is FieldInfo {IsInitOnly: true})
                    throw new ArgumentException($"Field is readonly: {mexp.Member.Name}");

                var v = Expression.Parameter(typeof(TVal), "value");
                var setterExp = Expression.Lambda<Action<T, TVal>>(
                    Expression.Assign(mexp, v),
                    p, v
                );

                _setter = setterExp.Compile();
                _serdes = serdes;
            }

            public void Deserialize(T target, Stream stream)
            {
                _setter(target, _serdes.Deserialize(stream));
            }
        }
    }
}