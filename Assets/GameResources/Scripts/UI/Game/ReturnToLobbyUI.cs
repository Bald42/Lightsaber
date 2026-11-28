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
			// Хост сам сервер: своего игрока в GameScene у него нет, сцену меняет напрямую.
			if (NetworkServer.active)
			{
				NetworkManager.singleton.ServerChangeScene(_lobbySceneName);
				return;
			}

			// На телефоне просьба идёт через SaberPlayerNetwork: лобби-игрок при смене сцены уничтожен.
			if (NetworkClient.localPlayer != null
				&& NetworkClient.localPlayer.TryGetComponent(out SaberPlayerNetwork saberPlayer))
			{
				saberPlayer.RequestReturnToLobby(_lobbySceneName);
			}
		}
	}
}