using Mirror;
using UnityEngine;

namespace Lightsaber.Network
{
	public class TestGyro : MonoBehaviour
	{
		[Header("Rotation Target")]
		[SerializeField] private Transform bladeTransform;

		[Header("Gyro Control Settings")]
		[SerializeField] private float speed = 1.0f;
		[SerializeField] private float smoothing = 0.1f; // Чем меньше (0.01 - 0.2), тем плавнее
		[SerializeField] private bool waitGyroInitialization = true;
		[SerializeField] private float waitGyroDuration = 0.5f;

		// Приватные переменные скрипта
		private bool _isGyroSupported;
		private bool _isGyroInitialized;
		private Quaternion _initialRotation = Quaternion.identity;
		private Quaternion _gyroOffset = Quaternion.identity;
		private Quaternion _targetRotation = Quaternion.identity;

		private void Start()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			_isGyroSupported = SystemInfo.supportsGyroscope;

			if (_isGyroSupported)
			{
				Input.gyro.enabled = true;
				
				if (waitGyroInitialization)
				{
					Invoke(nameof(InitGyro), waitGyroDuration);
				}
				else
				{
					InitGyro();
				}
			}
		}

		private void InitGyro()
		{
			if (bladeTransform != null)
			{
				_initialRotation = bladeTransform.localRotation;
			}

			// Фиксируем стартовый офсет датчика
			_gyroOffset = GetGyroAttitude();
			_isGyroInitialized = true;
			
			Debug.Log("<color=green>[GyroControl]</color> Gyroscope Initialized!");
		}

		private void Update()
		{
			if (!_isGyroSupported || !_isGyroInitialized) return;

			// Сброс калибровки "вперед" по тапу или пробелу
			if (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
			{
				RecalibrateGyro();
			}

			ApplyGyroRotation();
		}

		/// <summary>
		/// Преобразует сырые данные Android в правильные оси Unity
		/// </summary>
		private Quaternion GetGyroAttitude()
		{
			Quaternion raw = Input.gyro.attitude;
			// Перевод из правой системы координат в левую Unity
			return new Quaternion(raw.x, raw.y, -raw.z, -raw.w);
		}

		private void ApplyGyroRotation()
		{
			if (bladeTransform == null) return;

			// Считаем разницу относительно точки калибровки
			Quaternion currentGyro = GetGyroAttitude();
			Quaternion offsetRotation = currentGyro * Quaternion.Inverse(_gyroOffset);

			// Для хвата БОКОМ к экрану (кнопки в монитор):
			// X отвечает за наклоны вперед/назад, Z за вращение кисти, Y за повороты влево/вправо
			Quaternion tempTarget = new Quaternion(
				offsetRotation.x * speed,
				offsetRotation.y * speed,
				offsetRotation.z * speed,
				offsetRotation.w * speed
			);

			_targetRotation = _initialRotation * tempTarget;

			// Плавный Lerp без рывков
			bladeTransform.localRotation = Quaternion.Lerp(
				bladeTransform.localRotation, 
				_targetRotation, 
				smoothing
			);
		}

		public void RecalibrateGyro()
		{
			if (_isGyroSupported && Input.gyro.enabled)
			{
				_gyroOffset = GetGyroAttitude();
				Debug.Log("<color=green>[GyroControl]</color> Recalibrated!");
			}
		}
	}
}