using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConvolutionLayerCreation : MonoBehaviour
{

    public float scale = 0.01f;
    public float cubeCount = 24;
    public float spacingRate = 0.5f;
    public string nameStr = "ConvolutionLayerCube";

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
                float beginZ = layerLength * 0.5f - size * 0.5f;
                Vector3 beginLocalPos = new Vector3(beginX, 0, beginZ);


                for (int i = 0; i < cubeCount; i++)
                {
                    for (int j = 0; j < cubeCount; j++)
                    {
                        GameObject newCube = Instantiate(go, goObj.transform);
                        newCube.transform.localScale = new Vector3(scale, scale, scale);
                        newCube.transform.parent = goObj.transform;
                        newCube.transform.localPosition = new Vector3(beginLocalPos.x + j * (spacing + size), 0, beginLocalPos.z - i * (spacing + size));
                        newCube.transform.rotation = goObj.transform.rotation;
                        newCube.name = i + "-" + j;
                    }
                }

            }
        }
    }
}
