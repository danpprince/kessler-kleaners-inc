using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayRandomSong : MonoBehaviour
{

    public AudioSource source;
    public AudioMixer mixergroup;

    void Start()
    {
        mixergroup.SetFloat("Volume", -20f);
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Music");

        print(clips.Length);
        source.clip = clips[Random.Range(0, clips.Length)];
        source.Play();
    }


    private void Update()
    {
        mixergroup.SetFloat("Pitch", Time.timeScale);
    }

}
