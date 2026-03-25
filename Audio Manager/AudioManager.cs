using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject SFXPrefab;

    [Header("Audio Clips")]
    public Sound[] music;
    public Sound[] bgAmbience;
    public Sound[] sfx;
    private Sound[] allSounds;

    [Header("Sources")]
    private AudioSource[] audios;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        allSounds = sfx.Concat(music).Concat(bgAmbience).ToArray();

        //create an AudioSource for each stored audio clip
        foreach (Sound s in allSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        audios = gameObject.GetComponents<AudioSource>();
    }

    //Plays the associated AudioSource -- good for looping sounds or sounds you need to later Stop() 
    public void Play(string name)
    {
        Sound s = Array.Find(allSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    //Creates a new AudioSource and randomizes pitch/volume for variety -- good for sounds that happen in quick succession
    public void SpawnSFX(string name)
    {
        Sound s = Array.Find(allSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }

        AudioSource prefab = Instantiate(SFXPrefab).GetComponent<AudioSource>();
        prefab.volume = UnityEngine.Random.Range(0.8f, 1.2f) * s.source.volume;
        prefab.pitch = UnityEngine.Random.Range(0.85f, 1.15f) * s.source.pitch;
        prefab.resource = s.source.clip;
        prefab.Play();
        Destroy(prefab, s.source.clip.length);
    }

    //Instantly stop a clip you started with Play()
    public void Stop(string name)
    {
        Sound s = Array.Find(allSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    //Fade pitch/volume over a set duration
    public IEnumerator FadeEffects(string name, float duration, float volume, float pitch = 1)
    {
        Sound s = Array.Find(allSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            yield break;
        }

        float elapsed = 0;
        float startVol = s.source.volume;
        float startPitch = s.source.pitch;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            s.source.volume = Mathf.Lerp(startVol, volume, elapsed / duration);
            s.source.pitch = Mathf.Lerp(startPitch, pitch, elapsed / duration);
            yield return null;
        }
    }

    //Instantly set pitch/volume
    public void SetEffects(string name, float volume, float pitch = 1)
    {
        Sound s = Array.Find(allSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }

        s.source.volume = volume;
        s.source.pitch = pitch;
    }

    // Get the current volume of a sound
    public float GetVolume(string name)
    {
        Sound s = Array.Find(allSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return 0;
        }
        return s.volume;
    }
}


[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0, 3f)]
    public float pitch = 1f;

    public bool loop;

    [HideInInspector] public AudioSource source;
}