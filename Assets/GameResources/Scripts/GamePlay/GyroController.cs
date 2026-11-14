using UnityEngine;

namespace Lightsaber.InputSystem
{
	public class GyroController : MonoBehaviour
	{
		private bool _isGyroSupported;
		private Quaternion _baseRotationCorrection;

		private void Start()
		{
			// Проверяем поддержку гироскопа на устройстве
			_isGyroSupported = SystemInfo.supportsGyroscope;

			if (_isGyroSupported)
			{
				EnableGyro();

				// Корректирующий поворот для компенсации разницы осей гироскопа и Unity
				_baseRotationCorrection = Quaternion.Euler(90f, 90f, 0f);
			}
			else
			{
				Debug.LogWarning("[GyroController] Gyroscope is not supported on this device.");
			}
		}

		private void Update()
		{
			if (!_isGyroSupported || !Input.gyro.enabled)
			{
				return;
			}

			// Преобразуем кватернион ориентации гироскопа в координаты Unity
			transform.localRotation = ConvertRotation(Input.gyro.attitude);
		}

		public void EnableGyro()
		{
			if (_isGyroSupported)
			{
				Input.gyro.enabled = true;
			}
		}

		public void DisableGyro()
		{
			if (_isGyroSupported)
			{
				Input.gyro.enabled = false;
			}
		}

		/// <summary>
		/// Конвертация ориентации гироскопа из правосторонней системы координат в левостороннюю Unity
		/// </summary>
		private Quaternion ConvertRotation(Quaternion q)
		{
			// Инвертируем оси Z и W для корректного отображения вращения в Unity
			return new Quaternion(q.x, q.y, -q.z, -q.w) * _baseRotationCorrection;
		}
	}
}