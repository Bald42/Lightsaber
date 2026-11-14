using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lightsaber.Network
{
	public class CustomNetworkManager : NetworkManager
	{
		[Header("Custom Prefabs")]
		[SerializeField] private GameObject lobbyPlayerPrefab;

		[Header("Scene Settings")]
		[Scene] [SerializeField] private string lobbySceneName = "Lobby";

		// --- СЕРВЕРНЫЕ СОБЫТИЯ ---
		public event Action OnServerConnected;
		public event Action OnServerDisconnected;

		// --- КЛИЕНТСКИЕ СОБЫТИЯ ---
		public event Action OnClientConnected;
		public event Action OnClientDisconnected;

		// --- СОБЫТИЯ СЦЕНЫ И ГОТОВНОСТИ ---
		public static event Action<string> OnServerSceneChangedEvent;
		public static event Action<NetworkConnectionToClient> OnServerReadyEvent;

		public override void OnServerConnect(NetworkConnectionToClient conn)
		{
			base.OnServerConnect(conn);
			OnServerConnected?.Invoke();
		}

		public override void OnServerDisconnect(NetworkConnectionToClient conn)
		{
			base.OnServerDisconnect(conn);
			OnServerDisconnected?.Invoke();
		}

		public override void OnClientConnect()
		{
			base.OnClientConnect();

			if (!NetworkClient.ready)
			{
				NetworkClient.Ready();
			}

			NetworkClient.AddPlayer();
			OnClientConnected?.Invoke();
		}

		// ВЫЗЫВАЕТСЯ НА КЛИЕНТЕ, КОГДА ХОСТ УПАЛ ИЛИ СЕРВЕР ОТКЛЮЧИЛСЯ
		public override void OnClientDisconnect()
		{
			base.OnClientDisconnect();
			OnClientDisconnected?.Invoke();

			Debug.LogWarning("<color=yellow>[CustomNetworkManager]</color> Host disconnected or network lost. Returning to Lobby...");

			// Если мы не на сцене Лобби — принудительно грузим локальную сцену Лобби
			if (SceneManager.GetActiveScene().name != lobbySceneName)
			{
				SceneManager.LoadScene(lobbySceneName);
			}
		}

		public override void OnServerAddPlayer(NetworkConnectionToClient conn)
		{
			if (lobbyPlayerPrefab != null)
			{
				GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab);
				DontDestroyOnLoad(lobbyPlayer);
				NetworkServer.AddPlayerForConnection(conn, lobbyPlayer);
				Debug.Log($"<color=cyan>[CustomNetworkManager]</color> Spawned persistent LobbyPlayer for connId: {conn.connectionId}");
			}
			else
			{
				base.OnServerAddPlayer(conn);
			}
		}

		public override void OnServerSceneChanged(string sceneName)
		{
			base.OnServerSceneChanged(sceneName);
			OnServerSceneChangedEvent?.Invoke(sceneName);
		}

		public override void OnServerReady(NetworkConnectionToClient conn)
		{
			base.OnServerReady(conn);
			Debug.Log($"<color=green>[CustomNetworkManager]</color> Client connId {conn.connectionId} is now READY!");
			OnServerReadyEvent?.Invoke(conn);
		}
	}
}