using UnityEngine;
using UnityEngine.Audio;

namespace PrototypeTwo
{

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;
        public AudioSource sfxSource;
        public AudioSource bgmSource;

        public AudioClip gameOverClip;
        public AudioClip victoryClip;
        public AudioClip bgmClip;

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

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
            }

            Debug.Log("SoundManager initialized.");
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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

        public void PlayGameOverSound() => PlaySoundAtPoint(gameOverClip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
        public void PlayVictorySound() => PlaySoundAtPoint(victoryClip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);

        public void PlayBGM()
        {
            if (bgmClip == null)
            {
                Debug.LogWarning("No BGM Clip Asigned.");
                return;
            }

            if (!bgmSource.isPlaying) return;
            {
                bgmSource.clip = bgmClip;
                bgmSource.volume = defaultVolume;
                bgmSource.Play();
            }
        }


        public void StopBGM()
        {
            if (bgmSource.isPlaying)
                bgmSource.Stop();
        }
    }
}

