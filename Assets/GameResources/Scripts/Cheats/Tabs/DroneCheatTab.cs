using Lightsaber.Game;
using Lightsaber.Network;
using UnityEngine;
using Zenject;

namespace Lightsaber.Cheats
{
	public class DroneCheatTab : CheatTab
	{
		protected override string Name => "Drone";

		[Inject] private readonly INetworkRoleManagerReadOnly _networkRoleManager;
		[Inject] private readonly IDroneDuelManager _duelManager;
		[Inject] private readonly DroneSettings _settings;

		private DroneController _droneController;
		private bool _isPaused;

		// Чит — только на хосте, вся логика дуэли и так считается там же.
		public override void Initialize()
		{
			if (_networkRoleManager.CurrentRole != NetworkRole.Host)
			{
				return;
			}

			// Дрон один на сцену (см. docs/features/6_DroneDuel.md) — полноценный реестр, как у
			// мечей (там N игроков), пока лишний.
			_droneController = Object.FindFirstObjectByType<DroneController>();

			AddOption("Drone HP", () => _duelManager.DroneHp, _ => { });
			AddOption("Player HP", () => _duelManager.PlayerHp, _ => { });
			AddButton("Добить дрона", () => _duelManager.SetDroneHp(0));
			AddButton("Добить игрока", () => _duelManager.SetPlayerHp(0));
			AddButton("Сбросить дуэль", ResetDuel);

			AddOption("Пауза дрона", () => _isPaused, SetPaused);

			AddOption("Скорость дрона", () => _settings.MoveSpeed, value => _settings.MoveSpeed = value);
			AddOption("Интервал стрельбы", () => _settings.FireInterval, value => _settings.FireInterval = value);
			AddOption("Скорость снаряда", () => _settings.ProjectileSpeed, value => _settings.ProjectileSpeed = value);
			AddOption("Разброс отбития", () => _settings.ReflectSpread, value => _settings.ReflectSpread = value);
		}

		private void ResetDuel()
		{
			_duelManager.ResetDuel();
			_droneController?.ResetPosition();
		}

		private void SetPaused(bool value)
		{
			_isPaused = value;
			if (_droneController != null)
			{
				_droneController.IsPaused = value;
			}
		}
	}
}
