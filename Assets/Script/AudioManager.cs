using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //静态实例
    public static AudioManager Instance;
    //SE开关
    public bool SeActive = true;
    //SE音量
    public float SeVolume = 1f;
    //BGM开关
    public bool BgmActive = true;
    //BGM音量
    public float BgmVolume = 1f;
    private void Start()
    {
        Instance = this;
    }
    //持续播放音效
    public void SePlayLoop(Transform AudioTransform, AudioClip se, float pitch = 1f)
    {
        if (SeActive)
        {
            AudioSource audioSource = AudioTransform.GetComponent<AudioSource>();
            if (audioSource.isPlaying)
            {
                return;
            }
            audioSource.clip = se;
            audioSource.pitch = pitch;
            audioSource.volume = SeVolume;
            audioSource.Play();
        }
    }
    //临时播放音效
    public void SePlayOnece(Transform AudioTransform, AudioClip se, float pitch = 1f)
    {
        if (SeActive)
        {
            foreach(AudioSource audio in AudioTransform.gameObject.GetComponents<AudioSource>())
            {
                if (audio.clip == se)
                {
                    return;
                }
            }
            AudioSource audioSource = AudioTransform.gameObject.AddComponent<AudioSource>();
            audioSource.clip = se;
            audioSource.pitch = pitch;
            audioSource.volume = SeVolume;
            audioSource.Play();
            Destroy(audioSource, audioSource.clip.length);
        }
    }
    //点播放音效
    public void SePlay(Vector3 position, AudioClip se)
    {
        if (SeActive)
        {
            AudioSource.PlayClipAtPoint(se, position, SeVolume);
        }
    }
    //播放音乐
    public void BgmPlay(Transform AudioTransform, AudioClip bgm)
    {
        AudioSource audioSource = AudioTransform.GetComponentInChildren<AudioSource>();
        if (BgmActive)
        {
            audioSource.volume = BgmVolume;
            if (audioSource.isPlaying)
            {               
                return;
            }
            audioSource.clip = bgm;
            audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
