using System;
using Mirror;

namespace Lightsaber.Network
{
	public class CustomNetworkManager : NetworkManager
	{
		public event Action OnServerConnected;
		public event Action OnServerDisconnected;

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
	}
}