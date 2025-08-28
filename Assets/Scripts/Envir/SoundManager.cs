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
    private Queue<AudioSource> activeSources = new Queue<AudioSource>(); // ά������˳��

    private void Awake()
    {
        Instance = this;

        foreach (var clip in audioClips)
        {
            if (clip != null && !clipDict.ContainsKey(clip.name))
                clipDict.Add(clip.name, clip);
        }

        // ��ʼ�� AudioSource �����
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
            Debug.LogWarning($"SoundManager: �Ҳ�����Ч {soundName}");
            return;
        }

        // ���Ի�ȡ���е� AudioSource
        AudioSource source = GetAvailableAudioSource();

        if (source == null)
        {
            // û�п��еģ�ֹͣ������Ǹ�
            if (activeSources.Count > 0)
            {
                source = activeSources.Dequeue();
                source.Stop();
            }
            else
            {
                Debug.LogWarning("��Ч��Ϊ����û�п��滻����Ч��");
                return;
            }
        }

        source.gameObject.SetActive(true);
        source.PlayOneShot(clip);
        activeSources.Enqueue(source);

        // �Զ�����
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
