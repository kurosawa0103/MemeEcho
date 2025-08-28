using UnityEngine;
using TunnelEffect;

public class TunnelFXAutoAdjust : MonoBehaviour
{
    public TunnelFX2 tunnelFX;

    public float targetSpeed = 3f;
    public float maxSpeed = 5f;
    public float speedIncreaseRate = 1f;
    public float minFallOffStart = 0.1f;
    public float minFallOffEnd = 0f;
    public float fallOffReduceRate = 0.5f;

    public float alphaFadeSpeed = 0.5f; // 淡出速度

    public bool allowContinueSpeedUp = false; // 由 Fungus 控制是否继续加速
    public bool stopAtTargetSpeed = false;   // 是否在 targetSpeed 达到后停止加速

    private bool[] speedReached;
    private int layerCount;

    private bool startFadingAlpha = false;

    void Start()
    {
        if (tunnelFX == null)
        {
            tunnelFX = TunnelFX2.instance;
        }

        layerCount = tunnelFX.layerCount;
        speedReached = new bool[layerCount];
    }

    void Update()
    {
        for (int i = 0; i < layerCount; i++)
        {
            if (speedReached[i]) continue;

            float currentSpeed = tunnelFX.GetTravelSpeed(i);

            if (stopAtTargetSpeed)
            {
                if (currentSpeed < targetSpeed)
                {
                    currentSpeed += speedIncreaseRate * Time.deltaTime;
                }

                if (currentSpeed >= targetSpeed)
                {
                    currentSpeed = targetSpeed;
                }
            }
            else
            {
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += speedIncreaseRate * Time.deltaTime;
                }

                if (currentSpeed >= maxSpeed)
                {
                    currentSpeed = maxSpeed;
                    speedReached[i] = true;
                }
            }

            if (allowContinueSpeedUp && currentSpeed < maxSpeed)
            {
                currentSpeed += speedIncreaseRate * Time.deltaTime;
                if (currentSpeed >= maxSpeed)
                {
                    currentSpeed = maxSpeed;
                    speedReached[i] = true;
                }
            }

            tunnelFX.SetTravelSpeed(i, currentSpeed);
        }

        if (AllSpeedsReached())
        {
            if (tunnelFX.fallOff > 0)
            {
                tunnelFX.fallOff -= fallOffReduceRate * Time.deltaTime;
                if (tunnelFX.fallOff <= 0)
                {
                    tunnelFX.fallOff = 0;
                    startFadingAlpha = true;
                }
            }
        }

        // 淡出 globalAlpha
        if (startFadingAlpha && tunnelFX.globalAlpha > 0)
        {
            tunnelFX.globalAlpha -= alphaFadeSpeed * Time.deltaTime;
            if (tunnelFX.globalAlpha < 0)
            {
                tunnelFX.globalAlpha = 0;
                startFadingAlpha = false; // 可选：防止持续更新
            }
        }
    }

    private bool AllSpeedsReached()
    {
        foreach (var reached in speedReached)
        {
            if (!reached) return false;
        }
        return true;
    }

    // Fungus调用该方法来继续加速
    public void ContinueSpeedUp()
    {
        allowContinueSpeedUp = true;
    }
}