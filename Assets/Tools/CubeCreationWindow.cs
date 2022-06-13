using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeCreationWindow : EditorWindow
{
    InputLayerCreation inputLayerCreation = new InputLayerCreation();
    ConvolutionLayerCreation convolutionLayerCreation = new ConvolutionLayerCreation();
    PoolingLayerCreation poolingLayerCreation = new PoolingLayerCreation();
    OutputLayerCreation outputLayerCreation = new OutputLayerCreation();
    [MenuItem("Cube/Create")]
    static void CreateWindow()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        CubeCreationWindow window = (CubeCreationWindow)EditorWindow.GetWindowWithRect(typeof(CubeCreationWindow), wr, true, "CreateCubes");
    }

    private void OnGUI()
    {
        EditorGUILayout.TextArea("Input Layer");
        this.Repaint();//ʵʱˢ��
        if (GUILayout.Button("����"))
        {
           inputLayerCreation.CreateCubes();
        }

        EditorGUILayout.TextArea("Convolution Layer");
        if (GUILayout.Button("����"))
        {
            convolutionLayerCreation.CreateCubes();
        }
        EditorGUILayout.TextArea("Pooling Layer");
        if (GUILayout.Button("����"))
        {
            poolingLayerCreation.CreateCubes();
        }
        EditorGUILayout.TextArea("Output Layer");
        if (GUILayout.Button("����"))
        {
            outputLayerCreation.CreateCubes();
        }
        if (GUILayout.Button("clear"))
        {
            if (Selection.gameObjects != null)
            {
                foreach (GameObject goObj in Selection.gameObjects)
                {
                    List<Transform> list = new List<Transform>();
                    foreach (Transform child in goObj.transform)
                    {
                        list.Add(child);
                        Debug.Log(child.gameObject.name);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        DestroyImmediate(list[i].gameObject);
                    }
                }
            }

        }
        if (GUILayout.Button("close"))
        {
            this.Close();
        }
    }
}
