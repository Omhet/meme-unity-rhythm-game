using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndPopup : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
