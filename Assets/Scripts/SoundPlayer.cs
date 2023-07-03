using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class SoundPlayer : MonoBehaviour
{
    [System.Serializable]
    public struct SoundClipSpec
    {
        public string name;
        public AudioClip clip;
        public bool loop;
    }

    public SoundClipSpec[] clips;
    public AudioMixerGroup mixerGroup;

    private Dictionary<string, AudioSource> players;
    private Dictionary<string, SoundClipSpec> clipLookup;

    void Awake()
    {
        players = new Dictionary<string, AudioSource>();
        clipLookup = clips.ToDictionary(c => c.name, c => c);

        if(!mixerGroup)
        {
            Debug.LogWarning("No AudioMixer assigned to " + name);
        }
    }

    private AudioSource CreateAudioSource(SoundClipSpec clip)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = clip.clip;
        audioSource.loop = clip.loop;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 10f;
        audioSource.maxDistance = 60f;
        audioSource.spatialBlend = 1;

        players.Add(clip.name, audioSource);
        return audioSource;
    }

    private AudioSource CreateAudioSource(string clipName)
    {
        if(clipLookup.TryGetValue(clipName, out var spec))
            return CreateAudioSource(spec);
        
        return null;
    }

    public void Play(string clipName)
    {
        AudioSource player;
        if(!players.TryGetValue(clipName, out player))
        {
            player = CreateAudioSource(clipName);
        }

        if(player)
        {
            player.Play();
        }
    }

    public void PlayOneShot(string clipName)
    {
        if(players.TryGetValue(clipName, out var player))
        {
            AudioSource.PlayClipAtPoint(player.clip, transform.position, player.volume);
        }
    }

    public void Stop(string clipName)
    {
        if(players.TryGetValue(clipName, out var player))
        {
            player.Stop();
        }
    }
}
