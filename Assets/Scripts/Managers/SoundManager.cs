using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMusic(string name)
    {
        Sound sound = System.Array.Find(musicSounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Music not found: " + name);
            return;
        }

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume;
        musicSource.pitch = sound.pitch;
        musicSource.loop = sound.loop;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound sound = System.Array.Find(sfxSounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("SFX not found: " + name);
            return;
        }

        sfxSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
