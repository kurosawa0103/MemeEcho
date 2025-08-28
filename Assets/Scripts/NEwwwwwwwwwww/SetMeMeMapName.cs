using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetMeMeMapName : MonoBehaviour
{
    public TextMeshProUGUI memeMapNameText;
    public MapInfo mapInfo;

    void Start()
    {
        memeMapNameText.text = mapInfo.memeMapName;
    }

}
