using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class BGMController : MonoBehaviour
{
    [Header("���� TimeSystem")]
    public TimeSystem timeSystem;

    [Header("BGM ��Ƶ����")]
    public AudioClip dayClip;
    public AudioClip nightClip;

    [Header("���뵭������")]
    public float fadeDuration = 2f;
    [Range(0f, 1f)]
    public float volume = 1f;

    public AudioSource audioSource;
    [ReadOnly]
    public AudioClip currentClip;   // ��ǰ������
    [ReadOnly]
    public AudioClip currentBGM;    // ��ǰ�趨ΪӦ���ţ�δ���ţ�

    private DayState lastState;

    private bool isOverridden = false;
    private bool returnToAutoAfterOverride = false;
    private float pauseTime = 0f;
    public bool isPaused = false;

    void Start()
    {
        if (timeSystem == null)
        {
            Debug.LogError("δ�� TimeSystem��");
            enabled = false;
            return;
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        lastState = timeSystem.dayState;
        //SwitchBGM(lastState);
        //PlayCurrentBGM(true); // ��ʼֱ�Ӳ���
    }

    void Update()
    {
        if (isOverridden) return;

        if (timeSystem.dayState != lastState)
        {
            lastState = timeSystem.dayState;
            SwitchBGM(lastState);
            PlayCurrentBGM(); // �л����Զ����ţ����������룩
        }
    }

    /// <summary>
    /// ���õ�ǰ BGM�����������ţ�
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
    /// ���ŵ�ǰ BGM���Զ����뵭����
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

        // ������ǰ
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

        // ��������
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

    [ContextMenu("ֹͣBGM")]
    public void DebugStop() => StopBGM();

    [ContextMenu("��������Ϊ0.2")]
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
