using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private int index = 0;
    private float _delayTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Room 1&2");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip, s.level);
            Debug.Log("Playing Woohoo");
        }
    }

    public float PlaySFXRandom(string name, float minRand, float maxRand, float levelX = 1f)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            //Debug.Log("Sound " + name + " not found");

            return 0f;
        }
        else
        {
            sfxSource.pitch = Random.Range(minRand, maxRand);
            sfxSource.PlayOneShot(s.clip, s.level*levelX);
        }
        return s.clip.length*sfxSource.pitch;
    }

    public async void PlayNextMusic()
    {
        if (index >= musicSounds.Length - 1) return;
        _delayTime = musicSounds[index].clip.length - musicSource.time;
        //Debug.Log(_delayTime*1000);
        await Task.Delay((int)_delayTime*1000);
        musicSource.Stop();
        index++;
        PlayMusic(musicSounds[index].name);
    }
    
    public IEnumerator Fade(bool isSkip, int targetVolume, float duration)
    {
        float currentTime = 0;
        float start = musicSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if (isSkip)
        {
            PlayNextMusic();
        }
        yield break;
    }
}
