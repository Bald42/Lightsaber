using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Lightsaber.Network.UI
{
	public class ReturnToLobbyUI : MonoBehaviour
	{
		[SerializeField] private Button _returnButton;
		[SerializeField] private string _lobbySceneName = "Lobby";

		[Inject] private INetworkConnectionController _connectionController;

		private void OnEnable()
		{
			if (_returnButton != null)
			{
				_returnButton.onClick.AddListener(OnReturnButtonClicked);
			}
		}

		private void OnDisable()
		{
			if (_returnButton != null)
			{
				_returnButton.onClick.RemoveListener(OnReturnButtonClicked);
			}
		}

		private void OnReturnButtonClicked()
		{
			if (LobbyPlayerNetwork.LocalInstance != null)
			{
				LobbyPlayerNetwork.LocalInstance.RequestStartGame(_lobbySceneName);
			}
		}
	}
}