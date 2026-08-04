using System;
using UnityEngine;

namespace Lightsaber.Network
{
	public partial class NetworkRoleManager
	{
		[Serializable]
		public class Settings
		{
			[SerializeField] private bool useTestOverride = false;
			[SerializeField] private NetworkRole testRole = NetworkRole.Host;
			[SerializeField] private bool enableCheatHotkeys = true;
			[SerializeField] private KeyCode hostCheatKey = KeyCode.F1;
			[SerializeField] private KeyCode clientCheatKey = KeyCode.F2;

			public bool UseTestOverride => useTestOverride;
			public NetworkRole TestRole => testRole;
			public bool EnableCheatHotkeys => enableCheatHotkeys;
			public KeyCode HostCheatKey => hostCheatKey;
			public KeyCode ClientCheatKey => clientCheatKey;
		}
	}
}