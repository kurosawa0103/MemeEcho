using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDebug : MonoBehaviour
{
    public TextMeshProUGUI state;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI timerText;  // ��ʾ��ʱ���� TextMeshProUGUI
    public TextMeshProUGUI sailText;  // ��ʾ��ʱ���� TextMeshProUGUI
    public EventManager eventManager;
    public string stateInfo;

    private float elapsedTime = 0f;  // ��ʱ����ʱ�����
    public Sailor sailor;
    void Start()
    {

    }

    void Update()
    {
        state.text = stateInfo;

        if (eventManager.currentEvent != null)
        {
            eventText.text = eventManager.currentEvent.name.ToString();
        }

        // ���¼�ʱ������ʾΪ���Ӹ�ʽ������ȡ����
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);  // ȡ��������
                                                            // ��ʾ Sailor �ĺ��о���
        if (sailor != null)
        {
            //sailText.text = $"����: {sailor.sailDistance:F1} ��";  // ��ȡ���о���
        }

        timerText.text = $"{minutes}����{seconds}��";
    }

}
