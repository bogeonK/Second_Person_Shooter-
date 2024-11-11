using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] clips; // 0���� StartScene BGM, 1���� PlayScene BGM
    public static AudioManager instance;
    AudioSource audio_Click;
    AudioSource audio_Walk;
    AudioSource bgmSource; // ��������� ���� AudioSource �߰�

    private float walkCooldown = 0.5f; // ��ٿ� �ð� (0.5��)
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

    // Ŭ�� ȿ�� ����Լ�
    public void Audio_Click(int clipNumber)
    {
        // ��Ȳ�� �°� ����� Ŭ�� ��ü �� ���
        audio_Click.clip = clips[clipNumber];
        audio_Click.Play();
    }

    public void Audio_Walk(bool isWalking)
    {
        if (isWalking && !audio_Walk.isPlaying && Time.time - lastWalkTime > walkCooldown)
        {
            audio_Walk.Play();
            lastWalkTime = Time.time; // ������ ��� �ð� ����
        }
        else if (!isWalking)
        {
            audio_Walk.Stop();
        }
    }
}
