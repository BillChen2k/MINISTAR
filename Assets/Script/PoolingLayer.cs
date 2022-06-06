using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingLayer : MonoBehaviour
{

    public static PoolingLayer Instance { get; private set; }

    //边长
    public int sideLength = 26;

    //间距
    public float spacing = 10.0f;

    //正方体模板
    public GameObject cube;

    //产生的正方体
    GameObject[,] cubes;

    //开始的地点
    Vector3 beginLocalPos;

    public double[,,] poolingLayerValue;

    private void Awake()
    {
        Instance = this;

        //开始点
        float beginX = (-(spacing / 2.0f) - spacing * (sideLength / 2 - 1));
        float beginZ = ((spacing / 2.0f) + spacing * (sideLength / 2 - 1));
        beginLocalPos = new Vector3(beginX, 0, beginZ);

        cubes = new GameObject[sideLength, sideLength];
        poolingLayerValue = new double[5, sideLength, sideLength];


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
        
    }

    public void updateData()
    {
        ConvolutionLayer convolutionLayer = ConvolutionLayer.Instance;
        for (int k = 0; k < 5; k++)
        {
            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {
                    double result = convolutionLayer.convolutionLayerValue[k,i*3,j*3];
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            double temp = convolutionLayer.convolutionLayerValue[k, i * 3 + x, j * 3 + y];
                            if(temp>result)
                            {
                                result = temp;
                            }
                           
                        }
                    }
                    poolingLayerValue[k, i, j] = result;
                }
            }
        }

        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)

            {
                float temp =(float)(1 - poolingLayerValue[0, i, j]);
                cubes[i, j].GetComponent<Cube>().ChangeColor(new Color(temp, temp, temp, 1));
            }
        }


    }
}
