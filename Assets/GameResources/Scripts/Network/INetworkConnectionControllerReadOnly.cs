using System;

namespace Lightsaber.Network
{
	public interface INetworkConnectionControllerReadOnly
	{
		bool IsConnecting { get; }
		int ConnectedPlayersCount { get; }
		bool IsServerStarted { get; }
		string CurrentPinCode { get; }
		
		event Action<ServerStartedEvent> OnServerStartedEvent;
		event Action OnClientConnectedEvent;
		event Action OnClientDisconnectedEvent;
		event Action<int> OnPlayerCountChangedEvent;
		event Action<string> OnConnectionFailedEvent;
	}
}