using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;

public class LightSyetem : MonoBehaviour
{
    private TimeSystem _timeSystem;
    private Light2D sunlight;
    [LabelText("������Ⱦɫ")]
    public Color sunColorDay;
    [LabelText("�ƻ���Ⱦɫ")]
    public Color sunColorDusk;
    [LabelText("ҹ����Ⱦɫ")]
    public Color sunColorNight;
    [LabelText("���ʱ��")]
    public float changeTime;

    void Start()
    {
        _timeSystem = FindObjectOfType<TimeSystem>();
        sunlight = transform.GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        sunlight.color = transform.GetComponent<SpriteRenderer>().color;
        switch (_timeSystem.dayState)
        {

            case DayState.DAY:
                {
                    transform.GetComponent<SpriteRenderer>().DOColor(sunColorDay, changeTime);
                }
                break;
            case DayState.DUSK:
                {
                    transform.GetComponent<SpriteRenderer>().DOColor(sunColorDusk, changeTime);
                }
                break;
            case DayState.NIGHT:
                {
                    transform.GetComponent<SpriteRenderer>().DOColor(sunColorNight, changeTime);
                }
                break;



        }

    }
}
