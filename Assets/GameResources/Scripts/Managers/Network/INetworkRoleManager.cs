namespace Lightsaber.Network
{
	public interface INetworkRoleManager : INetworkRoleManagerReadOnly
	{
		void SetRole(NetworkRole newRole);
		void ToggleRole();
	}
}