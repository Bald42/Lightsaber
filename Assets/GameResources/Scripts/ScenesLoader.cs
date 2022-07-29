using UnityEngine.SceneManagement;

namespace Loader
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