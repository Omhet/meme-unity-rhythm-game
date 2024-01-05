using UnityEngine;

public class Note : MonoBehaviour
{
    public float assignedTime;
    public bool isPlayerNote;

    private SpriteRenderer spriteRenderer;
    private RectTransform rectTransform;
    private Material material;

    private float fadeInDuration;
    private float fadeInTimer = 0f;

    float dissolveFade = 1f;
    private bool isDissolving = false;

    private float fadeOutTimer = 1f;
    private bool isFadingOut = false;


    [ColorUsage(true, true)]
    public Color color = Color.white;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rectTransform = GetComponent<RectTransform>();
        material = spriteRenderer.material;

        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color); // Replace "_Color" with your shader's color property name
        spriteRenderer.SetPropertyBlock(propBlock);

        // Fade In
        // Initialize with transparent
        Color initialColor = spriteRenderer.color;
        initialColor.a = 0f;
        spriteRenderer.color = initialColor;

        fadeInDuration = SongManager.Instance.noteTime * 0.6f;
    }

    public void Dissolve()
    {
        isDissolving = true;
    }

    public void FadeOut()
    {
        isFadingOut = true;
    }

    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - assignedTime + SongManager.Instance.noteTime;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));


        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            spriteRenderer.enabled = true;

            // Dissolving
            if (isDissolving)
            {
                dissolveFade -= Time.deltaTime * 5;
                material.SetFloat("_Fade", dissolveFade);

                if (dissolveFade == 0)
                {
                    isDissolving = false;
                    Destroy(gameObject);
                }
            }

            if (isFadingOut)
            {
                fadeOutTimer -= Time.deltaTime * 4f;
                Color color = spriteRenderer.color;
                color.a = fadeOutTimer;
                spriteRenderer.color = color;
            }

            // Canvas movement
            if (!isDissolving)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(Vector2.up * SongManager.Instance.noteSpawnY, Vector2.up * SongManager.Instance.noteDespawnY, t); ;
            }

            // Fade-in
            if (fadeInTimer < fadeInDuration)
            {
                fadeInTimer += Time.deltaTime;
                float alpha = Mathf.Clamp01(fadeInTimer / fadeInDuration); // Normalize alpha between 0 and 1
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }
    }
}