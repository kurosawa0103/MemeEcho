using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening; 

public class EventMaker : MonoBehaviour
{

    public enum PlayerDirection
    {
        Left,   // 
        Right   //
    }

    public Animator animator; // 岛屿角色的 Animator
    private Animator animatorGG;
    public string animationName;

    public string animationNameGuiGui;
    private Sailor sailor;
    public  bool needSailor=true;
    public TimeSystem timeSystem;
    public bool needDayAndNight;
    public DayState currentDayState;

    [Header("初始黑屏淡出设置")]
    public bool needFadeOut = false;
    public SpriteRenderer fadeSprite; // 用来淡入淡出的黑色Sprite
    public float fadeDuration = 1f;   // 淡入淡出时长


    private void Awake()
    {
        if(needFadeOut)
        {
            FadeOut();
        }
        //SetPlayerAnima();
    }
    void Start()
    {
        //dialogManager = FindObjectOfType<DialogManager>();
        //dialogManager.dialogPool = dialogPool;
        //dialogManager.RefreshDialogList();
        //dialogManager.selectDialogData = null;
        //SetPlayerAnima();
        //SetSailor();
        //StopDayAndNight();
    }

    private void Update()
    {

    }
    void SetPlayerAnima()
    {
        PlayPlayerAnimation(animator, animationName); // 播放岛屿角色的动画（根据实际情况修改动画名称）
        PlayPlayerAnimation(animatorGG, animationNameGuiGui); // 播放岛屿角色的动画（根据实际情况修改动画名称）
    }
    void FadeOut()
    {
        fadeSprite.gameObject.SetActive(true);
        fadeSprite.color = new Color(0, 0, 0, 1); // 一开始黑色
        fadeSprite.DOFade(0, fadeDuration).OnComplete(() =>
        {
            fadeSprite.gameObject.SetActive(false);
        });
    }
    void SetSailor()
    {
        //执行航行
        sailor = FindObjectOfType<Sailor>();
        sailor.isSailing = needSailor;
    }
    void StopDayAndNight()
    {
        timeSystem = FindObjectOfType<TimeSystem>();
        if (!needDayAndNight)
        {           
            timeSystem.StopDayNightCycle();
            //设置
            timeSystem.SetDayState(currentDayState,2);
        }
        else
        {
            timeSystem.ResumeDayNightCycle();
        }

    }

    // 根据动画名称播放指定动画
    void PlayPlayerAnimation(Animator animator, string animationName)
    {
        if (animator == null) return;

        // 使用动画名称直接播放指定动画
        animator.Play(animationName);
    }

    public void RefreshEvent()
    {
        SetPlayerAnima();
    }
}
