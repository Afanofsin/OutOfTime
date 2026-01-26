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