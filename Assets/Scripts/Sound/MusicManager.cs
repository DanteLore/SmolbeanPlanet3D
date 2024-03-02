using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private SoundPlayer soundPlayer;
    private string currentTrack;

    void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
        currentTrack = RandomTrackName();

        StartCoroutine(PlayMusic());
    }

    private IEnumerator PlayMusic()
    {
        while(true)
        {
            if(!soundPlayer.IsPlaying(currentTrack))
            {
                currentTrack = RandomTrackName();
                soundPlayer.Play(currentTrack);
            }

            yield return new WaitForSeconds(8f);
        }
    }

    private string RandomTrackName()
    {
        return soundPlayer.clips[UnityEngine.Random.Range(0, soundPlayer.clips.Length)].name;
    }
}
