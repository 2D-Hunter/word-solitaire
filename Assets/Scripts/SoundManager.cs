using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    
    [Header("Audio Sources")]
     public AudioSource bgmSource; // Background Music
    public AudioSource sfxSource; // Sound Effects

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> soundEffects; // List of SFX Clips

    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.2f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    public bool isBgmMuted;
    public bool isSfxMuted;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Singleton Pattern - Ensure only one SoundManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object persistent
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
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
        if (bgmSource.clip == bgm && bgmSource.isPlaying) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        bgmSource.clip = bgm;
        bgmSource.loop = loop;
        Debug.Log("________PlayBGM: " + isBgmMuted);
        if (!isBgmMuted)
        {
            bgmSource.Play();
            StartCoroutine(SetInitialVolumeAndFade(fadeDuration));
        }
    }

    private IEnumerator SetInitialVolumeAndFade(float fadeDuration)
    {
        yield return null; // Wait 1 frame to ensure Play() starts properly

        if (isBgmMuted)
        {
            bgmSource.volume = 0;
            yield break;
        }
        bgmSource.volume = 0;
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
    public void PlaySFX(string soundName, float volumeMultiplier = 1f)
    {
        if (!isSfxMuted && soundDictionary.ContainsKey(soundName))
        {
            float finalVolume = Mathf.Clamp01(sfxVolume * volumeMultiplier);
            sfxSource.PlayOneShot(soundDictionary[soundName], finalVolume);
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
    public void SetBgmToggle(Toggle toggle)
    {
        toggle.isOn = !isBgmMuted; // Sync toggle with current mute state
        toggle.onValueChanged.AddListener(delegate { ToggleMuteBGMFromUI(toggle.isOn); });
    }
    private void Update()
    {
        //Debug.Log("Music: " + FBPlayerData.instance.GAME_MUSIC);
        //Debug.Log("Sound: " + FBPlayerData.instance.GAME_SOUND);
        //Debug.Log("isBgmMuted: " + isBgmMuted);
    }
    public void SetMusicUI()
    {
        if(FBPlayerData.instance.GAME_MUSIC)
            isBgmMuted = false;
        else
            isBgmMuted = true;
        bgmSource.mute = isBgmMuted;
    }
    public void SetSFXUI()
    {
        if (FBPlayerData.instance.GAME_SOUND)
            isSfxMuted = false;
        else
            isSfxMuted = true;
        sfxSource.mute = isSfxMuted;
    }

    public void ToggleMuteBGMFromUI(bool isOn)
    {
        //isBgmMuted = !isOn; // If toggle is ON, unmute; if OFF, mute
        //bgmSource.mute = isBgmMuted;
        //SaveAudioSettings();
        isBgmMuted = !isOn;
        bgmSource.mute = isBgmMuted;
        Debug.Log("UISwitcher111: " + isBgmMuted);
        if (!isBgmMuted)
        {
            Debug.Log("UISwitcher222: " + bgmSource.isPlaying);
            bgmSource.volume = bgmVolume;
            // Resume music if not playing
            if (!bgmSource.isPlaying)
            {
                Debug.Log("UISwitcher333: " + bgmSource.clip);
                if (bgmSource.clip != null)
                {
                    
                    bgmSource.Play(); // Resume the last clip
                    bgmSource.volume = bgmVolume;
                    Debug.Log("UISwitcher444: " + bgmSource.volume);
                    Debug.Log("UISwitcher555: " + isBgmMuted);
                }
                else
                {
                    // Optional: handle case where no BGM clip was set yet
                    Debug.LogWarning("No BGM clip assigned. Can't play music.");
                    SoundManager.instance.PlayBGM(SoundManager.instance.bgmSource.clip, true, 5f);
                }
            }
        }
        else
        {
            bgmSource.Pause();
        }

        SaveAudioSettings();
    }
    public bool IsBGMMuted()
    {
        return isBgmMuted;
    }

    public void ToggleMuteSFXFromUI(bool isOn)
    {
        isSfxMuted = !isOn; // If switch is ON, unmute. If OFF, mute.
        sfxSource.mute = isSfxMuted;
        if(isOn)
            sfxSource.volume = sfxVolume;
        SaveAudioSettings();
    }
    public bool IsSFXMuted()
    {
        return isSfxMuted;
    }

    /// <summary>
    /// Toggles mute for sound effects and saves the state.
    /// </summary>
    

    /// <summary>
    /// Saves audio settings using PlayerPrefs.
    /// </summary>
    private void SaveAudioSettings()
    {
        if (GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.GAME_MUSIC = !isBgmMuted;
            FBPlayerData.instance.GAME_SOUND = !isSfxMuted;
            Debug.Log("SaveAudioSettings: "+ FBPlayerData.instance.GAME_MUSIC +"____"+ FBPlayerData.instance.GAME_SOUND);
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetFloat("BGM_Volume", bgmVolume);
            PlayerPrefs.SetFloat("SFX_Volume", sfxVolume);
            PlayerPrefs.SetInt("BGM_Muted", isBgmMuted ? 1 : 0);
            PlayerPrefs.SetInt("SFX_Muted", isSfxMuted ? 1 : 0);
            PlayerPrefs.Save();
        }
            
    }

    /// <summary>
    /// Loads saved audio settings.
    /// </summary>
    private void LoadAudioSettings()
    {
        if (GameUtils.IsFacebookBuild())
        {
            isBgmMuted = !FBPlayerData.instance.GAME_MUSIC;
            isSfxMuted = !FBPlayerData.instance.GAME_SOUND;


        }
        else
        {
            bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", bgmVolume);
            sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", sfxVolume);
            isBgmMuted = PlayerPrefs.GetInt("BGM_Muted", 0) == 1;
            isSfxMuted = PlayerPrefs.GetInt("SFX_Muted", 0) == 1;
        }
        
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
        if (isBgmMuted)
            yield break;

        bgmSource.volume = 0;
        Debug.Log("FadeInBGM: "+ bgmSource.volume);
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


    public void MuteAll()
    {
        isBgmMuted = true;
        isSfxMuted = true;
        if (bgmSource != null) bgmSource.mute = true;
        if (sfxSource != null) sfxSource.mute = true;
    }

    public void SetMusicMute(bool mute)
    {
        isBgmMuted = mute;
        if (bgmSource != null) bgmSource.mute = mute;
    }

    public void SetSFXMute(bool mute)
    {
        isSfxMuted = mute;
        if (sfxSource != null) sfxSource.mute = mute;
    }
}