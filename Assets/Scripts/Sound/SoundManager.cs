using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class SoundManager : SerializedMonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [DictionaryDrawerSettings(KeyLabel = "Sound ID", ValueLabel = "Variants")]
    [OdinSerialize] private Dictionary<SoundId, List<Sound>> _soundLibrary;

    [SerializeField] private int pooledSources = 10;
    private AudioSource[] _sources;
    private int _sourceIndex;
    
    [Header("Music (Quick & Dirty)")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip firstLevelMusic;
    [SerializeField] private float musicFadeTime = 0.5f;

    private AudioSource _musicSource;
    private Coroutine _musicFadeRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sources = new AudioSource[pooledSources];
        for (int i = 0; i < pooledSources; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _sources[i] = source;
        }
        
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.spatialBlend = 0f;
        _musicSource.volume = 0.005f;
    }

    private AudioSource GetSource()
    {
        var source = _sources[_sourceIndex];
        _sourceIndex = (_sourceIndex + 1) % _sources.Length;
        return source;
    }

    private Sound GetRandomVariant(SoundId id)
    {
        if (!_soundLibrary.TryGetValue(id, out var variants) || variants == null || variants.Count == 0)
            return null;

        return variants[Random.Range(0, variants.Count)];
    }

    public void Play(SoundId id)
    {
        var sound = GetRandomVariant(id);
        if (sound == null || sound.clip == null) return;

        var source = GetSource();
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.spatialBlend = 0f;
        source.Play();
    }

    public void PlayAt(SoundId id, Vector3 position)
    {
        var sound = GetRandomVariant(id);
        if (sound == null || sound.clip == null) return;

        var source = GetSource();
        source.transform.position = position;
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.spatialBlend = 1f;
        source.Play();
    }
    
    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
    }

    public void PlayFirstLevelMusic()
    {
        PlayMusic(firstLevelMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (_musicSource.clip == clip)
            return;

        if (_musicFadeRoutine != null)
            StopCoroutine(_musicFadeRoutine);

        _musicFadeRoutine = StartCoroutine(FadeToMusic(clip));
    }

    private IEnumerator FadeToMusic(AudioClip newClip)
    {
        float startVolume = _musicSource.volume;
        
        for (float t = 0; t < musicFadeTime; t += Time.deltaTime)
        {
            _musicSource.volume = Mathf.Lerp(startVolume, 0f, t / musicFadeTime);
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.Play();
        
        for (float t = 0; t < musicFadeTime; t += Time.deltaTime)
        {
            _musicSource.volume = Mathf.Lerp(0f, 0.005f, t / musicFadeTime);
            yield return null;
        }

        _musicSource.volume = 0.005f;
        _musicFadeRoutine = null;
    }
    
}



public enum SoundId
{
    SwordAttack,
    Shoot,
    PlayerHit,
    EnemyHit,
    Dash,
    Death,
    Skill
}