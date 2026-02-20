using UnityEngine;
using TMPro;

/// <summary>
/// Attach to the GameOverPanel. Populates final stats when the panel activates.
/// </summary>
public class GameOverScreen : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalKillsText;
    public TextMeshProUGUI finalTimeText;

    void OnEnable()
    {
        if (GameManager.Instance == null) return;

        if (finalScoreText != null)
            finalScoreText.text = $"{GameManager.Instance.score}";

        if (finalKillsText != null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            int kills = 0;
            if (playerObj != null)
            {
                DualitySystem duality = playerObj.GetComponent<DualitySystem>();
                if (duality != null)
                    kills = duality.totalKills;
            }
            finalKillsText.text = $"Enemies Killed: {kills}";
        }

        if (finalTimeText != null)
        {
            float t = GameManager.Instance.gameTime;
            int min = Mathf.FloorToInt(t / 60f);
            int sec = Mathf.FloorToInt(t % 60f);
            finalTimeText.text = $"Time Survived: {min:00}:{sec:00}";
        }
    }

    public void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
    }

    public void OnMainMenuButton()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
