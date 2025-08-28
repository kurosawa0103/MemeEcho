using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DisappearTip : MonoBehaviour
{
    public TextMeshProUGUI coinTextPrefab; // 用于显示硬币值的文本预制体
    public float moveUpDistance = 50f; // 硬币上升的距离
    public float duration = 5f; // 动画持续时间
    public Transform  parent; // 硬币文本的父级 Canvas

    private void Start()
    {
       
    }

    public void ShowWord()
    {
        // 创建并显示硬币值的文本，初始位置在硬币上方
        TextMeshProUGUI coinText = Instantiate(coinTextPrefab, parent);

        // 开始文本的动画
        Sequence textSequence = DOTween.Sequence();

        // 1. 文本逐渐向上移动并透明度逐渐变为0
        textSequence.Append(coinText.transform.DOMoveY(coinText.transform.position.y + moveUpDistance, duration).SetEase(Ease.OutQuad));
        textSequence.Join(coinText.DOFade(0, duration));

        // 2. 文本的动画完成后销毁文本对象
        textSequence.OnComplete(() =>
        {
            Destroy(coinText.gameObject);
        });

    }
}
