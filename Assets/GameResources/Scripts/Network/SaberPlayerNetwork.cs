using Mirror;
using UnityEngine;
using Lightsaber.Game;

namespace Lightsaber.Network
{
	// Транспорт игрока меча: только приём сырых данных гироскопа с телефона.
	// Вся математика (оси, хват, калибровка, сглаживание) будет жить в Lightsaber.Game — здесь её нет.
	public class SaberPlayerNetwork : NetworkBehaviour
	{
		public int PlayerIndex { get; private set; }
		public Quaternion RawAttitude { get; private set; } = Quaternion.identity;
		public Vector3 RawRotationRate { get; private set; }
		public Vector3 RawUserAcceleration { get; private set; }

		[SerializeField] private GameObject _phoneModelPrefab;
		[SerializeField] private float _sendRateHz = 60f;

		private float _sendInterval;
		private float _timeSinceLastSend;
		private PhoneModelView _phoneModelView;

		private void Awake()
		{
			_sendInterval = 1f / Mathf.Max(_sendRateHz, 1f);
		}

		// Выполняется один раз и только на серверной копии объекта (на хосте).
		// Модель-«телефон» нужна только там — показать её на экране телефона смысла нет.
		public override void OnStartServer()
		{
			base.OnStartServer();

			if (_phoneModelPrefab == null)
				return;

			GameObject modelInstance = Instantiate(_phoneModelPrefab, transform.position, transform.rotation);
			modelInstance.TryGetComponent(out _phoneModelView);
		}

		public override void OnStartLocalPlayer()
		{
			base.OnStartLocalPlayer();
			Input.gyro.enabled = true;
		}

		private void Update()
		{
			if (!isLocalPlayer)
				return;

			_timeSinceLastSend += Time.deltaTime;
			if (_timeSinceLastSend < _sendInterval)
				return;

			_timeSinceLastSend = 0f;

			Gyroscope gyro = Input.gyro;
			CmdSendRawData(gyro.attitude, gyro.rotationRate, gyro.userAcceleration);
		}

		// Вызывает CustomNetworkManager сразу после спавна, один раз.
		public void ServerSetIndex(int index)
		{
			PlayerIndex = index;
		}

		// Канал Unreliable: пакет может потеряться без ретрансляции — это ок,
		// следующий придёт через 1/sendRateHz секунды, ждать подтверждения смысла нет.
		[Command(channel = Channels.Unreliable)]
		private void CmdSendRawData(Quaternion attitude, Vector3 rotationRate, Vector3 userAcceleration)
		{
			RawAttitude = attitude;
			RawRotationRate = rotationRate;
			RawUserAcceleration = userAcceleration;

			_phoneModelView?.SetRawAttitude(attitude);
		}
	}
}
