using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // Background Music
    [SerializeField] private AudioSource sfxSource; // Sound Effects

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> soundEffects; // List of SFX Clips

    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private bool isBgmMuted;
    private bool isSfxMuted;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Singleton Pattern - Ensure only one SoundManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object persistent
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        return;
        LoadAudioSettings();
        ApplyVolumeSettings();
        InitializeSoundDictionary();
    }

    /// <summary>
    /// Loads all sounds into the dictionary for quick access.
    /// </summary>
    private void InitializeSoundDictionary()
    {
        foreach (AudioClip clip in soundEffects)
        {
            soundDictionary[clip.name] = clip; // Store clip using its name
        }
    }

    /// <summary>
    /// Plays background music with optional fade-in.
    /// </summary>
    public void PlayBGM(AudioClip bgm, bool loop = true, float fadeDuration = 0f)
    {
        if (bgmSource.clip == bgm) return; // Prevent restarting same music

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        bgmSource.clip = bgm;
        bgmSource.loop = loop;
        bgmSource.Play();

        if (fadeDuration > 0)
            fadeCoroutine = StartCoroutine(FadeInBGM(fadeDuration));
        else
            bgmSource.volume = bgmVolume;
    }

    /// <summary>
    /// Stops background music with optional fade-out.
    /// </summary>
    public void StopBGM(float fadeDuration = 0f)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (fadeDuration > 0)
            fadeCoroutine = StartCoroutine(FadeOutBGM(fadeDuration));
        else
            bgmSource.Stop();
    }

    /// <summary>
    /// Plays a sound effect by name.
    /// </summary>
    public void PlaySFX(string soundName)
    {
        if (!isSfxMuted && soundDictionary.ContainsKey(soundName))
        {
            sfxSource.PlayOneShot(soundDictionary[soundName], sfxVolume);
        }
    }

    /// <summary>
    /// Sets volume for BGM and SFX and saves settings.
    /// </summary>
    public void SetVolume(float bgmVol, float sfxVol)
    {
        bgmVolume = Mathf.Clamp01(bgmVol);
        sfxVolume = Mathf.Clamp01(sfxVol);
        ApplyVolumeSettings();
        SaveAudioSettings();
    }

    /// <summary>
    /// Toggles mute for background music and saves the state.
    /// </summary>
    public void ToggleMuteBGM()
    {
        isBgmMuted = !isBgmMuted;
        bgmSource.mute = isBgmMuted;
        SaveAudioSettings();
    }

    /// <summary>
    /// Toggles mute for sound effects and saves the state.
    /// </summary>
    public void ToggleMuteSFX()
    {
        isSfxMuted = !isSfxMuted;
        SaveAudioSettings();
    }

    /// <summary>
    /// Saves audio settings using PlayerPrefs.
    /// </summary>
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("BGM_Volume", bgmVolume);
        PlayerPrefs.SetFloat("SFX_Volume", sfxVolume);
        PlayerPrefs.SetInt("BGM_Muted", isBgmMuted ? 1 : 0);
        PlayerPrefs.SetInt("SFX_Muted", isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads saved audio settings.
    /// </summary>
    private void LoadAudioSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        isBgmMuted = PlayerPrefs.GetInt("BGM_Muted", 0) == 1;
        isSfxMuted = PlayerPrefs.GetInt("SFX_Muted", 0) == 1;
    }

    /// <summary>
    /// Applies the volume settings to audio sources.
    /// </summary>
    private void ApplyVolumeSettings()
    {
        bgmSource.volume = isBgmMuted ? 0 : bgmVolume;
        sfxSource.volume = isSfxMuted ? 0 : sfxVolume;
    }

    /// <summary>
    /// Fades in the background music.
    /// </summary>
    private IEnumerator FadeInBGM(float duration)
    {
        bgmSource.volume = 0;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            bgmSource.volume = Mathf.Lerp(0, bgmVolume, (Time.time - startTime) / duration);
            yield return null;
        }

        bgmSource.volume = bgmVolume;
    }

    /// <summary>
    /// Fades out the background music and stops it.
    /// </summary>
    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, (Time.time - startTime) / duration);
            yield return null;
        }

        bgmSource.volume = 0;
        bgmSource.Stop();
    }
}