using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject quotePanel;

    [Header("Quote")]
    public TextMeshProUGUI quoteText;
    public string quote = "If something is bound to be good\nit's also bound to be bad";
    public float fadeInDuration = 1.5f;
    public float holdDuration = 2.5f;
    public float fadeOutDuration = 1f;

    [Header("Screen Fade")]
    public Image screenFade; // full-screen black Image for transitions

    public void PlayGame()
    {
        StartCoroutine(PlaySequence());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator PlaySequence()
    {
        // Fade menu out to black
        yield return StartCoroutine(Fade(screenFade, 0f, 1f, 0.5f));

        // Hide menu, show quote
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (quotePanel != null)
            quotePanel.SetActive(true);

        // Set quote text invisible
        if (quoteText != null)
        {
            quoteText.text = quote;
            quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, 0f);
        }

        // Fade black out to reveal quote
        yield return StartCoroutine(Fade(screenFade, 1f, 0f, 0.5f));

        // Fade quote text in
        yield return StartCoroutine(FadeText(quoteText, 0f, 1f, fadeInDuration));

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade everything to black
        yield return StartCoroutine(Fade(screenFade, 0f, 1f, fadeOutDuration));

        // Load gameplay
        SceneManager.LoadScene("MainGameplay");
    }

    IEnumerator Fade(Image image, float from, float to, float duration)
    {
        if (image == null) yield break;

        float elapsed = 0f;
        Color c = image.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            image.color = c;
            yield return null;
        }

        c.a = to;
        image.color = c;
    }

    IEnumerator FadeText(TextMeshProUGUI text, float from, float to, float duration)
    {
        if (text == null) yield break;

        float elapsed = 0f;
        Color c = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            text.color = c;
            yield return null;
        }

        c.a = to;
        text.color = c;
    }
}
