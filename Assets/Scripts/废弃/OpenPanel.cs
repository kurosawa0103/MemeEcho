using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject ob;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenaPanel()
    {
        ob.SetActive(true);
    }
    public void ClosePanel()
    {
        ob.SetActive(false);
    }
}
