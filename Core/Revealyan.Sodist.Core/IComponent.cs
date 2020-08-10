using System;

namespace Revealyan.Sodist.Core
{
    public interface IComponent
    {
        string Name { get; }
        void Startup();
        void Shutdown();
        void Reboot();
    }
}
