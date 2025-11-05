using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makesoundonawake : MonoBehaviour
{
    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();

    }

    void PlayAudio()
    {
        audio.Play(0);
        Debug.Log("playsound");
    }

}
