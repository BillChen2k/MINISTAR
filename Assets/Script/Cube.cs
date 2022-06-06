using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    Material material;

    void Awake()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;
        material.SetColor("_Diffuse", new Color(1,1,1,1));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor(Color newColor)
    {
        material.SetColor("_Diffuse",newColor);
    }
}
