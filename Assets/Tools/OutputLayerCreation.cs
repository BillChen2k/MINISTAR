using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class OutputLayerCreation : MonoBehaviour
{
    public float scale = 0.01f;
    public float cubeCount = 10;
    public float spacingRate = 0.5f;
    public string nameStr = "OutputLayerCube";

    public void CreateCubes()
    {
        GameObject go = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + nameStr + ".prefab", typeof(GameObject)) as GameObject;
        float size = go.GetComponent<Renderer>().bounds.size.x * 0.01f;
        float layerLength = size * cubeCount + (cubeCount - 1) * size * spacingRate;
        float spacing = spacingRate * size;
        if (Selection.gameObjects != null)
        {
            foreach (GameObject goObj in Selection.gameObjects)
            {


                //¿ªÊ¼µã
                float beginX = -layerLength * 0.5f + size * 0.5f;
           
                Vector3 beginLocalPos = new Vector3(beginX, 0, 0);


                for (int i = 0; i < cubeCount; i++)
                {
                    
                        GameObject newCube = Instantiate(go, goObj.transform);
                        newCube.transform.localScale = new Vector3(scale, scale, scale);
                        newCube.transform.parent = goObj.transform;
                        newCube.transform.localPosition = new Vector3(beginLocalPos.x + i * (spacing + size), 0, beginLocalPos.z);
                        newCube.transform.rotation = goObj.transform.rotation;
                        newCube.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = i.ToString();
                        newCube.name = i.ToString();
                    }
                

            }
        }
    }
}
