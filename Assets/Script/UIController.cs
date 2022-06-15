using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public Transform modelTransform;
    public GameObject[] layerNames;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(modelTransform.localScale.x < 0.5f )
        {
            hideLayerName();
        }
        else
        {
            showLayerName();
        }
    }


    void showLayerName()
    {
        foreach(GameObject layerName in layerNames)
        {
            layerName.SetActive(true);
        }
    }

    void hideLayerName()
    {
        foreach (GameObject layerName in layerNames)
        {
            layerName.SetActive(false);
        }
    }
}
