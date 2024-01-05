using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePopup : MonoBehaviour
{
    void OnEnable()
    {
        SongManager.Instance.Pause();
    }

    void OnDisable()
    {
        SongManager.Instance.Resume();
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void Resume()
    {
        PopupsManager.Instance.ClosePausePopup();
    }
}
