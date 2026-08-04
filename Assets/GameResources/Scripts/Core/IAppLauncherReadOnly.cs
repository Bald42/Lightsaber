using System;

namespace Lightsaber.Core
{
    public interface IAppLauncherReadOnly
    {
        event Action GameReadyEvent;
        bool IsGameReady { get; }
    }
}