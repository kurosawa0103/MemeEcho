using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class OpenMap : MonoBehaviour
{
    public bool mapIsOn = false;
    public bool isFading = false;
    public GameObject mapPanel;
    public Image mapBg;
    public List<Image> mapItems;
    public float fadeDuration = 1;

    //public SailorMap sailorMap;
    public GameObject redPoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMapPanel()
    {
        redPoint.SetActive(false);
        //sailorMap.RefreshMapItems();
        if (!mapIsOn && !isFading)
        {
            isFading = true;
            foreach (var renderer in mapItems)
            {
                renderer.DOFade(0f, 0);
            }
            mapBg.DOFade(0, 0);
            mapPanel.SetActive(true);
            // ���� parent2 ��������
            mapBg.DOFade (1, fadeDuration).OnComplete(() => {
                foreach (var renderer in mapItems)
                {
                    renderer.DOFade(1f, fadeDuration);
                }

                // �ȴ�������ɺ����� parent2����ʾ parent1��������
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    mapIsOn = true;
                    isFading = false;
                });
            });
        }
        else if(mapIsOn&& !isFading)
        {
            isFading = true;
            // ����������
            foreach (var renderer in mapItems)
            {
                renderer.DOFade(0f, fadeDuration);
            }

            // �ȴ�������ɺ����� parent2����ʾ parent1��������
            DOVirtual.DelayedCall(0.2f, () =>
            {
                mapBg.DOFade(0, fadeDuration).OnComplete(() => {                  

                    // �ȴ�������ɺ�������һ���л�
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        mapIsOn = false;
                        mapPanel.SetActive(false);
                        isFading = false;
                    });
                });             
            });
            
        }

    }

}
