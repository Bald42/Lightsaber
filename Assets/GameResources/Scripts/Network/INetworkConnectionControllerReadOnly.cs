using System;

namespace Lightsaber.Network
{
	public interface INetworkConnectionControllerReadOnly
	{
		bool IsServerStarted { get; }
		bool IsClientConnecting { get; }
		bool IsClientConnected { get; }
		int ConnectedPlayersCount { get; }
		string CurrentPinCode { get; }
		string CurrentIpAddress { get; }
		
		event Action<ServerStartedEvent> OnServerStartedEvent;
		event Action OnClientConnectedEvent;
		event Action OnClientDisconnectedEvent;
		event Action<int> OnPlayerCountChangedEvent;
		event Action<string> OnConnectionFailedEvent;
	}
}