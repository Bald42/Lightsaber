using UnityEngine.SceneManagement;

namespace Lightsaber.Core.Loading
{
    public struct ScenesLoader
    {
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}