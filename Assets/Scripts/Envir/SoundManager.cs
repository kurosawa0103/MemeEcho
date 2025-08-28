using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioMixerGroup sfxMixerGroup;
    public AudioClip[] audioClips;
    public GameObject audioSourcePrefab;

    public int maxSimultaneousSounds = 3;

    private Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private Queue<AudioSource> activeSources = new Queue<AudioSource>(); // 维护播放顺序

    private void Awake()
    {
        Instance = this;

        foreach (var clip in audioClips)
        {
            if (clip != null && !clipDict.ContainsKey(clip.name))
                clipDict.Add(clip.name, clip);
        }

        // 初始化 AudioSource 对象池
        for (int i = 0; i < maxSimultaneousSounds; i++)
        {
            GameObject go = Instantiate(audioSourcePrefab, transform);
            AudioSource source = go.GetComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxMixerGroup;
            go.SetActive(false);
            audioSourcePool.Add(source);
        }
    }

    public void PlaySFX(string soundName)
    {
        if (!clipDict.TryGetValue(soundName, out var clip))
        {
            Debug.LogWarning($"SoundManager: 找不到音效 {soundName}");
            return;
        }

        // 尝试获取空闲的 AudioSource
        AudioSource source = GetAvailableAudioSource();

        if (source == null)
        {
            // 没有空闲的，停止最早的那个
            if (activeSources.Count > 0)
            {
                source = activeSources.Dequeue();
                source.Stop();
            }
            else
            {
                Debug.LogWarning("音效池为空且没有可替换的音效！");
                return;
            }
        }

        source.gameObject.SetActive(true);
        source.PlayOneShot(clip);
        activeSources.Enqueue(source);

        // 自动回收
        StartCoroutine(RecycleWhenFinished(source, clip.length));
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
                return source;
        }
        return null;
    }

    private System.Collections.IEnumerator RecycleWhenFinished(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!source.isPlaying)
        {
            source.gameObject.SetActive(false);
        }
    }
}
