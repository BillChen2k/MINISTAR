using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{

    public Text speedUpText;

    public int times = 1;
    public int maxTimes = 512;

    // Start is called before the first frame update
    void Start()
    {
        speedUpText.text = "X " + times;
    }

    // Update is called once per frame
    void Update()
    {
        
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

        speedUpText.text = "X "+times;
    }
}
