using UnityEngine;

/// <summary>
/// Loops a music track during gameplay. Attach to an empty GameObject in the scene.
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    [Header("Music")]
    public AudioClip musicClip;
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (musicClip != null)
            audioSource.Play();
    }
}
