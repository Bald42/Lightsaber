using UnityEngine;
using Lightsaber.Core.Loading;
using Zenject;

namespace Lightsaber.Core
{
	public class LoadLobby : MonoBehaviour
	{
		[Header("Target Scene Index or Name")] [SerializeField]
		private int lobbySceneIndex = 1;

		[Inject] private IAppLauncherReadOnly appLauncherReadOnly;

		private void Start()
		{
			if (appLauncherReadOnly != null)
			{
				if (appLauncherReadOnly.IsGameReady)
				{
					OnLoadLobby();
				}
				else
				{
					appLauncherReadOnly.GameReadyEvent += GameReadyHandler;
				}
			}
		}

		private void OnDestroy()
		{
			if (appLauncherReadOnly != null)
			{
				appLauncherReadOnly.GameReadyEvent -= GameReadyHandler;
			}
		}

		private void GameReadyHandler()
		{
			OnLoadLobby();
		}
		
		private void OnLoadLobby()
		{
			ScenesLoader.LoadScene(lobbySceneIndex);
		}
	}
}