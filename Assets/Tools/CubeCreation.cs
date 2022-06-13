using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeCreation : EditorWindow
{
    string nameStr = "123";
    string[] ctnName = new string[6];
    [MenuItem("Window/CreatCtns")]
    static void Apply()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        CubeCreation window = (CubeCreation)EditorWindow.GetWindowWithRect(typeof(CubeCreation), wr, true, "CreatCtns");
    }

    private void OnGUI()
    {
        nameStr = EditorGUILayout.TextField("Name: ", nameStr);
        this.Repaint();//实时刷新
        if (GUILayout.Button("生成"))
        {
            SetNames(true);
        }
        if (GUILayout.Button("close"))
        {
            this.Close();
        }
    }

    void SetNames(bool isReady)
    {
        GameObject go = null;
        ctnName[0] = "Prefab/bluectn";
        ctnName[1] = "Prefab/blue";
        ctnName[2] = "Prefab/redctn";
        ctnName[3] = "Prefab/red";
        ctnName[4] = "Prefab/greenctn";
        ctnName[5] = "Prefab/yellowctn";

        if (Selection.gameObjects != null)
        {
            foreach (var item in Selection.gameObjects)
            {
                int n = Random.Range(0, 5);
                //switch(n)           
                //{
                //    case 0:
                //        go = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/bluectn"))as GameObject;
                //        break;
                //    case 1:
                //        go = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/red")) as GameObject;
                //        break;
                //    case 2:
                //        go = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/blue")) as GameObject;
                //        break;
                //    case 3:
                //        go = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/greenctn")) as GameObject;
                //        break;
                //    case 4:
                //        go = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/redctn")) as GameObject;
                //        break;
                //    case 5:
                //        go = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/yellowctn")) as GameObject;
                //        break;
                //    default:
                //        break;
                //};
                go = PrefabUtility.InstantiatePrefab(Resources.Load(ctnName[n])) as GameObject;
                go.transform.parent = item.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}



