using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLayer: MonoBehaviour
{

    public static InputLayer Instance { get; private set; }

    //边长
    public int cubeCount = 28;

    //产生的正方体
    GameObject[,] cubes;

    public double[,] intputValue;


    private void Awake()
    {
        Instance = this;
        cubes = new GameObject[cubeCount, cubeCount];
        intputValue = new double[cubeCount, cubeCount];

        for (int i = 0; i < cubeCount; i++)
        {
            for (int j = 0; j < cubeCount; j++)
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


    //显示图像


    //测试使用
    ////读取
    //void readTestImages()
    //{
    //    testColors = new float[10, cubeCount, cubeCount];

    //    for(int k=0;k<10;k++)
    //    {
    //        Texture2D image = (Texture2D)Resources.Load(k.ToString()) as Texture2D;
    //        for (int i = 0; i < cubeCount; i++)
    //        {
    //            for (int j = 0; j < cubeCount; j++)
    //            {
    //                Color tempColor = image.GetPixel(j, cubeCount - i);
    //                testColors[k, i, j] = tempColor.r * 0.299f + tempColor.g * 0.587f + tempColor.b * 0.114f;
    //            }
    //        }
    //    }
    //}

    public void inputImage(DataParams dataParams)
    {
        for (int i = 0; i < cubeCount; i++)
        {
            for (int j = 0; j < cubeCount; j++)
            {
                float temp = (float)(1 - dataParams.input[i, j]);
                intputValue[i, j] = dataParams.input[i, j];
                cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
            }

        }
    }

    public void handInput(double[,] inputData)
    {
        for (int i = 0; i < cubeCount; i++)
        {
            for (int j = 0; j < cubeCount; j++)
            {
                float temp = (float)(1 - inputData[i,j]);
                intputValue[i, j] = inputData[i,j];
                cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
            }
        }
    }
}