using System;

namespace DigBuild.Engine.Storage
{
    /// <summary>
    /// A type that can notify changes.
    /// </summary>
    public interface IChangeNotifier
    {
        /// <summary>
        /// Fired when an internal change occurs.
        /// </summary>
        event Action Changed;
    }
}