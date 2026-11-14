using System.Collections.Generic;
using Mirror;
using Lightsaber.Network;
using UnityEngine;

namespace Lightsaber.Game
{
	public class GameManager : MonoBehaviour
	{
		[Header("Prefabs & Points")]
		[SerializeField] private GameObject gamePlayerPrefab;
		[SerializeField] private Transform[] spawnPoints;

		private readonly HashSet<int> _spawnedConnections = new HashSet<int>();

		private void OnEnable()
		{
			CustomNetworkManager.OnServerReadyEvent += HandleClientReady;
		}

		private void OnDisable()
		{
			CustomNetworkManager.OnServerReadyEvent -= HandleClientReady;
		}

		private void Start()
		{
			if (NetworkServer.active)
			{
				foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
				{
					// Проверяем только готовых клиентов
					if (conn != null && conn.isReady)
					{
						SpawnPlayerForConnection(conn);
					}
				}
			}
		}

		private void HandleClientReady(NetworkConnectionToClient conn)
		{
			if (!NetworkServer.active) return;
			
			SpawnPlayerForConnection(conn);
		}

		[Server]
		private void SpawnPlayerForConnection(NetworkConnectionToClient conn)
		{
			if (conn == null) return;

			// КЛЮЧЕВАЯ ПРОВЕРКА: Если это сокет самого Хоста (сервера) — ИГНОРИРУЕМ!
			// На Хосте меч спавнить не нужно, он только отображает игру.
			if (conn == NetworkServer.localConnection)
			{
				Debug.Log("<color=yellow>[GameManager]</color> Skipped spawning GamePlayer for Host local connection.");
				return;
			}

			// Если для этого клиента меч уже заспавнен — пропускаем
			if (_spawnedConnections.Contains(conn.connectionId)) return;

			if (gamePlayerPrefab == null)
			{
				Debug.LogError("[GameManager] GamePlayerPrefab is not assigned!");
				return;
			}

			_spawnedConnections.Add(conn.connectionId);

			// Выбираем точку спавна по индексу
			int spawnIndex = _spawnedConnections.Count - 1;
			Transform point = (spawnPoints != null && spawnPoints.Length > spawnIndex)
				? spawnPoints[spawnIndex]
				: transform;

			Vector3 pos = point != null ? point.position : Vector3.zero;
			Quaternion rot = point != null ? point.rotation : Quaternion.identity;

			GameObject playerInstance = Instantiate(gamePlayerPrefab, pos, rot);

			// Спавним объект в сети
			NetworkServer.Spawn(playerInstance);

			// Выдаем Authority конкретному подключенному клиенту (смартфону)
			playerInstance.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);

			Debug.Log($"<color=green>[GameManager]</color> Successfully spawned GamePlayer for Client connId: {conn.connectionId}");
		}
	}
}