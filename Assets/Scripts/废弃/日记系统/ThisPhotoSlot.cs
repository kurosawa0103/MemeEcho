using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisPhotoSlot : MonoBehaviour
{
    public PhotoData thisPhotoData;
    private PhotoSystem photoSystem;

    void Start()
    {
        photoSystem = FindObjectOfType<PhotoSystem>();
    }

    public void SelectPhoto()
    {
        photoSystem.currentPhotoData = thisPhotoData;
        photoSystem.UpdateCurrentPhoto();
    }
}
