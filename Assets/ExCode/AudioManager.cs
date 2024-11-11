using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] clips; // 0번은 StartScene BGM, 1번은 PlayScene BGM
    public static AudioManager instance;
    AudioSource audio_Click;
    AudioSource audio_Walk;
    AudioSource bgmSource; // 배경음악을 위한 AudioSource 추가

    private float walkCooldown = 0.5f; // 쿨다운 시간 (0.5초)
    private float lastWalkTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        var audioSources = GetComponents<AudioSource>();
        audio_Click = audioSources[1];
        audio_Walk = audioSources[2];
        bgmSource = audioSources[0]; 
    }

    public void PlayBGM(int clipIndex)
    {
        if (bgmSource.clip != clips[clipIndex])
        {
            bgmSource.clip = clips[clipIndex];
            bgmSource.Play();
        }
    }

    // 클릭 효과 재생함수
    public void Audio_Click(int clipNumber)
    {
        // 상황에 맞게 오디오 클립 교체 후 재생
        audio_Click.clip = clips[clipNumber];
        audio_Click.Play();
    }

    public void Audio_Walk(bool isWalking)
    {
        if (isWalking && !audio_Walk.isPlaying && Time.time - lastWalkTime > walkCooldown)
        {
            audio_Walk.Play();
            lastWalkTime = Time.time; // 마지막 재생 시간 갱신
        }
        else if (!isWalking)
        {
            audio_Walk.Stop();
        }
    }
}
