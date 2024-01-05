using UnityEngine;

public class PopupsManager : MonoBehaviour
{
    public static PopupsManager Instance;

    public GameObject popupsCanvas;
    public GameObject pausePopup;
    public GameObject levelEndPopup;

    void Start()
    {
        Instance = this;
    }

    public void OpenPopup(GameObject popup)
    {
        popupsCanvas.SetActive(true);
        popup.SetActive(true);
    }

    public void ClosePopup(GameObject popup)
    {
        popup.SetActive(false);
        popupsCanvas.SetActive(false);
    }

    public void OpenLevelEndPopup()
    {
        OpenPopup(levelEndPopup);
    }

    public void CloseLevelEndPopup()
    {
        ClosePopup(levelEndPopup);
    }

    public void OpenPausePopup()
    {
        OpenPopup(pausePopup);
    }

    public void ClosePausePopup()
    {
        ClosePopup(pausePopup);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePopup.active)
            {
                ClosePausePopup();
            }
            else
            {
                OpenPausePopup();
            }
        }
    }
}
