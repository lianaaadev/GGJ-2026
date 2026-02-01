using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;

    // audio file path
    // BGM
    public const string BGM_FIRSTLEVEL = "Audios/FirstLevelMusic";
    public const string BGM_CITY = "Audios/CityLevelMusic";
    public const string BGM_FINALLEVEL = "Audios/FinalLevelMusic";
    // SE
    public const string SE_CLICK = "Audios/ClickyClick";
    public const string SE_JUMP = "Audios/JumpSound";
    public const string SE_LANDING = "Audios/LandingSound";
    public const string SE_FAIL = "Audios/FailStateSound";
    public const string SE_SUCCESS = "Audios/SuccessSound";

    // Cache loaded clips (avoid repeated Resources.Load)
    Dictionary<string, AudioClip> clipCache = new();

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Initial volume
        SetBGMVolume(0.5f);
        SetSEVolume(0.5f);
        PlayBGM(BGM_CITY);
    }

    // Public API
    public void PlayBGM(string path, bool loop = true)
    {
        AudioClip clip = GetClip(path);
        if (clip == null) return;

        if (bgmSource.clip == clip) return; // already playing

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PlaySE(string path)
    {
        AudioClip clip = GetClip(path);
        if (clip == null) return;

        seSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSEVolume(float volume)
    {
        seSource.volume = Mathf.Clamp01(volume);
    }

    // =========================
    // Internal
    // =========================

    AudioClip GetClip(string path)
    {
        if (clipCache.TryGetValue(path, out var clip))
            return clip;

        clip = Resources.Load<AudioClip>(path);

        if (clip == null)
        {
            Debug.LogWarning($"Audio not found at path: {path}");
            return null;
        }

        clipCache[path] = clip;
        return clip;
    }
}
