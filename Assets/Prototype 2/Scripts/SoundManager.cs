using UnityEngine;
using UnityEngine.Audio;

namespace PrototypeTwo
{

    public class SoundManager : MonoBehaviour
    {
        public SoundManager instance;
        public AudioSource audioSource;

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
            if (bgmSource.isPlaying) return;
            bgmSource.Play();
        }
    }
}

