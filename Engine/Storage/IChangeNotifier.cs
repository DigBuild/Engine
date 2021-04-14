using System;

namespace DigBuild.Engine.Storage
{
    public interface IChangeNotifier
    {
        event Action Changed;
    }
}