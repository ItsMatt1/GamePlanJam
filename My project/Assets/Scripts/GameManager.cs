using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Runtime")]
    public bool isGameOver = false;
    public float gameTime = 0f;

    [Header("Score")]
    public int score = 0;
    public int scorePerKill = 100;
    public int scorePerGem = 25;
    public float devilScoreMultiplier = 3f;

    [Header("UI Panels")]
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isGameOver)
            gameTime += Time.deltaTime;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void AddKillScore()
    {
        float multiplier = GetScoreMultiplier();
        score += Mathf.RoundToInt(scorePerKill * multiplier);
    }

    public void AddGemScore()
    {
        float multiplier = GetScoreMultiplier();
        score += Mathf.RoundToInt(scorePerGem * multiplier);
    }

    float GetScoreMultiplier()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            DualitySystem duality = playerObj.GetComponent<DualitySystem>();
            if (duality != null && duality.currentForm == PlayerForm.Devil)
                return devilScoreMultiplier;
        }
        return 1f;
    }
}
