using System;
using DigBuild.Engine.Storage;

namespace DigBuild.Engine.Items
{
    /// <summary>
    /// A read only instance of an item.
    /// </summary>
    public interface IReadOnlyItemInstance : IEquatable<IReadOnlyItemInstance>
    {
        /// <summary>
        /// The item type.
        /// </summary>
        public Item Type { get; }
        /// <summary>
        /// The amount.
        /// </summary>
        public ushort Count { get; }
        internal DataContainer? DataContainer { get; }
        
        /// <summary>
        /// Gets an attribute of this item.
        /// </summary>
        /// <typeparam name="TAttrib">The attribute type</typeparam>
        /// <param name="attribute">The attribute</param>
        /// <returns>The value</returns>
        TAttrib Get<TAttrib>(ItemAttribute<TAttrib> attribute);

        /// <summary>
        /// Tests for equality with another item.
        /// </summary>
        /// <param name="other">The other item</param>
        /// <param name="ignoreCount">Whether to ignore counts and just match on type, or not</param>
        /// <param name="testEmpty">Whether to test empty items instead of ignoring them, or not</param>
        /// <returns>Whether both items are deemed equal</returns>
        bool Equals(IReadOnlyItemInstance other, bool ignoreCount = false, bool testEmpty = false);
        
        /// <summary>
        /// Creates a deep copy of the item.
        /// </summary>
        /// <returns>The copy</returns>
        ItemInstance Copy();
    }
}