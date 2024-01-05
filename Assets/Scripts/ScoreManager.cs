using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public AudioSource hitSFX;
    public AudioSource missSFX;

    public TMPro.TextMeshProUGUI comboText;
    public TMPro.TextMeshProUGUI scoreText;

    public float score;
    public float maximumLevelScore;
    public float comboScore;

    void Start()
    {
        Instance = this;
        score = 0;
        comboScore = 0;
    }

    public static void Hit()
    {
        Instance.score++;
        Instance.comboScore++;

        DanceProgressBar.Instance.AddToCurrentValue(1);
        DanceManager.Instance.StabilizePlayerDance();
        SongManager.Instance.RestoreSong();

        Instance.hitSFX.Play();
    }

    public static void Miss()
    {
        Instance.comboScore = 0;

        DanceProgressBar.Instance.AddToCurrentValue(-1);
        DanceManager.Instance.DisruptPlayerDance();
        SongManager.Instance.CoruptSong();

        Instance.missSFX.Play();
    }
}