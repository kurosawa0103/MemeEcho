using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public enum ShadowType
{
    Object,
    Sprite,
}
public class WaterShadow : MonoBehaviour
{
    [LabelText("倒影类型")]
    public ShadowType shadowType;
    public float offsetX;
    public float offsetY;
    [LabelText("倒影是否生成在子目录")]
    public bool setChild;
    private GameObject go;
    private bool oneTime;
    private SpriteRenderer goSpriteRenderer;
    private SpriteRenderer selfSpriteRenderer;
    void Start()
    {

    }

    void Update()
    {
        switch (shadowType)
        {
            case ShadowType.Object:
                {
                    if(!oneTime)
                    {
                        go = Instantiate(gameObject);
                        go.name = this.name + "_shadow";
                        Destroy(go.GetComponent<WaterShadow>());
                        oneTime = true;
                    }
                    go.transform.position = new Vector3(offsetX - transform.position.x + 500, offsetY - transform.position.y, transform.position.z);

                    //设置子集的情况下需要将scale变为1
                    if (setChild)
                    {
                        go.transform.SetParent(transform);
                        go.transform.localScale = new Vector3(-1, -1, 1);
                    }
                    if (!setChild)
                    {
                        go.transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z);
                    }               
                }
                break;

            case ShadowType.Sprite:
                {
                    if (!oneTime)
                    {
                        go = new GameObject();
                        go.name = this.name + "_shadow";
                        goSpriteRenderer = go.AddComponent<SpriteRenderer>();
                        selfSpriteRenderer =this.GetComponent<SpriteRenderer>();
                        go.GetComponent<SpriteRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;   //同步影子sortinglayer
                        go.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder;           //同步影子sortinglayer
                        go.tag = gameObject.tag;
                        go.layer = gameObject.layer;
                        oneTime = true;
                    }
                    goSpriteRenderer.sprite = selfSpriteRenderer.sprite;

                    //设置子集的情况下需要将scale变为1
                    if (setChild)
                    {
                        go.transform.SetParent(transform);
                        go.transform.localScale = new Vector3(-1, -1, 1);
                    }
                    if(!setChild)
                    {
                        go.transform.localScale = new Vector3(-transform.localScale.x, -transform.localScale.y, transform.localScale.z);
                    }

                    go.transform.position = new Vector3(offsetX-transform.position.x + 500, offsetY - transform.position.y, transform.position.z);
                }

                break;
        }
    }
}
