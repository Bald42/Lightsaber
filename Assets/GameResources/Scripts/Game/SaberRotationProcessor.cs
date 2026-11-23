using UnityEngine;

namespace Lightsaber.Game
{
	// Чистая математика поворота меча: без Mirror, без Zenject-атрибутов,
	// один экземпляр на игрока, создаётся снаружи (см. SaberView).
	public class SaberRotationProcessor
	{
		private readonly SaberSettings _settings;

		public SaberRotationProcessor(SaberSettings settings)
		{
			_settings = settings;
		}

		public Quaternion Process(Quaternion rawAttitude)
		{
			Quaternion signed = ApplyAxisSigns(rawAttitude);
			Quaternion converted = ConvertToUnity(signed);
			Quaternion withGrip = converted * Quaternion.Euler(_settings.GripCorrectionEuler);

			// Калибровка по yaw — заглушка на этом шаге, шаг 5 подставит реальный поворот.
			return Quaternion.identity * withGrip;
		}

		// Инверсия знака компоненты — рабочий приём для перебора осей по таблице поз
		// (docs/features/4_SaberRotation.md), а не строгое математическое отражение поворота.
		private Quaternion ApplyAxisSigns(Quaternion raw)
		{
			return new Quaternion(
				_settings.InvertX ? -raw.x : raw.x,
				_settings.InvertY ? -raw.y : raw.y,
				_settings.InvertZ ? -raw.z : raw.z,
				raw.w);
		}

		// Гироскоп правосторонний, Unity левосторонняя — подтверждено документацией Unity (Gyroscope).
		private static Quaternion ConvertToUnity(Quaternion q)
		{
			return new Quaternion(q.x, q.y, -q.z, -q.w);
		}
	}
}
