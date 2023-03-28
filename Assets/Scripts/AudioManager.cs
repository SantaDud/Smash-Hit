using System;
using UnityEngine;
// using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;
    public static AudioManager Instance;

    void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            DestroyImmediate(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    void Start()
    {
        
    }

    public void Play(string name)
    {
        Sound sound = Array.Find<Sound>(sounds, s => s.name == name);
        
        if (sound == null)
        {
            Debug.LogWarning($"No sound named {name}.");
            return;
        }

        sound.source.Play();
    }
}

[Serializable]
class Sound
{
    public AudioClip clip;
    public AudioSource source;

    public string name;
    
    [Range(0f, 1f)]
    public float volume;
    
    [Range(-3f, 3f)]
    public float pitch;

    public bool loop;
    public bool playOnAwake;
}