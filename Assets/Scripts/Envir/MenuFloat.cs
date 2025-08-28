using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuFloat : MonoBehaviour
{
    public Vector3 offset;
    public float frequency;

    private Vector3 originPosition;
    private float tick;
    private float amplitude;
    private bool animate;
    public float delayTime;

    void Awake()
    {

        // 如果没有设置频率或者设置的频率为0则自动记录成1
        if (Mathf.Approximately(frequency, 0))
            frequency = 1f;

        originPosition = transform.localPosition;
        tick = Random.Range(0f, 2f * Mathf.PI);
        // 计算振幅
        amplitude = 2 * Mathf.PI / frequency;
     
    }

    public void Play()
    {
        transform.localPosition = originPosition;
        animate = true;
    }

    public void Stop()
    {
        transform.localPosition = originPosition;
        animate = false;
    }

    void FixedUpdate()
    {
        if(delayTime >=0)
        {
            animate = false;
            delayTime -= Time.deltaTime;
        }
        if (delayTime <= 0)
        {
            animate = true;
        }

        if (animate)
        {
            // 计算下一个时间量
            tick = tick + Time.fixedDeltaTime * amplitude;
            // 计算下一个偏移量
            var amp = new Vector3(Mathf.Cos(tick) * offset.x, Mathf.Sin(tick) * offset.y, Mathf.Sin(tick) * offset.z);
            // 更新坐标
            transform.localPosition = originPosition + amp;
        }
    }
}