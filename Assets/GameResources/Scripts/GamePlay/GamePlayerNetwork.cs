using Mirror;
using UnityEngine;

namespace Lightsaber.Network
{
	public class GamePlayerNetwork : NetworkBehaviour
	{
		[Header("Rotation Target")]
		[SerializeField] private Transform bladeTransform;

		[Header("Smoothing & Sensitivity")]
		[SerializeField] private float gyroSmoothSpeed = 12f;
		[SerializeField] private float mouseSensitivity = 2f;
		[SerializeField] private bool requireRightClick = true;

		[Header("Live Tuning (Host to Client)")]
		[SerializeField] private Vector3 debugCorrectionAngles = new Vector3(0f, -90f, 90f);
		[SerializeField] private Vector4 debugGyroInvertMask = new Vector4(1f, 1f, -1f, -1f);

		// Приватные рабочие переменные
		private bool _isGyroSupported;
		private Quaternion _gyroInitialOffset = Quaternion.identity;
		private Quaternion _targetRotation;
		private Vector3 _simulatedEuler;

		private Quaternion _correction;
		private Vector4 _gyroInvertMask;

		private void Start()
		{
			// Устанавливаем стартовые значения
			_correction = Quaternion.Euler(debugCorrectionAngles);
			_gyroInvertMask = debugGyroInvertMask;

			Debug.Log($"<color=yellow>[GamePlayerNetwork]</color> Spawned! isOwned: {isOwned}");

			if (!isOwned) return;

			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			_isGyroSupported = SystemInfo.supportsGyroscope;

			if (_isGyroSupported)
			{
				Input.gyro.enabled = true;
				RecalibrateGyro();
			}
			else
			{
				Debug.Log("<color=cyan>[GamePlayerNetwork]</color> Gyro not supported. Mouse enabled.");
				if (bladeTransform != null)
				{
					_simulatedEuler = bladeTransform.localEulerAngles;
					_targetRotation = bladeTransform.localRotation;
				}
			}
		}

		private void Update()
		{
			// if (!isOwned) return;

			// Калибровка
			if (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
			{
				RecalibrateGyro();
			}

			if (_isGyroSupported && Input.gyro.enabled)
			{
				Quaternion rawGyro = ConvertRotation(Input.gyro.attitude);
				_targetRotation = _gyroInitialOffset * rawGyro;
			}
			else
			{
				HandleMouseInput();
			}

			if (bladeTransform != null)
			{
				bladeTransform.localRotation = Quaternion.Slerp(
					bladeTransform.localRotation,
					_targetRotation,
					Time.deltaTime * gyroSmoothSpeed
				);
			}
		}

		public void RecalibrateGyro()
		{
			if (_isGyroSupported && Input.gyro.enabled)
			{
				Quaternion currentRaw = ConvertRotation(Input.gyro.attitude);
				_gyroInitialOffset = Quaternion.Inverse(currentRaw);
				Debug.Log("<color=green>[GamePlayerNetwork]</color> Gyro Recalibrated!");
			}
		}

		private void HandleMouseInput()
		{
			if (bladeTransform == null) return;

			if (requireRightClick && !Input.GetMouseButton(1)) return;

			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

			_simulatedEuler.x -= mouseY;
			_simulatedEuler.y += mouseX;

			_targetRotation = Quaternion.Euler(_simulatedEuler);
		}

		private Quaternion ConvertRotation(Quaternion q)
		{
			Quaternion unityGyro = new Quaternion(
				q.x * _gyroInvertMask.x,
				q.y * _gyroInvertMask.y,
				q.z * _gyroInvertMask.z,
				q.w * _gyroInvertMask.w
			);

			return unityGyro * _correction;
		}

		// --- ПРИНУДИТЕЛЬНАЯ ОТПРАВКА НА ВСЕ КЛИЕНТЫ И КЛОНЫ ---

		/// <summary>
		/// Вызывается кнопкой в инспекторе на Хосте
		/// </summary>
		[ContextMenu("Send Config To Client")]
		public void SendConfigToClient()
		{
			// if (!isServer)
			// {
			// 	Debug.LogWarning("[GamePlayerNetwork] SendConfigToClient can only be called on Host/Server!");
			// 	return;
			// }

			// Вызываем ClientRpc, чтобы принудительно протолкнуть новые конфиги во все клиенты
			RpcApplyConfig(debugCorrectionAngles, debugGyroInvertMask);
			Debug.Log($"<color=cyan>[GamePlayerNetwork]</color> Host broadcasted config: Angles={debugCorrectionAngles}, Mask={debugGyroInvertMask}");
		}

		[ClientRpc]
		private void RpcApplyConfig(Vector3 newAngles, Vector4 newMask)
		{
			_correction = Quaternion.Euler(newAngles);
			_gyroInvertMask = newMask;
			debugCorrectionAngles = newAngles;
			debugGyroInvertMask = newMask;
			if (isOwned)
			{
				RecalibrateGyro();
			}

			Debug.Log($"<color=green>[GamePlayerNetwork]</color> RpcApplyConfig received on Client/Clone! Angles={newAngles}, Mask={newMask}");
		}
	}
}