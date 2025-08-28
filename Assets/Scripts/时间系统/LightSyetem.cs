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
    [LabelText("白天渲染色")]
    public Color sunColorDay;
    [LabelText("黄昏渲染色")]
    public Color sunColorDusk;
    [LabelText("夜晚渲染色")]
    public Color sunColorNight;
    [LabelText("混合时间")]
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
