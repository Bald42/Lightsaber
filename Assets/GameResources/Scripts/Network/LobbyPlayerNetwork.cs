using Mirror;
using UnityEngine;

namespace Lightsaber.Network
{
	public class LobbyPlayerNetwork : NetworkBehaviour
	{
		public static LobbyPlayerNetwork LocalInstance { get; private set; }

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		public override void OnStartLocalPlayer()
		{
			base.OnStartLocalPlayer();
			LocalInstance = this;
		}

		public void RequestStartGame(string sceneName)
		{
			if (isLocalPlayer)
			{
				CmdRequestStartGame(sceneName);
			}
		}

		[Command]
		private void CmdRequestStartGame(string sceneName)
		{
			Debug.Log($"<color=green>[Server]</color> Received StartGame request. Changing scene to: {sceneName}");
			NetworkManager.singleton.ServerChangeScene(sceneName);
		}
	}
}