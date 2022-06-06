using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputLayer : MonoBehaviour
{
    public static OutputLayer Instance { get; private set; }

    //边长
    public int sideLength = 10;

    //间距
    public float spacing = 10.0f;

    //正方体模板
    public GameObject cube;
    //产生的正方体
    GameObject[] cubes;

    //开始的地点
    Vector3 beginLocalPos;

    double[] outputValue;
    private void Awake()
    {
        Instance = this;

        //开始点
        float beginX = (-(spacing / 2.0f) - spacing * (sideLength / 2 - 1));
        float beginZ = 0;
        beginLocalPos = new Vector3(beginX, 0, beginZ);

        cubes = new GameObject[sideLength];
        outputValue = new double[10];


        for (int i = 0; i < sideLength; i++)
        {
            GameObject newCube = Instantiate(cube);
            newCube.transform.parent = this.transform;
            newCube.transform.localPosition = new Vector3(beginLocalPos.x+ i* spacing, 0, beginLocalPos.z);
            newCube.transform.rotation = this.transform.rotation;
            newCube.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = i.ToString();
            cubes[i] = newCube;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateData(DenseLayer denseLayer)
    {
        PoolingLayer poolingLayer = PoolingLayer.Instance;
        double[] input = new double[320];
        int index = 0;


        for(int k=0;k<5;k++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    input[index] = poolingLayer.poolingLayerValue[k, j, i];
                    //Debug.Log(input[index]);
                    index++;

                }
            }
        }


        for(int k=0;k<10;k++)
        {
            outputValue[k] = 0;
            for(int i=0;i<320;i++)
            {
                outputValue[k] += input[i] * denseLayer.w[k, i];
            }
            outputValue[k] += denseLayer.b[k];
        }

        int result = 0;
        for (int j = 0; j < 10; j++)
        {

            if(outputValue[j]>outputValue[result])
            {
                result = j;
            }
            cubes[j].GetComponent<Cube>().ChangeColor(new Color(1, 1, 1, 1));
        }



        float temp = (float)(1 - outputValue[result]);
        cubes[result].GetComponent<Cube>().ChangeColor(new Color(0, 0, 0, 1));

    }
}
