using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource introSource;
    public AudioSource loopSource;
    public bool changingVolume = false;
    void Start(){  
        introSource.Play();
        loopSource.PlayDelayed(introSource.clip.length);
    }

    public IEnumerator lowerVolume(){
        changingVolume = true;
        while (introSource.volume > .25f) {
            introSource.volume -= 1 * Time.deltaTime / 1f;
            loopSource.volume -= 1 * Time.deltaTime / 1f;
            yield return null;
        }
        changingVolume = false;
    }
    public IEnumerator raiseVolume(){
        changingVolume = true;
        while (introSource.volume < 1f) {
            introSource.volume += 1 * Time.deltaTime / 1f;
            loopSource.volume += 1 * Time.deltaTime / 1f;
            yield return null;
        }
        changingVolume = false;
    }
}
