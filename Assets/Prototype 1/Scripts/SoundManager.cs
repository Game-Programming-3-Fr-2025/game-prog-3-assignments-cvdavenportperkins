using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;
    public AudioClip accessGrantedClip;
    public AudioClip accessDeniedClip;
    public AudioClip infectionClip;
    public AudioClip gameOverClip;
    public AudioClip victoryClip;

    public AudioClip bgmClip;
    private AudioSource bgmSource;

    public float defaultVolume = 1f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        Debug.Log("SoundManager initialized.");

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = defaultVolume;
    }

    public void PlaySoundAtPoint(AudioClip clip, Vector3 position, float volume = -1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("Attempted to play a sound, but the AudioClip is null.");
            return;
        }
        AudioSource.PlayClipAtPoint(clip, position, volume < 0 ? defaultVolume : volume);
    }

    public void PlayAccessGrantedSound(Vector3 position) => PlaySoundAtPoint(accessGrantedClip, position);
    public void PlayAccessDeniedSound(Vector3 position) => PlaySoundAtPoint(accessDeniedClip, position);
    public void PlayInfectionSound(Vector3 position) => PlaySoundAtPoint(infectionClip, position);
    public void PlayGameOverSound() => PlaySoundAtPoint(gameOverClip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
    public void PlayVictorySound() => PlaySoundAtPoint(victoryClip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);

    public void PlayBGM()
    {
        if (bgmSource.isPlaying) return;
        bgmSource.Play();
    }
}