using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
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
			// 1. ЕСЛИ НЕТ АКТИВНОГО СОЕДИНЕНИЯ ИЛИ СЕТЬ УЖЕ ОТВАЛИЛАСЬ (Дисконнект)
			if (!NetworkClient.isConnected && !NetworkServer.active)
			{
				Debug.Log("<color=yellow>[ReturnToLobbyUI]</color> Network offline. Returning to Lobby locally...");
				SceneManager.LoadScene(_lobbySceneName);
				return;
			}

			// 2. ЕСЛИ ЭТО ЧИСТЫЙ КЛИЕНТ (Смартфон)
			if (NetworkClient.active && !NetworkServer.active)
			{
				Debug.Log("<color=red>[ReturnToLobbyUI]</color> Client leaving game. Disconnecting...");
				NetworkClient.Disconnect();
				return;
			}

			// 3. ЕСЛИ ЭТО ХОСТ/СЕРВЕР (ПК)
			if (NetworkServer.active)
			{
				if (LobbyPlayerNetwork.LocalInstance != null)
				{
					Debug.Log("<color=green>[ReturnToLobbyUI]</color> Host changing scene for everyone...");
					LobbyPlayerNetwork.LocalInstance.RequestStartGame(_lobbySceneName);
				}
				else
				{
					// На крайний случай, если экземпляр игрока пропал — глушим хост
					NetworkManager.singleton.StopHost();
				}
			}
		}
	}
}