using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsControl : MonoBehaviour {
    public static SoundsControl Instance { get; private set; }

    public AudioClip[] soundEffect;
    public AudioClip[] buttonSoundFX;
    public AudioClip[] musics;
    private int currentMusicIndex;

    private AudioSource audioSource;
    public AudioSource musicAudioSource;
    public float minMusicVolume = 0.1f;
    public float maxMusicVolume = 0.5f;
    private bool musicStarted;
    private void Start () {
        Instance = this;
        audioSource = GetComponent<AudioSource> ();
        StartCoroutine (InitMusic (5));
    }
    private void Update () {
        if (!musicAudioSource.isPlaying && musicStarted && OptionControl.Instance.isMusic) {
            PlayMusicShuffle (true);
        }

    }
    public void PlayButtonSound (int index) {
        if (OptionControl.Instance.isSound) {
            audioSource.PlayOneShot (buttonSoundFX[index]);
        }
    }
    public void PlaySoundEffect (int index) {
        if (OptionControl.Instance.isSound) {
            audioSource.PlayOneShot (soundEffect[index]);
        }
    }
    public void PlayMusicLoop (bool inFX) {
            currentMusicIndex++;
        if (currentMusicIndex >= musics.Length) {
            currentMusicIndex = 0;
        } 
        musicAudioSource.PlayOneShot (musics[currentMusicIndex]);
        if (inFX)
            FadeInEffect ();
        else
            FadeOutEffect ();
    }
    public void PlayMusicShuffle (bool inFX) {
        int random = Random.Range (0, musics.Length);
        do {
            if (random == currentMusicIndex) {
                random = Random.Range (0, musics.Length);
            }
        } while (random == currentMusicIndex);
        currentMusicIndex = random;
        musicAudioSource.PlayOneShot (musics[currentMusicIndex]);
        if (inFX)
            FadeInEffect ();
        else
            FadeOutEffect ();
    }
    public void FadeInEffect () {
        StartCoroutine (FadeInEffect (1));
    }
    public void FadeOutEffect () {
        StartCoroutine (FadeOutEffect (1));
    }
    public void StopMusic () {
        //StopCoroutine (StopMusic (1));
        StartCoroutine (StopMusic (1));
    }
    private IEnumerator StopMusic (float time) {
        float count = 0;
        while (count < time + 0.2f) {
            if (count >= time) {
                musicAudioSource.Stop ();
                break;
            }
            musicAudioSource.volume = Mathf.Lerp (musicAudioSource.volume, 0, 5 * Time.deltaTime);
            count += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
    private IEnumerator FadeOutEffect (float time) {
        // musicAudioSource.volume = minMusicVolume;
        float count = 0;
        while (count < time + .2f) {
            if (count >= time) {
                musicAudioSource.volume = minMusicVolume;
                break;
            }
            musicAudioSource.volume = Mathf.Lerp (musicAudioSource.volume, minMusicVolume, 0.5f * Time.smoothDeltaTime);
            count += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
    private IEnumerator FadeInEffect (float time) {
        // musicAudioSource.volume = 0;
        float count = 0;
        while (count < time + .2f) {
            if (count >= time) {
                musicAudioSource.volume = maxMusicVolume;
                break;
            }
            if (count > 1)
                musicAudioSource.volume = Mathf.Lerp (musicAudioSource.volume, maxMusicVolume, 0.5f * Time.smoothDeltaTime);
            count += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
    private IEnumerator InitMusic (float time) {
        if (!OptionControl.Instance.isMusic) {
            musicStarted = true;
            yield return null;
        } else {
            musicAudioSource.volume = 0;
            musicStarted = false;
            currentMusicIndex = UnityEngine.Random.Range (0, musics.Length);
            musicAudioSource.PlayOneShot (musics[currentMusicIndex]);
            float count = 0;
            while (count < time + .2f) {
                if (count >= time) {
                    musicAudioSource.volume = minMusicVolume;
                    musicStarted = true;
                    break;
                }
                if (count > 1)
                    musicAudioSource.volume = Mathf.Lerp (musicAudioSource.volume, minMusicVolume, 0.25f * Time.smoothDeltaTime);
                count += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }
}