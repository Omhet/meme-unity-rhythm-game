using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceProgressBar: MonoBehaviour
{
    public static DanceProgressBar Instance;

    [SerializeField] private float maximum = 0f;
    private float current = 0f;

    private RectTransform barContainer;
    public RectTransform profilesContainer;

    private Vector2 profileVelocity;
    public float profileSmoothTime = 0.2f;

    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        barContainer = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentFill();
    }

    void UpdateCurrentFill()
    {
        float percent = current / maximum;

        mask.fillAmount = percent;

        float halfProfileWidth = profilesContainer.rect.width / 2;
        float minX = halfProfileWidth;
        float maxX = barContainer.rect.width - halfProfileWidth;

        float targetX = Mathf.Lerp(minX, maxX, percent);

        // Smoothly move the x position
        Vector2 newPosition = new Vector2(targetX, profilesContainer.anchoredPosition.y);
        profilesContainer.anchoredPosition = Vector2.SmoothDamp(profilesContainer.anchoredPosition, newPosition, ref profileVelocity, profileSmoothTime);

    }

    public void SetMaximumValue(float num)
    {
       maximum = num;
       current = maximum / 2;
    }

    public void AddToCurrentValue(float num)
    {
        current += num;
        current = Mathf.Min(Mathf.Max(0, current), maximum);
    }
}
