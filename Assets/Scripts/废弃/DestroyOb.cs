using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyObject11()
    {
        Destroy(gameObject);
    }
    public void DisableObject11()
    {
        gameObject.SetActive(false);
    }
}
