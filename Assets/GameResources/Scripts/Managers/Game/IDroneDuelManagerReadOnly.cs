using System;

namespace Lightsaber.Game
{
	public interface IDroneDuelManagerReadOnly
	{
		int DroneHp { get; }
		int PlayerHp { get; }

		event Action DroneDamaged;
		event Action PlayerDamaged;
	}
}
