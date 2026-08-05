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
		[SerializeField] private Button _playButton;

		[Header("Status Displays")]
		[SerializeField] private TextMeshProUGUI _statusText;

		[Inject] private INetworkConnectionController _connectionController;

		private void OnEnable()
		{
			_connectionController.OnClientConnectedEvent += OnClientConnectedHandler;
			_connectionController.OnClientDisconnectedEvent += OnClientDisconnectedHandler;
			_connectionController.OnConnectionFailedEvent += OnConnectionFailedHandler;

			if (_pinInputField != null)
			{
				_pinInputField.onValueChanged.AddListener(OnPinValueChanged);
			}

			if (_playButton != null)
			{
				_playButton.onClick.AddListener(OnConnectButtonClicked);
			}

			ResetUi();
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

			if (_playButton != null)
			{
				_playButton.onClick.RemoveListener(OnConnectButtonClicked);
			}
		}

		private void OnPinValueChanged(string value)
		{
			ClearError();
		}

		private void OnConnectButtonClicked()
		{
			if (_pinInputField == null)
			{
				return;
			}

			if (_pinInputField.text.Length != 4)
			{
				UpdateStatus("Invalid PIN...");
				return;
			}

			if (_connectionController.IsConnecting)
			{
				UpdateStatus("Connecting in progress...");
				return;
			}

			string pin = _pinInputField.text.Trim();

			SetConnectingState(true);
			UpdateStatus("Connecting to server...");

			_connectionController.ConnectByPin(pin);
		}

		private void OnClientConnectedHandler()
		{
			SetConnectingState(false);
			UpdateStatus("Successfully connected!");
			ClearError();
		}

		private void OnClientDisconnectedHandler()
		{
			SetConnectingState(false);
			UpdateStatus("Disconnected");
		}

		private void OnConnectionFailedHandler(string errorMessage)
		{
			SetConnectingState(false);
			UpdateStatus(string.Empty);

			if (_statusText != null)
			{
				_statusText.text = errorMessage;
			}
		}

		private void SetConnectingState(bool isConnecting)
		{
			if (_pinInputField != null)
			{
				_pinInputField.interactable = !isConnecting;
			}

			if (_playButton != null)
			{
				_playButton.interactable = !isConnecting;
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
			SetConnectingState(false);
			UpdateStatus(string.Empty);
			ClearError();
		}
	}
}