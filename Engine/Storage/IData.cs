using System;

namespace DigBuild.Engine.Storage
{
    /// <summary>
    /// A data class.
    /// </summary>
    public interface IData
    {
        internal IData Copy();
    }

    /// <summary>
    /// A data class of a certain type.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public interface IData<out T> : IData where T : IData<T>
    {
        /// <summary>
        /// Creates a deep copy of this data class.
        /// </summary>
        /// <returns></returns>
        new T Copy();
        IData IData.Copy() => Copy();
    }
}