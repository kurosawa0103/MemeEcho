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

    public Animator animator; // �����ɫ�� Animator
    private Animator animatorGG;
    public string animationName;

    public string animationNameGuiGui;
    private Sailor sailor;
    public  bool needSailor=true;
    public TimeSystem timeSystem;
    public bool needDayAndNight;
    public DayState currentDayState;

    [Header("��ʼ������������")]
    public bool needFadeOut = false;
    public SpriteRenderer fadeSprite; // �������뵭���ĺ�ɫSprite
    public float fadeDuration = 1f;   // ���뵭��ʱ��


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
        PlayPlayerAnimation(animator, animationName); // ���ŵ����ɫ�Ķ���������ʵ������޸Ķ������ƣ�
        PlayPlayerAnimation(animatorGG, animationNameGuiGui); // ���ŵ����ɫ�Ķ���������ʵ������޸Ķ������ƣ�
    }
    void FadeOut()
    {
        fadeSprite.gameObject.SetActive(true);
        fadeSprite.color = new Color(0, 0, 0, 1); // һ��ʼ��ɫ
        fadeSprite.DOFade(0, fadeDuration).OnComplete(() =>
        {
            fadeSprite.gameObject.SetActive(false);
        });
    }
    void SetSailor()
    {
        //ִ�к���
        sailor = FindObjectOfType<Sailor>();
        sailor.isSailing = needSailor;
    }
    void StopDayAndNight()
    {
        timeSystem = FindObjectOfType<TimeSystem>();
        if (!needDayAndNight)
        {           
            timeSystem.StopDayNightCycle();
            //����
            timeSystem.SetDayState(currentDayState,2);
        }
        else
        {
            timeSystem.ResumeDayNightCycle();
        }

    }

    // ���ݶ������Ʋ���ָ������
    void PlayPlayerAnimation(Animator animator, string animationName)
    {
        if (animator == null) return;

        // ʹ�ö�������ֱ�Ӳ���ָ������
        animator.Play(animationName);
    }

    public void RefreshEvent()
    {
        SetPlayerAnima();
    }
}
