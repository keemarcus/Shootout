using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    // audio player
    public AudioSource audioSource;
    public AudioClip gunshot;
    public bool playAudio = false;
    private bool soundPlayed = false;

    // Update is called once per frame
    void Update()
    {
        if(playAudio && !soundPlayed){Debug.Log("sound"); audioSource.Play(); soundPlayed = true;}
        else if(!playAudio && soundPlayed){soundPlayed = false;}
    }
}
