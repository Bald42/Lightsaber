using TMPro;
using UnityEngine;
using Zenject;

namespace Lightsaber.Network.UI
{
	public class HostLobbyView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _pinText;
		[SerializeField] private TextMeshProUGUI _ipText;
		[SerializeField] private TextMeshProUGUI _amountPlayersText;

		[Inject] private INetworkConnectionControllerReadOnly _connectionController;

		private void OnEnable()
		{
			_connectionController.OnServerStartedEvent += OnServerStartedHandler;
			_connectionController.OnPlayerCountChangedEvent += OnPlayerCountChangedHandler;

			RefreshDisplay();
		}

		private void OnDisable()
		{
			_connectionController.OnServerStartedEvent -= OnServerStartedHandler;
			_connectionController.OnPlayerCountChangedEvent -= OnPlayerCountChangedHandler;
		}

		private void RefreshDisplay()
		{
			if (_connectionController.IsServerStarted)
			{
				SetPinText(_connectionController.CurrentPinCode);
				SetIpText(_connectionController.CurrentIpAddress);
			}

			UpdatePlayerCountDisplay(_connectionController.ConnectedPlayersCount);
		}

		private void OnServerStartedHandler(ServerStartedEvent serverStartedEvent)
		{
			SetPinText(serverStartedEvent.PinCode);
			SetIpText(serverStartedEvent.IpAddress);
			UpdatePlayerCountDisplay(_connectionController.ConnectedPlayersCount);
		}

		private void OnPlayerCountChangedHandler(int currentCount)
		{
			UpdatePlayerCountDisplay(currentCount);
		}

		private void SetPinText(string pinCode)
		{
			if (_pinText != null)
			{
				_pinText.text = $"PIN: {pinCode}";
			}
		}

		private void SetIpText(string ipAddress)
		{
			if (_ipText != null)
			{
				_ipText.text = $"IP: {ipAddress}";
			}
		}

		private void UpdatePlayerCountDisplay(int count)
		{
			if (_amountPlayersText != null)
			{
				_amountPlayersText.text = $"Players in lobby: {count}";
			}
		}
	}
}