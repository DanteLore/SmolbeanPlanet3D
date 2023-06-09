using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

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

    void Awake()
    {
        players = new Dictionary<string, AudioSource>();

        if(!mixerGroup)
        {
            Debug.LogWarning("No AudioMixer assigned to " + name);
        }

        foreach(var clip in clips)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip.clip;
            audioSource.loop = clip.loop;
            audioSource.outputAudioMixerGroup = mixerGroup;

            players.Add(clip.name, audioSource);
        }
    }   

    public void Play(string clipName)
    {
        if(players.TryGetValue(clipName, out var player))
        {
            player.Play();
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
