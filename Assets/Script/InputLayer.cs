using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLayer: MonoBehaviour
{

    public static InputLayer Instance { get; private set; }

    //边长
    public int sideLength = 28;

    //间距
    public float spacing = 10.0f;

    //正方体模板
    public GameObject cube;

    //产生的正方体
    GameObject[,] cubes;

    //开始的地点
    Vector3 beginLocalPos;

    public double[,] intputValue;


    private void Awake()
    {
        Instance = this;

        //开始点
        float beginX = (-(spacing / 2.0f) - spacing * (sideLength / 2 - 1));
        float beginZ = ((spacing / 2.0f) + spacing * (sideLength / 2 - 1));
        beginLocalPos = new Vector3(beginX, 0, beginZ);

        cubes = new GameObject[sideLength, sideLength];
        intputValue = new double[sideLength, sideLength];

        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                GameObject newCube = Instantiate(cube);
                newCube.transform.parent = this.transform;
                newCube.transform.localPosition = new Vector3(beginLocalPos.x + j * spacing, 0, beginLocalPos.z - i * spacing);
                newCube.transform.rotation = this.transform.rotation;
                cubes[i, j] = newCube;
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

        //调整间距代码
        //float beginX = (-(spacing / 2.0f) - spacing * (sideLength / 2 - 1));
        //float beginZ = ((spacing / 2.0f) + spacing * (sideLength / 2 - 1));
        //beginLocalPos = new Vector3(beginX, 0, beginZ);

        //int count = 0;

        //for (int i = 0; i < sideLength; i++)
        //{
        //    for (int j = 0; j < sideLength; j++)
        //    {
        //        cubes[count++].transform.localPosition = new Vector3(beginLocalPos.x + j * spacing, 0, beginLocalPos.z - i * spacing);
        //    }
        //} 
    }


    //显示图像


    //测试使用
    ////读取
    //void readTestImages()
    //{
    //    testColors = new float[10, sideLength, sideLength];

    //    for(int k=0;k<10;k++)
    //    {
    //        Texture2D image = (Texture2D)Resources.Load(k.ToString()) as Texture2D;
    //        for (int i = 0; i < sideLength; i++)
    //        {
    //            for (int j = 0; j < sideLength; j++)
    //            {
    //                Color tempColor = image.GetPixel(j, sideLength - i);
    //                testColors[k, i, j] = tempColor.r * 0.299f + tempColor.g * 0.587f + tempColor.b * 0.114f;
    //            }
    //        }
    //    }
    //}

    public void inputImage(DataParams dataParams)
    {
        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                float temp = (float)(1 - dataParams.input[i, j]);
                intputValue[i, j] = dataParams.input[i, j];
                cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
            }

        }
    }

    public void handInput(double[,] inputData)
    {
        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                float temp = (float)(1 - inputData[i,j]);
                intputValue[i, j] = inputData[i,j];
                cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
            }
        }
    }
}