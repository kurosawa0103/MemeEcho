using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ECGLine : MonoBehaviour
{
    public int pointCount = 200;
    public float scrollSpeed = 1f;
    public float amplitude = 1f;
    public float updateInterval = 0.01f;

    public HeartBeat heartBeat;
    
    private LineRenderer lineRenderer;
    private Queue<float> ecgPoints = new Queue<float>();
    private float timer = 0f;
    private float currentAlpha = 0f;
    public float fadeSpeed = 1f;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

       

        lineRenderer.positionCount = pointCount;
      
       lineRenderer.useWorldSpace = false;

        if (heartBeat == null)
        {
            heartBeat = FindObjectOfType<HeartBeat>();
        }

        for (int i = 0; i < pointCount; i++)
        {
            ecgPoints.Enqueue(0f);
        }
    }

    void Update()
    {
        if (heartBeat == null || heartBeat.beatCurve == null || heartBeat.beatCurve.length == 0) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer -= updateInterval;

            float t = Time.time % 1f;
            float rawValue = heartBeat.beatCurve.Evaluate(t);
            float newValue = amplitude * rawValue;

            ecgPoints.Dequeue();
            ecgPoints.Enqueue(newValue);

            var values = ecgPoints.ToArray();
            for (int i = 0; i < pointCount; i++)
            {
                float x = i / (float)pointCount * 10f;
                float y = values[i];
                lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
            }

            // 控制整条线条透明度
            if (rawValue >= 0.9f)
            {
                currentAlpha = 1f;
            }
            else
            {
                currentAlpha -= fadeSpeed * Time.deltaTime;
                currentAlpha = Mathf.Clamp01(currentAlpha);
            }

            // ✅ 只修改现有颜色的 alpha，不动 RGB
            Color baseColor = lineRenderer.startColor;
            baseColor.a = currentAlpha;
            lineRenderer.startColor = baseColor;
            lineRenderer.endColor = baseColor;

        }
    }
}