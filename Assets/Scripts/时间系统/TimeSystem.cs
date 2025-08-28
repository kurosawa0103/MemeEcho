using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;

public enum DayState
{
    [LabelText("����")]
    DAY = 1,
    [LabelText("�ƻ�")]
    DUSK = 2,
    [LabelText("ҹ��")]
    NIGHT = 4,
}

public class TimeSystem : MonoBehaviour
{
    [LabelText("��Ϸʱ�����")]
    public float timeCount;

    [LabelText("��ǰʱ������")]
    public DayState dayState;

    [Header("ʱ�������")]
    public bool stopDayNightCycle = false;

    [ShowIf("@!stopDayNightCycle")]
    [LabelText("���쳤��")]
    public float dayLength = 300f;

    [ShowIf("@!stopDayNightCycle")]
    [LabelText("�ƻ賤��")]
    public float duskLength = 100f;

    [ShowIf("@!stopDayNightCycle")]
    [LabelText("ҹ����")]
    public float nightLength = 200f;


    [LabelText("����ϵͳ������")]
    public Transform nightParticleParent;

    public Transform fogColorTransform;

    [LabelText("��������")]
    public Color fogColorDay;
    [LabelText("�ƻ�����")]
    public Color fogColorDusk;
    [LabelText("ҹ������")]
    public Color fogColorNight;

    public Light2D sunlight;

    [LabelText("������Ⱦɫ")]
    public Color sunColorDay;
    [LabelText("�ƻ���Ⱦɫ")]
    public Color sunColorDusk;
    [LabelText("ҹ����Ⱦɫ")]
    public Color sunColorNight;

    [Header("ʱ��ζ�ӦͼƬ")]
    public SpriteRenderer dayImage;
    public SpriteRenderer duskImage;
    public SpriteRenderer nightImage;

    [LabelText("��ҹ���뵭��ʱ��")]
    public float imageFadeDuration = 10f;

    [LabelText("ҹ���Χ��Ч")]
    public GameObject nightEff;


    public EventManager eventManager;

    void Start()
    {
        DayAndNight();
    }

    void Update()
    {

        // ֻ����û��ֹͣ��ҹ�л�ʱ�Ž�����ҹ�л�
        if (!stopDayNightCycle)
        {        
            DayAndNight();
        }
        // ÿ�����ʱ��������
        if (timeCount >= dayLength + duskLength + nightLength)
        {
            //dayCount++;
            //timeCount = 0;
        }
    }


    // ��ҹϵͳ
    public void DayAndNight()
    {
        //timeCount += Time.deltaTime * speedValue;

        DayState newState = dayState;  // ��ǰ���ֵ�״̬

        // �ж�Ӧ�ô����ĸ�ʱ���
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

        // ֻ��״̬�仯ʱ������
        if (newState != dayState)
        {
            SetDayState(newState,imageFadeDuration);
        }
    }


    // ������ֹͣ��ҹ�л�
    public void StopDayNightCycle()
    {
        stopDayNightCycle = true;
    }

    // �������ָ���ҹ�л�
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

        // ÿ���л�״̬ʱ��ˢ��һ�����Ӹ�����
        UpdateNightParticleParentByTag();
        switch (newDayState)
        {
            case DayState.DAY:
                timeCount = 0f;
                fogColorTransform.GetComponent<SpriteRenderer>().DOColor(fogColorDay, fadeTime);
                StopAllParticlesInParent();//�ر�����
                DOTween.To(() => sunlight.color, x => sunlight.color = x, sunColorDay, fadeTime); // �޸���
                FadeOut(duskImage, fadeTime);
                FadeOut(nightImage, fadeTime);
                FadeIn(dayImage, fadeTime);
                break;
            case DayState.DUSK:
                timeCount = dayLength;
                fogColorTransform.GetComponent<SpriteRenderer>().DOColor(fogColorDusk, fadeTime);
                DOTween.To(() => sunlight.color, x => sunlight.color = x, sunColorDusk, fadeTime); // �޸���
                FadeOut(dayImage, fadeTime);
                FadeOut(nightImage, fadeTime);
                FadeIn(duskImage, fadeTime);
                break;
            case DayState.NIGHT:
                timeCount = dayLength + duskLength;
                fogColorTransform.GetComponent<SpriteRenderer>().DOColor(fogColorNight, fadeTime);
                DOTween.To(() => sunlight.color, x => sunlight.color = x, sunColorNight, fadeTime); // �޸���
                PlayAllParticlesInParent(); //��������
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
            Debug.LogWarning("δ�ҵ����� Tag 'NightParticle' �Ķ���");
        }
    }

}
