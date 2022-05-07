using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Audio : MonoBehaviour
{
    public AudioSource source;
    public List<AudioClip> clips = new List<AudioClip>();
    public int currentIndex = 0;
    public string absolutePath = "./";

    void Start()
    {

        absolutePath = Application.dataPath + "/Sound/";

        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        PlayCurrent(currentIndex);
    }

    void PlayCurrent(int position)
    {
        source.clip = clips[position];
        source.Play();
    }
}