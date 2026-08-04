using System;

namespace Lightsaber.Network
{
	public interface INetworkRoleManagerReadOnly
	{
		event Action<NetworkRole> OnRoleChanged;
		NetworkRole CurrentRole { get; }
	}
}