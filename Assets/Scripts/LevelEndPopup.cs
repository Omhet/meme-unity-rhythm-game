using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndPopup : MonoBehaviour
{
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI ratingText;

    public void OnEnable()
    {
        float percent = ScoreManager.Instance.score / ScoreManager.Instance.maximumLevelScore * 100;
        scoreText.text = $"Accuracy: {Mathf.Floor(percent)}%";

        var ratingTextString = "Did you even play?";

        if (percent > 99)
        {
            ratingTextString = "You are the Dance King";
        }
        else if (percent > 90)
        {
            ratingTextString = "You are breathtaking!";
        }
        else if (percent > 60)
        {
            ratingTextString = "Good Job";
        }
        else if (percent > 20)
        {
            ratingTextString = "You can do better";
        }

        ratingText.text = ratingTextString;
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
