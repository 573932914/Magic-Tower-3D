using UnityEngine;
using UnityEngine.SceneManagement;
public class Initial : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        if (SceneManager.GetActiveScene().name == "游戏加载画面")
        {
            return;
        }
        SceneManager.LoadScene("游戏加载画面");
    }
}
