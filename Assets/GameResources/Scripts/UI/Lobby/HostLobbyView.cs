using TMPro;
using UnityEngine;
using Zenject;

namespace Lightsaber.Network.UI
{
	public class HostLobbyView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _pinText;
		[SerializeField] private TextMeshProUGUI _amountPlayersText;

		[Inject] private INetworkConnectionControllerReadOnly _connectionController;

		private void OnEnable()
		{
			_connectionController.OnServerStartedEvent += OnServerStartedHandler;
			_connectionController.OnPlayerCountChangedEvent += OnPlayerCountChangedHandler;

			// Первичная вычитка состояния, если хост уже запущен до включения UI
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
			}

			UpdatePlayerCountDisplay(_connectionController.ConnectedPlayersCount);
		}

		private void OnServerStartedHandler(ServerStartedEvent serverStartedEvent)
		{
			SetPinText(serverStartedEvent.PinCode);
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

		private void UpdatePlayerCountDisplay(int count)
		{
			if (_amountPlayersText != null)
			{
				_amountPlayersText.text = $"Игроков в лобби: {count}";
			}
		}
	}
}