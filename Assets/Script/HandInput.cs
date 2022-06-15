using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class HandInput : MonoBehaviour
{

    private GameObject clone;
    private LineRenderer line;
    int i;

    //带有LineRender物体
    public GameObject target;


    //捕捉方形
    public RectTransform contextRect;
    public Camera mainCamera;
    public RawImage handwriting;

    private void Awake()
    {


    }
    void Start()
    {
        Debug.Log("请开始写字");

    }

    void initLine()
    {
        //实例化对象
        clone = (GameObject)Instantiate(target, target.transform.position, Quaternion.identity);
        clone.transform.parent = this.transform;
        //获得该物体上的LineRender组件
        line = clone.GetComponent<LineRenderer>();
        //设置起始和结束的颜色
        line.startColor = Color.white;
        line.endColor = Color.white;
        //设置起始和结束的宽度
        line.startWidth = 0.6f;
        line.endWidth = 0.4f;
        //计数
        i = 0;
    }

    void addLineVertex(Vector2 point)
    {
        //每一帧检测，按下鼠标的时间越长，计数越多
        i++;
        //设置顶点数
        line.positionCount++;
        //Vector2 mousePosition = mouse.position.ReadValue();
        //Vector2 mousePosition = pointer.position.ReadValue();
        //Debug.LogError(mousePosition);
        //设置顶点位置(顶点的索引，将鼠标点击的屏幕坐标转换为世界坐标)
        Vector2 localPosition = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(contextRect, point, null, out localPosition);
        if (contextRect.rect.Contains(localPosition))
        {
            line.SetPosition(i - 1, Camera.main.ScreenToWorldPoint(new Vector3(point.x, point.y, 15)));
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current; // 鼠标
        Touchscreen touchScreen = Touchscreen.current;
        Pointer pointer = Pointer.current;
        if (touchScreen != null)
        {
            ReadOnlyArray<TouchControl> touches = touchScreen.touches;
            if (touches[0].phase.ReadValue() == TouchPhase.Began)
            {
                //initLine();
            }
            if (touches[0].phase.ReadValue() == TouchPhase.Moved)
            {
                //addLineVertex(pointer.position.ReadValue());
            }
        }
        else if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                //initLine();
            }
            if (mouse.leftButton.isPressed)
            {
                //addLineVertex(pointer.position.ReadValue());
            }
        }

    }


    public void clear()
    {
        CaptureCamera();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }


    Texture2D CaptureCamera()
    {
        Rect rect = contextRect.rect;
        Debug.Log(rect);
        // 创建一个RenderTexture对象
        RenderTexture rt = new RenderTexture((int)Screen.width, (int)Screen.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        mainCamera.targetTexture = rt;
        mainCamera.Render();

        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
        //camera2.targetTexture = rt;
        // camera2.Render();
        //ps: -------------------------------------------------------------------

        // 激活这个rt, 并从中中读取像素。
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(Screen.width*0.5f - rect.width*0.5f,
                                        Screen.height * 0.5f - rect.height * 0.5f,
                                        rect.width ,
                                        rect.width), 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();

        processTexture2D(screenShot);

        // 重置相关参数，以使用camera继续在屏幕上显示
        mainCamera.targetTexture = null;
        // camera2.targetTexture = null;
        //ps: camera2.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        GameObject.Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + string.Format("/Screenshot/Screenshot_.png");
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));
        return screenShot;
    }


    //图像处理算法
    void processTexture2D(Texture2D texture)
    {
        int width = (int)texture.width;
        int height = (int)texture.height;

        //int minX = 0;
        //int maxX = width;
        //int minY = 0;
        //int maxY = height;

        //Debug.Log(Color.white);
        //Debug.Log(texture.GetPixel(100, 100));
        ////寻找最小的y
        //for (int y=0;y<height;y++)
        //{
        //    bool test = false;
        //    for(int x=0;x<width;x++)
        //    {
        //        Color temp = texture.GetPixel(x, y);

        //        if(temp == Color.white)
        //        {

        //            minY = y;
        //            test = true;
        //            break;
        //        }
        //    }
        //    if(test)
        //    {
        //        break;
        //    }
        //}

        ////寻找最大的y
        //for (int y = height-1; y >= 0 ; y--)
        //{
        //    bool test = false;
        //    for (int x = 0; x < width; x++)
        //    {
        //        Color temp = texture.GetPixel(x, y);
        //        if (temp == Color.white)
        //        {
        //            maxY = y;
        //            test = true;
        //            break;
        //        }
        //    }
        //    if (test)
        //    {
        //        break;
        //    }
        //}

        ////寻找最小的x
        //for (int x = 0; x < width; x++)
        //{
        //    bool test = false;
        //    for (int y = 0; y < height; y++)
        //    {
        //        Color temp = texture.GetPixel(x, y);
        //        if (temp == Color.white)
        //        {
        //            minX = x;
        //            test = true;
        //            break;
        //        }
        //    }
        //    if (test)
        //    {
        //        break;
        //    }
        //}

        ////寻找最大的x
        //for (int x = width-1; x >= 0; x--)
        //{
        //    bool test = false;
        //    for (int y = 0; y < height; y++)
        //    {
        //        Color temp = texture.GetPixel(x, y);
        //        if (temp == Color.white)
        //        {
        //            maxX = x;
        //            test = true;
        //            break;
        //        }
        //    }
        //    if (test)
        //    {
        //        break;
        //    }
        //}

        //Debug.Log(minX+" "+maxX+" "+minY+" "+maxY);

        int finalLength = width > height ? width : height;
        Texture2D processedTexture = new Texture2D(finalLength,finalLength, TextureFormat.RGB24, true);

        for(int i=0;i<finalLength;i++)
        {
            for(int j=0;j<finalLength;j++)
            {
                processedTexture.SetPixel(i, j, texture.GetPixel(i, j)==Color.white? Color.white:Color.black);
            }
        }

        double[,] inputImage = new double[28, 28];

        int minX, minY, maxX, maxY;
        int stride = finalLength / 28;
        for(int i=0;i<28;i++)
        {
            for(int j=0;j<28;j++)
            {
                minX = i * stride;
                minY = j * stride;
                maxX = (i + 1) * stride  > finalLength ? (finalLength) : ((i + 1) * stride);
                maxY = (j + 1) * stride  > finalLength ? (finalLength) : ((j + 1) * stride);

                int count = 0;

                for(int x=minX;x<maxX;x++)
                {
                    for(int y=minY;y<maxY;y++)
                    {
                        if (processedTexture.GetPixel(x, y) == Color.white)
                        {
                            count = 1;
                        }
                    }
                }

                inputImage[27 - j, i] = count;


            }
        }

        //InputLayer inputLayer = InputLayer.Instance;
        //inputLayer.handInput(inputImage);
        NetworkController.Instance.handInputHandler(inputImage);
        //processedTexture.Resize(28,28);
        // 最后将这些纹理数据，成一个png图片文件
        byte[] bytes = processedTexture.EncodeToPNG();
        string filename = Application.dataPath + string.Format("/Screenshot/procfeScreenshot_.png");
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));
    }
}
