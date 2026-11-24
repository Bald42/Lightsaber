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
			Quaternion converted = ConvertToUnity(ApplyAxisSigns(rawAttitude));
			return Quaternion.Euler(_settings.WorldCorrectionEuler) * converted * Quaternion.Euler(_settings.GripCorrectionEuler);
		}

		// Инверсия знака компоненты — запасной ручной инструмент, по умолчанию выключен.
		private Quaternion ApplyAxisSigns(Quaternion raw)
		{
			return new Quaternion(
				_settings.InvertX ? -raw.x : raw.x,
				_settings.InvertY ? -raw.y : raw.y,
				_settings.InvertZ ? -raw.z : raw.z,
				raw.w);
		}

		// Гироскоп правосторонний, Unity левосторонняя — пример из документации Unity (Gyroscope).
		private static Quaternion ConvertToUnity(Quaternion q)
		{
			return new Quaternion(q.x, q.y, -q.z, -q.w);
		}
	}
}
