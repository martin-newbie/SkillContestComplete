using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    public List<AudioClip> clips = new List<AudioClip>();
    public Dictionary<string, AudioClip> ClipDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        foreach (var item in clips)
        {
            ClipDict.Add(item.name, item);
        }
    }

    public void PlaySoundSurround(string key, bool loop, Vector3 pos)
    {
        GameObject obj = new GameObject(key);
        AudioSource source = obj.AddComponent<AudioSource>();

        source.clip = ClipDict[key];
        source.loop = loop;
        source.spatialBlend = 1f;
        source.minDistance = 10f;
        source.Play();

        if (!loop) Destroy(obj, source.clip.length);
    }

    public void PlaySound(string key, bool loop = false)
    {
        GameObject obj = new GameObject(key);
        AudioSource source = obj.AddComponent<AudioSource>();

        source.clip = ClipDict[key];
        source.loop = loop;
        source.Play();

        if (!loop) Destroy(obj, source.clip.length);
    }
}
