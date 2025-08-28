using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class BGMController : MonoBehaviour
{
    [Header("引用 TimeSystem")]
    public TimeSystem timeSystem;

    [Header("BGM 音频剪辑")]
    public AudioClip dayClip;
    public AudioClip nightClip;

    [Header("淡入淡出配置")]
    public float fadeDuration = 2f;
    [Range(0f, 1f)]
    public float volume = 1f;

    public AudioSource audioSource;
    [ReadOnly]
    public AudioClip currentClip;   // 当前播放中
    [ReadOnly]
    public AudioClip currentBGM;    // 当前设定为应播放（未播放）

    private DayState lastState;

    private bool isOverridden = false;
    private bool returnToAutoAfterOverride = false;
    private float pauseTime = 0f;
    public bool isPaused = false;

    void Start()
    {
        if (timeSystem == null)
        {
            Debug.LogError("未绑定 TimeSystem！");
            enabled = false;
            return;
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        lastState = timeSystem.dayState;
        //SwitchBGM(lastState);
        //PlayCurrentBGM(true); // 初始直接播放
    }

    void Update()
    {
        if (isOverridden) return;

        if (timeSystem.dayState != lastState)
        {
            lastState = timeSystem.dayState;
            SwitchBGM(lastState);
            PlayCurrentBGM(); // 切换后自动播放（含淡出淡入）
        }
    }

    /// <summary>
    /// 设置当前 BGM（不立即播放）
    /// </summary>
    public void SwitchBGM(DayState state)
    {
        if (state == DayState.DUSK) return;

        AudioClip targetClip = null;
        if (state == DayState.DAY) targetClip = dayClip;
        else if (state == DayState.NIGHT) targetClip = nightClip;

        if (targetClip != null && targetClip != currentBGM)
            currentBGM = targetClip;
    }

    /// <summary>
    /// 播放当前 BGM（自动淡入淡出）
    /// </summary>
    public void PlayCurrentBGM(bool instant = false)
    {
        if (currentBGM == null || currentBGM == currentClip) return;

        StopAllCoroutines();
        StartCoroutine(FadeToClip(currentBGM, instant));
    }

    IEnumerator FadeToClip(AudioClip newClip, bool instant)
    {
        float fadeOutTime = instant ? 0f : fadeDuration;
        float fadeInTime = instant ? 0f : fadeDuration;

        // 淡出当前
        float t = 0f;
        float startVolume = audioSource.volume;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        currentClip = newClip;
        audioSource.Play();

        // 淡入新曲
        t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, t / fadeInTime);
            yield return null;
        }

        audioSource.volume = volume;

        if (isOverridden && !audioSource.loop && returnToAutoAfterOverride)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            isOverridden = false;
            SwitchBGM(timeSystem.dayState);
            PlayCurrentBGM();
        }
    }

    public void OverrideBGM(AudioClip newClip, bool loop = true, bool returnToAutoAfter = true)
    {
        if (newClip == null) return;

        isOverridden = true;
        this.returnToAutoAfterOverride = returnToAutoAfter;

        audioSource.loop = loop;
        StopAllCoroutines();
        StartCoroutine(FadeToClip(newClip, false));
    }

    public void StopBGM(bool fadeOut = true)
    {
        StopAllCoroutines();
        if (fadeOut)
        {
            StartCoroutine(FadeOutAndStop());
        }
        else
        {
            audioSource.Stop();
        }
        isOverridden = false;
    }

    IEnumerator FadeOutAndStop()
    {
        float t = 0f;
        float startVolume = audioSource.volume;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        audioSource.Stop();
    }

    public void PlayAutoBGM(bool instant = false)
    {
        isOverridden = false;
        returnToAutoAfterOverride = false;
        SwitchBGM(timeSystem.dayState);
        PlayCurrentBGM(instant);
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        audioSource.volume = volume;
    }

    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            pauseTime = audioSource.time;
            StartCoroutine(FadeOutAndPause());
        }
    }

    IEnumerator FadeOutAndPause()
    {
        float t = 0f;
        float startVolume = audioSource.volume;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        audioSource.Pause();
        isPaused = true;
    }

    public void ResumeBGM()
    {
        if (isPaused && currentClip != null)
        {
            audioSource.clip = currentClip;
            audioSource.time = pauseTime;
            audioSource.Play();
            StartCoroutine(FadeInAfterResume());
            isPaused = false;
        }
    }

    IEnumerator FadeInAfterResume()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = volume;
    }

    [ContextMenu("停止BGM")]
    public void DebugStop() => StopBGM();

    [ContextMenu("设置音量为0.2")]
    public void DebugSetVolume() => SetVolume(0.2f);

    public void ReplaceDayClip(AudioClip newClip, bool playNow = false)
    {
        dayClip = newClip;
        if (playNow && timeSystem.dayState == DayState.DAY)
        {
            isOverridden = false;
            SwitchBGM(DayState.DAY);
            PlayCurrentBGM();
        }
    }

    public void ReplaceNightClip(AudioClip newClip, bool playNow = false)
    {
        nightClip = newClip;
        if (playNow && timeSystem.dayState == DayState.NIGHT)
        {
            isOverridden = false;
            SwitchBGM(DayState.NIGHT);
            PlayCurrentBGM();
        }
    }
}
