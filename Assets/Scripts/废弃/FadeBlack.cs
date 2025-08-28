using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FadeBlack : MonoBehaviour
{
    public Image blackScreenImage;
    public GameObject blackOb;
    public float fadeOutTime;

    void Start()
    {
        blackOb.SetActive(true);
        blackScreenImage.DOFade(0, fadeOutTime).OnComplete(() =>
        {
            blackOb.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
