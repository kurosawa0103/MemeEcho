using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;

public enum DayState
{
    [LabelText("白天")]
    DAY = 1,
    [LabelText("黄昏")]
    DUSK = 2,
    [LabelText("夜晚")]
    NIGHT = 4,
}

public class TimeSystem : MonoBehaviour
{
    [LabelText("游戏时间计数")]
    public float timeCount;

    [LabelText("当前时间类型")]
    public DayState dayState;

    [Header("时间段配置")]
    public bool stopDayNightCycle = false;

    [ShowIf("@!stopDayNightCycle")]
    [LabelText("白天长度")]
    public float dayLength = 300f;

    [ShowIf("@!stopDayNightCycle")]
    [LabelText("黄昏长度")]
    public float duskLength = 100f;

    [ShowIf("@!stopDayNightCycle")]
    [LabelText("夜晚长度")]
    public float nightLength = 200f;


    [LabelText("粒子系统父物体")]
    public Transform nightParticleParent;

    public Transform fogColorTransform;

    [LabelText("白天雾气")]
    public Color fogColorDay;
    [LabelText("黄昏雾气")]
    public Color fogColorDusk;
    [LabelText("夜晚雾气")]
    public Color fogColorNight;

    public Light2D sunlight;

    [LabelText("白天渲染色")]
    public Color sunColorDay;
    [LabelText("黄昏渲染色")]
    public Color sunColorDusk;
    [LabelText("夜晚渲染色")]
    public Color sunColorNight;

    [Header("时间段对应图片")]
    public SpriteRenderer dayImage;
    public SpriteRenderer duskImage;
    public SpriteRenderer nightImage;

    [LabelText("昼夜淡入淡出时间")]
    public float imageFadeDuration = 10f;

    [LabelText("夜晚氛围特效")]
    public GameObject nightEff;


    public EventManager eventManager;

    void Start()
    {
        DayAndNight();
    }

    void Update()
    {

        // 只有在没有停止昼夜切换时才进行昼夜切换
        if (!stopDayNightCycle)
        {        
            DayAndNight();
        }
        // 每天结束时增加天数
        if (timeCount >= dayLength + duskLength + nightLength)
        {
            //dayCount++;
            //timeCount = 0;
        }
    }


    // 昼夜系统
    public void DayAndNight()
    {
        //timeCount += Time.deltaTime * speedValue;

        DayState newState = dayState;  // 当前保持的状态

        // 判断应该处于哪个时间段
        if (timeCount >= 0 && timeCount < dayLength)
        {
            newState = DayState.DAY;
        }
        else if (timeCount >= dayLength && timeCount < (dayLength + duskLength))
        {
            newState = DayState.DUSK;
        }
        else if (timeCount >= (dayLength + duskLength))
        {
            newState = DayState.NIGHT;
        }

        // 只有状态变化时才设置
        if (newState != dayState)
        {
            SetDayState(newState,imageFadeDuration);
        }
    }


    // 新增：停止昼夜切换
    public void StopDayNightCycle()
    {
        stopDayNightCycle = true;
    }

    // 新增：恢复昼夜切换
    public void ResumeDayNightCycle()
    {
        stopDayNightCycle = false;
    }

    void FadeIn(SpriteRenderer sr,float fadeTime)
    {
        sr.DOFade(1f, fadeTime);
    }

    void FadeOut(SpriteRenderer sr, float fadeTime)
    {
        sr.DOFade(0f, fadeTime);
    }
    private void StopAllParticlesInParent()
    {
        if (nightParticleParent == null) return;

        var particles = nightParticleParent.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            if (ps.isPlaying)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    private void PlayAllParticlesInParent()
    {
        if (nightParticleParent == null) return;

        var particles = nightParticleParent.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            if (!ps.isPlaying)
            {
                ps.Play();
            }
        }
    }
    public void SetDayState(DayState newDayState,float fadeTime)
    {
        dayState = newDayState;

        // 每次切换状态时，刷新一次粒子父物体
        UpdateNightParticleParentByTag();
        switch (newDayState)
        {
            case DayState.DAY:
                timeCount = 0f;
                fogColorTransform.GetComponent<SpriteRenderer>().DOColor(fogColorDay, fadeTime);
                StopAllParticlesInParent();//关闭粒子
                DOTween.To(() => sunlight.color, x => sunlight.color = x, sunColorDay, fadeTime); // 修复后
                FadeOut(duskImage, fadeTime);
                FadeOut(nightImage, fadeTime);
                FadeIn(dayImage, fadeTime);
                break;
            case DayState.DUSK:
                timeCount = dayLength;
                fogColorTransform.GetComponent<SpriteRenderer>().DOColor(fogColorDusk, fadeTime);
                DOTween.To(() => sunlight.color, x => sunlight.color = x, sunColorDusk, fadeTime); // 修复后
                FadeOut(dayImage, fadeTime);
                FadeOut(nightImage, fadeTime);
                FadeIn(duskImage, fadeTime);
                break;
            case DayState.NIGHT:
                timeCount = dayLength + duskLength;
                fogColorTransform.GetComponent<SpriteRenderer>().DOColor(fogColorNight, fadeTime);
                DOTween.To(() => sunlight.color, x => sunlight.color = x, sunColorNight, fadeTime); // 修复后
                PlayAllParticlesInParent(); //激活粒子
                FadeOut(dayImage, fadeTime);
                FadeOut(duskImage, fadeTime);
                FadeIn(nightImage, fadeTime);

                break;
        }
    }

    private void UpdateNightParticleParentByTag()
    {
        GameObject nightParticleGO = GameObject.FindWithTag("NightParticle");
        if (nightParticleGO != null)
        {
            nightParticleParent = nightParticleGO.transform;
        }
        else
        {
            nightParticleParent = null;
            Debug.LogWarning("未找到带有 Tag 'NightParticle' 的对象！");
        }
    }

}
