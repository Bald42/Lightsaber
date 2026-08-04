using UnityEngine;
using Mirror;
using Lightsaber.Core.Loading;

namespace Lightsaber.Core
{
	public class AppInitializer : MonoBehaviour
	{
		[Header("Target Scene Index or Name")]
		[SerializeField] private int lobbySceneIndex = 1;

		private void Start()
		{
			// Проверяем, что NetworkManager точно проинициализировался
			if (NetworkManager.singleton != null)
			{
				Debug.Log("[AppInitializer] NetworkManager initialized. Switching to Lobby...");
                
				// Переходим в Лобби через наш ScenesLoader
				ScenesLoader.LoadScene(lobbySceneIndex);
			}
			else
			{
				Debug.LogError("[AppInitializer] Critical Error: NetworkManager singleton is missing on Init scene!");
			}
		}
	}
}