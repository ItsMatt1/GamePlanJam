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

    [Header("Camera Zoom Punch")]
    public float zoomPunchDuration = 0.3f;
    public float zoomPunchAmount = 1.2f;

    [Header("Player Pulse")]
    public Transform playerTransform;
    public float pulseScale = 1.4f;
    public float pulseDuration = 0.25f;

    private Camera cam;
    private float originalOrthoSize;
    private Coroutine flashRoutine;
    private Coroutine zoomRoutine;
    private Coroutine pulseRoutine;

    void Start()
    {
        cam = Camera.main;
        if (cam != null)
            originalOrthoSize = cam.orthographicSize;

        if (flashOverlay != null)
        {
            flashOverlay.color = Color.clear;
            flashOverlay.raycastTarget = false;
        }
    }

    public void TriggerDevilVFX()
    {
        PlayFlash(devilFlashColor);
        PlayZoomPunch();
        PlayPulse();
    }

    public void TriggerAngelVFX()
    {
        PlayFlash(angelFlashColor);
        PlayZoomPunch();
        PlayPulse();
    }

    void PlayFlash(Color color)
    {
        if (flashOverlay == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    void PlayZoomPunch()
    {
        if (cam == null) return;
        if (zoomRoutine != null) StopCoroutine(zoomRoutine);
        zoomRoutine = StartCoroutine(ZoomPunchRoutine());
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

    IEnumerator ZoomPunchRoutine()
    {
        float zoomedSize = originalOrthoSize / zoomPunchAmount;
        float elapsed = 0f;
        float half = zoomPunchDuration * 0.3f;

        // Zoom in fast
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(originalOrthoSize, zoomedSize, elapsed / half);
            yield return null;
        }

        // Zoom back out smooth
        elapsed = 0f;
        float returnTime = zoomPunchDuration - half;
        while (elapsed < returnTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnTime;
            t = 1f - (1f - t) * (1f - t);
            cam.orthographicSize = Mathf.Lerp(zoomedSize, originalOrthoSize, t);
            yield return null;
        }

        cam.orthographicSize = originalOrthoSize;
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
