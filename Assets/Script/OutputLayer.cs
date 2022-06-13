using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputLayer : MonoBehaviour
{
    public static OutputLayer Instance { get; private set; }

    //边长
    public int sideLength = 10;

    //产生的正方体
    GameObject[] cubes;

    double[] outputValue;
    private void Awake()
    {
        Instance = this;

        cubes = new GameObject[sideLength];
        outputValue = new double[10];


        for (int i = 0; i < sideLength; i++)
        {
            //获取物体
            GameObject obj = this.transform.Find(i.ToString()).gameObject;
            cubes[i] = obj;
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
