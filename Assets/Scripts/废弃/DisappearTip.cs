using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DisappearTip : MonoBehaviour
{
    public TextMeshProUGUI coinTextPrefab; // ������ʾӲ��ֵ���ı�Ԥ����
    public float moveUpDistance = 50f; // Ӳ�������ľ���
    public float duration = 5f; // ��������ʱ��
    public Transform  parent; // Ӳ���ı��ĸ��� Canvas

    private void Start()
    {
       
    }

    public void ShowWord()
    {
        // ��������ʾӲ��ֵ���ı�����ʼλ����Ӳ���Ϸ�
        TextMeshProUGUI coinText = Instantiate(coinTextPrefab, parent);

        // ��ʼ�ı��Ķ���
        Sequence textSequence = DOTween.Sequence();

        // 1. �ı��������ƶ���͸�����𽥱�Ϊ0
        textSequence.Append(coinText.transform.DOMoveY(coinText.transform.position.y + moveUpDistance, duration).SetEase(Ease.OutQuad));
        textSequence.Join(coinText.DOFade(0, duration));

        // 2. �ı��Ķ�����ɺ������ı�����
        textSequence.OnComplete(() =>
        {
            Destroy(coinText.gameObject);
        });

    }
}
