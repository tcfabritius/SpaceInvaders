using UnityEngine;

/// <summary>
/// Manages game audio, including explosion and laser sound effects,
/// and implements a singleton pattern to allow global access.
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Static instance used for the singleton pattern.
    /// </summary>
    public static AudioManager instance;

    /// <summary>
    /// Array of audio clips (e.g., explosion, laser, etc.).
    /// </summary>
    public AudioClip[] soundClips;

    /// <summary>
    /// Reference to the AudioSource component used to play sounds.
    /// </summary>
    private AudioSource audioSource;
    
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the singleton instance.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject); ///< Prevent destruction when loading new scenes
            instance = this;               ///< Set this as the singleton instance
        }
        else
        {
            Destroy(gameObject);           ///< Destroy duplicate instance
        }
    }
    
    /// <summary>
    /// Called before the first frame update.
    /// Initializes the AudioSource reference.
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); ///< Get the AudioSource component
    }

    /// <summary>
    /// Plays the explosion sound effect.
    /// </summary>
    public void ExplodeSound()
    {
        audioSource.PlayOneShot(soundClips[0]); ///< Play explosion audio clip
    }

    /// <summary>
    /// Plays the laser sound effect.
    /// </summary>
    public void LaserSound()
    {
        audioSource.PlayOneShot(soundClips[1]); ///< Play laser audio clip
    }
}