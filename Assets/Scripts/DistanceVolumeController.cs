using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DistanceVolumeController : MonoBehaviour
{
    [Header("监听目标")]
    public Transform target;

    [Header("音量范围")]
    [Range(0f, 1f)]
    public float minVolume = 0.1f;

    [Range(0f, 1f)]
    public float maxVolume = 1f;

    [Header("距离控制")]
    public float maxDistance = 10f;

    private AudioSource audioSource;
    private GameObject box;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        box = GameObject.FindWithTag("Box");
    }

    void Update()
    {
        // 只有在播放中才调整音量
        if (audioSource.isPlaying && target != null)
        {
            float distance = Vector3.Distance(box.transform.position, target.position);
            float t = Mathf.Clamp01(1f - (distance / maxDistance));
            audioSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
        }
    }


}
