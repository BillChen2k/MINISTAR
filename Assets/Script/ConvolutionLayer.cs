using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvolutionLayer : MonoBehaviour
{

    public static ConvolutionLayer Instance { get; private set; }

    //边长
    public int sideLength = 24;

    //产生的正方体
    GameObject[,] cubes;

    public int index = 0;

    public double[,,] convolutionLayerValue;

    private void Awake()
    {
        Instance = this;

        cubes = new GameObject[sideLength, sideLength];
        convolutionLayerValue = new double[5, sideLength, sideLength];

        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                //获取物体
                GameObject obj = this.transform.Find(i + "-" + j).gameObject;
                cubes[i, j] = obj;
            }
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

    public void updateData(ConvLayer convLayer)
    {
        InputLayer inputLayer = InputLayer.Instance;
        for(int k=0;k<5;k++)
        {
            for(int i=0;i<sideLength;i++)
            {
                for(int j=0;j<sideLength;j++)
                {
                    convolutionLayerValue[k, i, j] = 0;
                    for(int x=0;x<5;x++)
                    {
                        for(int y=0;y<5;y++)
                        {

                            convolutionLayerValue[k, i, j] += inputLayer.intputValue[i+x,j+y] * convLayer.w[k, 0,x, y];
                        }
                    }
                    convolutionLayerValue[k, i, j] += convLayer.b[k];
                }
            }
        }


        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                //Debug.Log(convolutionLayerValue[0, i, j]);
                float temp = 1-(float)(convolutionLayerValue[0, i, j]);
                //temp = Mathf.Clamp(temp, 0, 1);
                cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
            }
        }

        //float maxTemp = float.MinValue;
        //float minTemp = float.MaxValue;
        //for (int i = 0; i < sideLength; i++)
        //{
        //    for (int j = 0; j < sideLength; j++)
        //    {
        //        //Debug.Log(convolutionLayerValue[0, i, j]);
        //        float temp = (float)(convolutionLayerValue[0, i, j]);

        //        if (temp > maxTemp) maxTemp = temp;
        //        if (temp < minTemp) minTemp = temp;
        //        //temp = Mathf.Clamp(temp, 0, 1);
        //        //cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
        //    }
        //}



        //Debug.Log(maxTemp+" " +minTemp);

        //for (int i = 0; i < sideLength; i++)
        //{
        //    for (int j = 0; j < sideLength; j++)
        //    {
        //        float temp = ((float)convolutionLayerValue[0, i, j] - minTemp) / (maxTemp - minTemp);

        //        if(temp==0)
        //        {
        //            Debug.Log(temp);
        //        }

        //        cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
        //    }
        //}
    }
}
