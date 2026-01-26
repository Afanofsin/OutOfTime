using UnityEngine;

[System.Serializable]
public class Sound
{
    public SoundId id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;
}