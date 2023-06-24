using System.Collections.Generic;
using UnityEngine;
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
            audioSource.playOnAwake = false;
            audioSource.clip = clip.clip;
            audioSource.loop = clip.loop;
            audioSource.outputAudioMixerGroup = mixerGroup;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 10f;
            audioSource.maxDistance = 60f;
            audioSource.spatialBlend = 1;

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
