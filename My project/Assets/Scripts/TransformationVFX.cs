using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Screen flash + camera shake on form change.
/// Wire DualitySystem events to TriggerDevilVFX / TriggerAngelVFX.
/// Needs a full-screen UI Image for the flash overlay.
/// </summary>
public class TransformationVFX : MonoBehaviour
{
    [Header("Screen Flash")]
    public Image flashOverlay;
    public Color devilFlashColor = new Color(1f, 0.1f, 0.05f, 0.7f);
    public Color angelFlashColor = new Color(1f, 1f, 0.7f, 0.6f);
    public float flashDuration = 0.4f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 0.25f;

    [Header("Player Pulse")]
    public Transform playerTransform;
    public float pulseScale = 1.4f;
    public float pulseDuration = 0.25f;

    private Camera cam;
    private Vector3 originalCamLocalPos;
    private Coroutine flashRoutine;
    private Coroutine shakeRoutine;
    private Coroutine pulseRoutine;

    void Start()
    {
        cam = Camera.main;
        if (cam != null)
            originalCamLocalPos = cam.transform.localPosition;

        if (flashOverlay != null)
        {
            flashOverlay.color = Color.clear;
            flashOverlay.raycastTarget = false;
        }
    }

    public void TriggerDevilVFX()
    {
        PlayFlash(devilFlashColor);
        PlayShake();
        PlayPulse();
    }

    public void TriggerAngelVFX()
    {
        PlayFlash(angelFlashColor);
        PlayShake();
        PlayPulse();
    }

    void PlayFlash(Color color)
    {
        if (flashOverlay == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    void PlayShake()
    {
        if (cam == null) return;
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    void PlayPulse()
    {
        if (playerTransform == null) return;
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    IEnumerator FlashRoutine(Color color)
    {
        flashOverlay.color = color;
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;
            // Fast flash in, smooth fade out
            float alpha = color.a * (1f - t * t);
            flashOverlay.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        flashOverlay.color = Color.clear;
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = shakeIntensity * (1f - elapsed / shakeDuration);
            Vector2 offset = Random.insideUnitCircle * strength;
            cam.transform.localPosition = originalCamLocalPos + (Vector3)offset;
            yield return null;
        }

        cam.transform.localPosition = originalCamLocalPos;
    }

    IEnumerator PulseRoutine()
    {
        Vector3 originalScale = playerTransform.localScale;
        Vector3 targetScale = originalScale * pulseScale;
        float elapsed = 0f;
        float half = pulseDuration * 0.3f; // fast expand, slower shrink

        // Expand
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            playerTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / half);
            yield return null;
        }

        // Shrink back
        elapsed = 0f;
        float shrinkTime = pulseDuration - half;
        while (elapsed < shrinkTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkTime;
            t = 1f - (1f - t) * (1f - t); // ease out
            playerTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        playerTransform.localScale = originalScale;
    }
}
