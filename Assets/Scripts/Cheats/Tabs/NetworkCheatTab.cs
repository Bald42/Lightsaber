using Lightsaber.Network;
using UnityEngine;
using Zenject;

namespace Lightsaber.Cheats
{
	public class NetworkCheatTab : CheatTab
	{
		protected override string Name => "Network Cheats";
		[Inject] private readonly INetworkRoleManager _networkRoleManager;
		
		public override void Initialize()
		{
			AddButton("Change role", () => ChangeRole());
		}

		private void ChangeRole()
		{
			_networkRoleManager
				.SetRole(_networkRoleManager.CurrentRole == NetworkRole.Host ? 
					NetworkRole.Client : NetworkRole.Host); 
		}
	}
}