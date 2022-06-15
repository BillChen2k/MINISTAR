using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;



public class ConvLayer
{
    public double[,,,] w;
    public double[] b;
}

public class DenseLayer
{
    public double[,] w;
    public double[] b;
}
public class DataParams
{
    public int epoch;
    public int batch;
    public float loss;
    public float accuracy;
    public double[,] input;
    public ConvLayer conv_layer;
    public DenseLayer dense_layer;
}

public class SingleDataFile
{
    public int begin_epoch;
    public int begin_batch;
    public int end_epoch;
    public int end_batch;
    public DataParams[] weights;
}

public class DataDepository : MonoBehaviour
{
    public static DataDepository Instance { get; private set; }

    private SingleDataFile singleDataFile;
    private int begin_epoch,begin_batch,end_epoch,end_batch;

    private void Awake()
    {
        Instance = this;
        begin_epoch = 0;
        begin_batch = 0;
        end_epoch = 0;
        end_batch = 19;
        singleDataFile = ReadFile(begin_epoch,begin_batch,end_epoch,end_batch);
    }

    public DataParams ReadParams(int epoch,int batch)
    {

        //这里面以0开始为索引
        epoch = epoch - 1;
        batch = batch - 1;
        if(epoch>=begin_epoch&&epoch<=end_epoch)
        {
            if(batch>=begin_batch&&batch<=end_batch)
            {
               
            }
            else
            {
                begin_batch += 20;
                end_batch += 20;
                singleDataFile = ReadFile(begin_epoch, begin_batch, end_epoch, end_batch);
            }
        }
        else
        {
            begin_epoch++;
            end_epoch++;
            begin_batch = 0;
            end_batch = 19;
            singleDataFile = ReadFile(begin_epoch, begin_batch, end_epoch, end_batch);
        }
        return singleDataFile.weights[batch%20];
    }

    //读取json文件
    private SingleDataFile ReadFile(int begin_epoch, int begin_batch, int end_epoch, int end_batch)
    {
        string jsonPath = Application.streamingAssetsPath + "/param_e"+begin_epoch+"b"+begin_batch+"_e"+end_epoch+"n"+end_batch+".json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError(jsonPath+"读取的文件不存在！");
            return null;
        }
        string json = File.ReadAllText(jsonPath);
        SingleDataFile jsonTemp = JsonConvert.DeserializeObject<SingleDataFile>(json);
        return jsonTemp;
    }    
}
