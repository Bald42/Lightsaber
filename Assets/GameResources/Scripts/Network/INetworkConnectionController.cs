namespace Lightsaber.Network
{
	public interface INetworkConnectionController : INetworkConnectionControllerReadOnly
	{
		void StartHost();
		void ConnectByPin(string pin);
		void ConnectByPinWithSubnet(string pin, string subnetPrefix);
		void Disconnect();
		void OnPlayerConnected();
		void OnPlayerDisconnected();
	}
}