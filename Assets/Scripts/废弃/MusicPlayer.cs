using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] musicQueue; // ���ֶ���
    public AudioSource audioSource; // ���ڲ������ֵ� AudioSource

    private int currentTrackIndex = 0; // ��ǰ���ŵ���������
    private bool isPaused = false; // ���ڸ��������Ƿ���ͣ

    public string currentTrackName; // ��ǰ���ŵ���������
    public AudioClip defaultMusic;
    public bool isPlaying=false;
    private void Start()
    {
    }

    public void OnPlayButtonClicked()
    {
        // ����������ڲ��Ų�δ��ͣ��ʲô������
        if (audioSource.isPlaying && !isPaused)
        {
            return;
        }

        // ���򲥷Ż�ָ�����
        PlayMusic();
        isPlaying = true;
    }

    // ���ŵ�ǰ����
    public void PlayMusic()
    {
        UpdateTrackName();
        isPlaying = true;
        if (isPaused)
        {
            audioSource.UnPause();
            isPaused = false;
        }
        else
        {
            audioSource.clip = musicQueue[currentTrackIndex];
            audioSource.Play();
        }
    }

    // �л�����һ������
    public void SwitchMusic()
    {
        if (isPaused)
        {
            isPaused = false;
        }

        currentTrackIndex = (currentTrackIndex + 1) % musicQueue.Length;
        PlayMusic();
        UpdateTrackName();
        isPlaying = true;
    }

    // ��ͣ��ǰ����
    public void PauseMusic()
    {
        isPlaying = false;
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            audioSource.clip = defaultMusic;//����Ĭ��bgm
            currentTrackName = null;
            audioSource.Play();//����
            isPaused = true;
        }
    }

    // ���µ�ǰ�������ֵ�����
    private void UpdateTrackName()
    {
        if (audioSource.clip != null)
        {
            currentTrackName = audioSource.clip.name;
        }
    }

    // ��ȡ��ǰ�������ֵ�����
    public string GetCurrentTrackName()
    {
        return currentTrackName;
    }
}
