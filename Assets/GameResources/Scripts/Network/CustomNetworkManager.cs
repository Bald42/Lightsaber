using Mirror;
using UnityEngine;

namespace Lightsaber.Network
{
	public class CustomNetworkManager : NetworkManager
	{
		// Серверные события
		public event System.Action OnServerConnected;
		public event System.Action OnServerDisconnected;

		// Клиентские события (ДОБАВЛЯЕМ ИХ)
		public event System.Action OnClientConnected;
		public event System.Action OnClientDisconnected;

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

		// Вызывается на стороне КЛИЕНТА при успешном подсоединении к серверу
		public override void OnClientConnect()
		{
			base.OnClientConnect();
			OnClientConnected?.Invoke();
		}

		// Вызывается на стороне КЛИЕНТА при отключении от сервера
		public override void OnClientDisconnect()
		{
			base.OnClientDisconnect();
			OnClientDisconnected?.Invoke();
		}
	}
}