namespace Lightsaber.Network
{
	public interface INetworkConnectionController : INetworkConnectionControllerReadOnly
	{
		void StartHost();
		void ConnectByPin(string pin);
		void Disconnect();
		void OnPlayerConnected();
		void OnPlayerDisconnected();
	}
}