using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] musicQueue; // 音乐队列
    public AudioSource audioSource; // 用于播放音乐的 AudioSource

    private int currentTrackIndex = 0; // 当前播放的音乐索引
    private bool isPaused = false; // 用于跟踪音乐是否暂停

    public string currentTrackName; // 当前播放的音乐名称
    public AudioClip defaultMusic;
    public bool isPlaying=false;
    private void Start()
    {
    }

    public void OnPlayButtonClicked()
    {
        // 如果音乐正在播放并未暂停，什么都不做
        if (audioSource.isPlaying && !isPaused)
        {
            return;
        }

        // 否则播放或恢复音乐
        PlayMusic();
        isPlaying = true;
    }

    // 播放当前音乐
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

    // 切换到下一首音乐
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

    // 暂停当前音乐
    public void PauseMusic()
    {
        isPlaying = false;
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            audioSource.clip = defaultMusic;//设置默认bgm
            currentTrackName = null;
            audioSource.Play();//播放
            isPaused = true;
        }
    }

    // 更新当前播放音乐的名称
    private void UpdateTrackName()
    {
        if (audioSource.clip != null)
        {
            currentTrackName = audioSource.clip.name;
        }
    }

    // 获取当前播放音乐的名称
    public string GetCurrentTrackName()
    {
        return currentTrackName;
    }
}
