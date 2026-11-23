using System;
using System.Collections.Generic;

namespace Lightsaber.Game
{
	public interface ISaberPlayerRegistryReadOnly
	{
		IReadOnlyList<SaberView> Players { get; }

		event Action<SaberView> PlayerRegistered;
		event Action<SaberView> PlayerUnregistered;
	}
}
