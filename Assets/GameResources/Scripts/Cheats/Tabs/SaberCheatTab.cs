using Lightsaber.Game;
using Lightsaber.Network;
using UnityEngine;
using Zenject;

namespace Lightsaber.Cheats
{
	public class SaberCheatTab : CheatTab
	{
		protected override string Name => "Saber";

		[Inject] private readonly ISaberPlayerRegistryReadOnly _registry;
		[Inject] private readonly INetworkRoleManagerReadOnly _networkRoleManager;

		private bool _isFrozen;
		private bool _isPhoneModelVisible = true;

		// Весь чит — только на хосте, с телефона эти цифры читать неудобно.
		public override void Initialize()
		{
			if (_networkRoleManager.CurrentRole != NetworkRole.Host)
			{
				return;
			}

			AddButton("Calibrate", Calibrate);
			AddButton("Reset calibration", ResetCalibration);

			AddOption("Freeze saber", () => _isFrozen, SetFrozen);
			AddOption("Show phone model", () => _isPhoneModelVisible, SetPhoneModelVisible);

			AddOption("Raw attitude", () => FormatQuaternion(FirstPlayer?.LastRawAttitude), _ => { });
			AddOption("Processed rotation", () => FormatQuaternion(FirstPlayer?.LastProcessedRotation), _ => { });
		}

		private SaberView FirstPlayer => _registry.Players.Count > 0 ? _registry.Players[0] : null;

		private void Calibrate()
		{
			foreach (SaberView view in _registry.Players)
			{
				view.Calibrate();
			}
		}

		private void ResetCalibration()
		{
			foreach (SaberView view in _registry.Players)
			{
				view.ResetCalibration();
			}
		}

		private void SetFrozen(bool value)
		{
			_isFrozen = value;
			foreach (SaberView view in _registry.Players)
			{
				view.IsFrozen = value;
			}
		}

		private void SetPhoneModelVisible(bool value)
		{
			_isPhoneModelVisible = value;
			foreach (SaberView view in _registry.Players)
			{
				view.SetPhoneModelVisible(value);
			}
		}

		// Готовая строка под SRDebugger: и сырые компоненты, и градусы — по градусам
		// проще сверяться с таблицей поз, чем по x/y/z/w.
		private static string FormatQuaternion(Quaternion? quaternion)
		{
			if (quaternion == null)
			{
				return "нет игрока";
			}

			Quaternion q = quaternion.Value;
			Vector3 euler = q.eulerAngles;
			return $"xyzw=({q.x:F2}, {q.y:F2}, {q.z:F2}, {q.w:F2}) | euler=({euler.x:F1}, {euler.y:F1}, {euler.z:F1})";
		}
	}
}
