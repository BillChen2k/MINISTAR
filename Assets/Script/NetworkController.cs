using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour
{

    public static NetworkController Instance { get; private set; }

    public int epoch = 1;
    public int batch = 1;
    public int maxEpoch = 3;
    public int maxBtach = 400;
    public float accuracy = 0.0f;

    public Text epochText;
    public Text batchText;
    public Text accuracyText;

    //计时器
    public float timer = 1.0f;
    public int times = 1;
    public int maxTimes = 512;

    public float timeCount = 0.0f;

    bool stop = true;
    bool finished = false;

    //计算轮次

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        batchText.text =  "0 / " + maxBtach;
        epochText.text =  "0 / " + maxEpoch;
        accuracyText.text = (0 * 100.0f).ToString("F2") + "%";
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        if((!stop) &&(!finished))
        {
            timeCount += Time.deltaTime;
            if (timeCount >= timer)
            {
                updateAccuracyText();
                updateBatchText();
                updateEpochText();
                updateModel();
                if (batch == maxBtach)
                {
                    batch = 1;
                    epoch++;
                    if(epoch == maxEpoch)
                    {
                        endTraining();
                    }
                }
                else
                {
                    batch++;
                }
                timeCount = 0.0f;
            }

        }

    }

    public void stopTraining()
    {
        stop = true;
    }

    public void continueTraing()
    {
        stop = false;
    }

    public void endTraining()
    {
        batch = maxBtach;
        epoch = maxEpoch;
        finished = true;

        //使操作消失
        GameObject.Find("Operation").SetActive(false);

        //更新测试面板
        GameObject UIRoot = GameObject.Find("UI");
        UIRoot.transform.Find("TestPanel").gameObject.SetActive(true);


        GameObject.Find("TestPanel").SetActive(true);

        updateEpochText();
        updateBatchText();
        updateAccuracyText();
        updateModel();
    }

    public void speedUp()
    {
        if(times == maxTimes)
        {
            times = 1;
        }
        else
        {
            times *= 2;
        }

        timer = 1.0f / times;

    }

    void updateBatchText()
    {
        batchText.text = batch + " / " + maxBtach;
    }

    void updateEpochText()
    {
        epochText.text = epoch + " / " + maxEpoch;
    }

    void updateAccuracyText()
    {
        accuracy = DataDepository.Instance.ReadParams(epoch, batch).accuracy;
        accuracyText.text = (accuracy * 1.0f).ToString("F2") + "%";
    }

    void updateModel()
    {
        DataParams dataParams = DataDepository.Instance.ReadParams(epoch, batch);
        //更新输入层
        InputLayer.Instance.inputImage(dataParams);

        //更新卷积层
        ConvolutionLayer.Instance.updateData(dataParams.conv_layer);

        //更新池化层
        PoolingLayer.Instance.updateData();

        //更新输出
        OutputLayer.Instance.updateData(dataParams.dense_layer);
    }

    public void handInputHandler(double[,] inputData)
    {
        DataParams dataParams = DataDepository.Instance.ReadParams(epoch, batch);

        //更新输入层
        InputLayer.Instance.handInput(inputData);

        //更新卷积层
        ConvolutionLayer.Instance.updateData(dataParams.conv_layer);

        //更新池化层
        PoolingLayer.Instance.updateData();

        //更新输出
        OutputLayer.Instance.updateData(dataParams.dense_layer);
    }

}
