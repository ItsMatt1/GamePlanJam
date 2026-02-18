using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Drives the in-game HUD: health bar, corruption bar, form label, kill count, timer.
/// Wire references in the Inspector.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Health Bar")]
    public Image healthBarFill;

    [Header("Corruption Bar")]
    public Image corruptionBarFill;
    public Color angelCorruptionColor = new Color(0.55f, 0.15f, 0.85f);
    public Color devilCorruptionColor = new Color(1f, 0.35f, 0f);

    [Header("Form Indicator")]
    public TextMeshProUGUI formText;
    public Color angelTextColor = new Color(1f, 1f, 0.75f);
    public Color devilTextColor = new Color(1f, 0.15f, 0.15f);

    [Header("Stats")]
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI timerText;

    [Header("References – drag from scene")]
    public DualitySystem dualitySystem;
    public PlayerHealth playerHealth;

    void Start()
    {
        if (dualitySystem != null)
        {
            dualitySystem.onCorruptionChanged.AddListener(UpdateCorruptionBar);
            dualitySystem.onTransformToDevil.AddListener(OnDevilForm);
            dualitySystem.onTransformToAngel.AddListener(OnAngelForm);
        }

        if (playerHealth != null)
            playerHealth.onHealthChanged.AddListener(UpdateHealthBar);

        // Initialize UI in angel state
        OnAngelForm();
        UpdateHealthBar(1f);
        UpdateCorruptionBar(0f);
    }

    void Update()
    {
        if (dualitySystem != null && killCountText != null)
            killCountText.text = $"Kills: {dualitySystem.totalKills}";

        if (GameManager.Instance != null && timerText != null)
        {
            float t = GameManager.Instance.gameTime;
            int min = Mathf.FloorToInt(t / 60f);
            int sec = Mathf.FloorToInt(t % 60f);
            timerText.text = $"{min:00}:{sec:00}";
        }
    }

    void UpdateHealthBar(float normalized)
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = normalized;
    }

    void UpdateCorruptionBar(float normalized)
    {
        if (corruptionBarFill != null)
            corruptionBarFill.fillAmount = normalized;
    }

    void OnDevilForm()
    {
        if (formText != null)
        {
            formText.text = "DEVIL";
            formText.color = devilTextColor;
        }

        if (corruptionBarFill != null)
            corruptionBarFill.color = devilCorruptionColor;
    }

    void OnAngelForm()
    {
        if (formText != null)
        {
            formText.text = "ANGEL";
            formText.color = angelTextColor;
        }

        if (corruptionBarFill != null)
            corruptionBarFill.color = angelCorruptionColor;
    }
}
