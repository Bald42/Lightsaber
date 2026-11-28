using UnityEngine;

namespace Lightsaber.Game
{
	// Чистая математика поворота меча: без Mirror, без Zenject-атрибутов,
	// один экземпляр на игрока, создаётся снаружи (см. SaberView).
	// меч = калибровка * поправкаМира * конвертация(raw) * поправкаХвата
	public class SaberRotationProcessor
	{
		// Клинок ближе 30° к вертикали: по нему не понять сторону света, калибровку пропускаем.
		private const float MIN_BLADE_HORIZONTAL = 0.5f;

		// Поворот мира, умножается слева: у гироскопа Android небо по оси Z, у Unity по Y.
		// Это факт систем координат, а не настройка, — поэтому константа, а не поле в SaberSettings.
		private static readonly Quaternion WORLD_CORRECTION = Quaternion.Euler(90f, 0f, 0f);

		private readonly SaberSettings _settings;
		private Quaternion _calibrationYaw = Quaternion.identity;

		public SaberRotationProcessor(SaberSettings settings)
		{
			_settings = settings;
		}

		public Quaternion Process(Quaternion rawAttitude)
		{
			return _calibrationYaw * Uncalibrated(rawAttitude);
		}

		// Калибровка меняет только поворот вокруг вертикали (docs/features/4_SaberRotation.md, п.4).
		// Делается в позе 1: телефон на столе, динамик к монитору. Клинок горизонтален,
		// его проекцию на пол доворачиваем на CalibrationTargetYaw.
		public bool Calibrate(Quaternion currentRawAttitude)
		{
			Vector3 blade = Uncalibrated(currentRawAttitude) * Vector3.up;
			Vector3 bladeFlat = new Vector3(blade.x, 0f, blade.z);

			if (bladeFlat.magnitude < MIN_BLADE_HORIZONTAL)
			{
				return false;
			}

			// SignedAngle вокруг Vector3.up, а не FromToRotation: при развороте ровно на 180°
			// FromToRotation может выбрать ось наклона вместо вертикали.
			Vector3 target = Quaternion.Euler(0f, _settings.CalibrationTargetYaw, 0f) * Vector3.forward;
			float yaw = Vector3.SignedAngle(bladeFlat, target, Vector3.up);
			_calibrationYaw = Quaternion.AngleAxis(yaw, Vector3.up);
			return true;
		}

		public void ResetCalibration()
		{
			_calibrationYaw = Quaternion.identity;
		}

		private Quaternion Uncalibrated(Quaternion rawAttitude)
		{
			return WORLD_CORRECTION * ConvertToUnity(rawAttitude) * Quaternion.Euler(_settings.GripCorrectionEuler);
		}

		// Гироскоп правосторонний, Unity левосторонняя — пример из документации Unity (Gyroscope).
		private static Quaternion ConvertToUnity(Quaternion q)
		{
			return new Quaternion(q.x, q.y, -q.z, -q.w);
		}
	}
}
