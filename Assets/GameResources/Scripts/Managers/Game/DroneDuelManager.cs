using System;
using UnityEngine;

namespace Lightsaber.Game
{
	// Урон и HP дуэли с дроном: чистая логика, без Mirror — считается только на хосте
	// (см. docs/features/6_DroneDuel.md, п.9). Это не "игра с концом": HP просто показывает,
	// что попадание засчиталось, в 0 упирается и дальше не падает — ни победы, ни проигрыша.
	public class DroneDuelManager : IDroneDuelManager
	{
		public int DroneHp { get; private set; }
		public int PlayerHp { get; private set; }

		public event Action DroneDamaged;
		public event Action PlayerDamaged;

		private readonly DroneSettings _settings;

		public DroneDuelManager(DroneSettings settings)
		{
			_settings = settings;
			ResetDuel();
		}

		public void DamageDrone()
		{
			DroneHp = Math.Max(0, DroneHp - _settings.DamageToDrone);
			DroneDamaged?.Invoke();
			Debug.Log($"[DroneDuel] Дрон получил урон, HP: {DroneHp}");
		}

		public void DamagePlayer()
		{
			PlayerHp = Math.Max(0, PlayerHp - _settings.DamageToPlayer);
			PlayerDamaged?.Invoke();
			Debug.Log($"[DroneDuel] Игрок получил урон, HP: {PlayerHp}");
		}

		// Для чита «Добить» — выставить HP напрямую, без привязки к DamageToDrone/DamageToPlayer.
		public void SetDroneHp(int value)
		{
			DroneHp = Mathf.Max(0, value);
			DroneDamaged?.Invoke();
			Debug.Log($"[DroneDuel] HP дрона выставлено вручную: {DroneHp}");
		}

		public void SetPlayerHp(int value)
		{
			PlayerHp = Mathf.Max(0, value);
			PlayerDamaged?.Invoke();
			Debug.Log($"[DroneDuel] HP игрока выставлено вручную: {PlayerHp}");
		}

		public void ResetDuel()
		{
			DroneHp = _settings.DroneHp;
			PlayerHp = _settings.PlayerHp;
		}
	}
}
