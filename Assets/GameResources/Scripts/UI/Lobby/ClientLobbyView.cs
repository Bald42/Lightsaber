using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Lightsaber.Network.UI
{
	public class ClientLobbyView : MonoBehaviour
	{
		[Header("Input Controls")]
		[SerializeField] private TMP_InputField _pinInputField;
		[SerializeField] private Button _connectButton;
		[SerializeField] private Button _startGameButton;

		[Header("Status Displays")]
		[SerializeField] private TextMeshProUGUI _statusText;
		[SerializeField] private TextMeshProUGUI _targetIpText;

		[Header("Settings")]
		[SerializeField] private float _connectionTimeoutSeconds = 5f;

		[Inject] private INetworkConnectionController _connectionController;

		private Coroutine _timeoutCoroutine;
		private bool _isConnectingAttempt; // Флаг: крутится ли прямо сейчас таймер подключения
		private const string DEFAULT_PIN = "22FG";

		private void OnEnable()
		{
			_connectionController.OnClientConnectedEvent += OnClientConnectedHandler;
			_connectionController.OnClientDisconnectedEvent += OnClientDisconnectedHandler;
			_connectionController.OnConnectionFailedEvent += OnConnectionFailedHandler;

			if (_pinInputField != null)
			{
				_pinInputField.onValueChanged.AddListener(OnPinValueChanged);
			}

			if (_connectButton != null)
			{
				_connectButton.onClick.AddListener(OnConnectButtonClicked);
			}

			if (_startGameButton != null)
			{
				_startGameButton.onClick.AddListener(OnStartGameButtonClicked);
			}

			CheckCurrentConnectionState();
		}

		private void OnDisable()
		{
			_connectionController.OnClientConnectedEvent -= OnClientConnectedHandler;
			_connectionController.OnClientDisconnectedEvent -= OnClientDisconnectedHandler;
			_connectionController.OnConnectionFailedEvent -= OnConnectionFailedHandler;

			if (_pinInputField != null)
			{
				_pinInputField.onValueChanged.RemoveListener(OnPinValueChanged);
			}

			if (_connectButton != null)
			{
				_connectButton.onClick.RemoveListener(OnConnectButtonClicked);
			}

			if (_startGameButton != null)
			{
				_startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
			}

			StopTimeoutTimer();
		}

		private void OnPinValueChanged(string value)
		{
			if (_pinInputField != null && _pinInputField.text != value.ToUpper())
			{
				_pinInputField.text = value.ToUpper();
			}

			ClearError();
		}

		private void OnConnectButtonClicked()
		{
			if (_pinInputField == null) return;

			string pin = _pinInputField.text.Trim().ToUpper();

			if (pin.Length != 4)
			{
				UpdateStatus("Invalid PIN...");
				return;
			}

			_isConnectingAttempt = true;
			SetConnectButtonState(false);

			_connectionController.ConnectByPin(pin);

			if (_targetIpText != null && _connectionController is NetworkConnectionController controller)
			{
				_targetIpText.text = $"IP: {controller.GetTargetIp()}";
			}

			StartTimeoutTimer();
		}

		private void OnStartGameButtonClicked()
		{
			Debug.Log("<color=green>[ClientLobbyView]</color> Start Game button clicked from Client.");
			if (LobbyPlayerNetwork.LocalInstance != null)
			{
				LobbyPlayerNetwork.LocalInstance.RequestStartGame("GameScene");
			}
		}

		private void OnClientConnectedHandler()
		{
			_isConnectingAttempt = false;
			StopTimeoutTimer();

			UpdateStatus("Successfully connected!");
			ClearError();

			SetConnectButtonState(false);

			if (_startGameButton != null)
			{
				_startGameButton.gameObject.SetActive(true);
			}
		}

		private void OnClientDisconnectedHandler()
		{
			// Если идёт попытка подключения (таймер висит) — игнорируем быстрый отбой Mirror,
			// чтобы дать таймеру честно отсчитать 5 секунд.
			if (_isConnectingAttempt)
			{
				return;
			}

			StopTimeoutTimer();
			UpdateStatus("Disconnected");
			ResetUi();
		}

		private void OnConnectionFailedHandler(string errorMessage)
		{
			_isConnectingAttempt = false;
			StopTimeoutTimer();

			if (_statusText != null)
			{
				_statusText.text = errorMessage;
			}

			ResetUi();
		}

		private void CheckCurrentConnectionState()
		{
			ClearError();

			if (_connectionController.IsClientConnected)
			{
				UpdateStatus("Successfully connected!");
				SetConnectButtonState(false);

				if (_startGameButton != null)
				{
					_startGameButton.gameObject.SetActive(true);
				}

				if (_pinInputField != null)
				{
					_pinInputField.text = DEFAULT_PIN;
				}

				if (_targetIpText != null && _connectionController is NetworkConnectionController controller)
				{
					_targetIpText.text = $"IP: {controller.GetTargetIp()}";
				}
			}
			else
			{
				ResetUi();

				if (_pinInputField != null)
				{
					_pinInputField.text = DEFAULT_PIN;
				}
			}
		}

		private void StartTimeoutTimer()
		{
			StopTimeoutTimer();
			_timeoutCoroutine = StartCoroutine(ConnectionTimeoutRoutine());
		}

		private void StopTimeoutTimer()
		{
			if (_timeoutCoroutine != null)
			{
				StopCoroutine(_timeoutCoroutine);
				_timeoutCoroutine = null;
			}
		}

		private IEnumerator ConnectionTimeoutRoutine()
		{
			float remainingTime = _connectionTimeoutSeconds;

			while (remainingTime > 0)
			{
				UpdateStatus($"Connecting to server... ({Mathf.CeilToInt(remainingTime)}s)");
				yield return new WaitForSeconds(1f);
				remainingTime -= 1f;
			}

			// Прошло 5 секунд, хоста нет
			_isConnectingAttempt = false;
			_connectionController.Disconnect();

			UpdateStatus("Failed to connect: Host not found");
			ResetUi();
		}

		private void SetConnectButtonState(bool interactable)
		{
			if (_connectButton != null)
			{
				_connectButton.interactable = interactable;
			}
		}

		private void UpdateStatus(string status)
		{
			if (_statusText != null)
			{
				_statusText.text = status;
			}
		}

		private void ClearError()
		{
			if (_statusText != null)
			{
				_statusText.text = string.Empty;
			}
		}

		private void ResetUi()
		{
			SetConnectButtonState(true);

			if (_targetIpText != null)
			{
				_targetIpText.text = string.Empty;
			}

			if (_startGameButton != null)
			{
				_startGameButton.gameObject.SetActive(false);
			}
		}
	}
}