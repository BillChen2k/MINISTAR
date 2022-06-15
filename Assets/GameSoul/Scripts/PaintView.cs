//-----------------------------------------------------------------------
// <copyright file="PaintView.cs" company="Codingworks Game Development">
//     Copyright (c) codingworks. All rights reserved.
// </copyright>
// <author> codingworks </author>
// <email> coding2233@163.com </email>
// <time> 2017-12-10 </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PaintView : MonoBehaviour
{
    #region 属性

    //绘图shader&material
    [SerializeField]
    private Shader _paintBrushShader;
    private Material _paintBrushMat;
    //清理renderTexture的shader&material
    [SerializeField]
    private Shader _clearBrushShader;
    private Material _clearBrushMat;
    //默认笔刷RawImage
    [SerializeField]
    private RawImage _defaultBrushRawImage;
    //默认笔刷&笔刷合集
    [SerializeField]
    private Texture _defaultBrushTex;
    //renderTexture
    private RenderTexture _renderTex;
    //默认笔刷RawImage
    [SerializeField]
    private Image _defaultColorImage;
    //绘画的画布
    [SerializeField]
    private RawImage _paintCanvas;
    //笔刷的默认颜色&颜色合集
    [SerializeField]
    private Color _defaultColor;
    //笔刷大小的slider
    private Text _brushSizeText;
    //笔刷的大小
    private float _brushSize;
    //笔刷的间隔大小
    private float _brushLerpSize;
    //默认上一次点的位置
    private Vector2 _lastPoint;
    public RectTransform contextRect;
    #endregion

    void Start()
	{
		InitData();
	}

	private void Update()
	{
		Color clearColor = new Color(0, 0, 0, 0);
		//if (Input.GetKeyDown(KeyCode.Space))
			//_paintBrushMat.SetColor("_Color", clearColor);
	}


	#region 外部接口

	public void SetBrushSize(float size)
    {
       _brushSize = size;
       _paintBrushMat.SetFloat("_Size", _brushSize);
    }

    public void SetBrushTexture(Texture texture)
    {
        _defaultBrushTex = texture;
        _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
        _defaultBrushRawImage.texture = _defaultBrushTex;
    }

    public void SetBrushColor(Color color)
    {
        _defaultColor = color;
        _paintBrushMat.SetColor("_Color", _defaultColor);
        _defaultColorImage.color = _defaultColor;
    }
    /// <summary>
    /// 选择颜色
    /// </summary>
    /// <param name="image"></param>
    public void SelectColor(Image image)
    {
        SetBrushColor(image.color);
    }
    /// <summary>
    /// 选择笔刷
    /// </summary>
    /// <param name="rawImage"></param>
    public void SelectBrush(RawImage rawImage)
    {
        SetBrushTexture(rawImage.texture);
    }
    /// <summary>
    /// 设置笔刷大小
    /// </summary>
    /// <param name="value"></param>
    public void BrushSizeChanged(Slider slider)
    {
      //  float value = slider.maxValue + slider.minValue - slider.value;
        SetBrushSize(Remap(slider.value,300.0f,30.0f));
        if (_brushSizeText == null)
        {
            _brushSizeText=slider.transform.Find("Background/Text").GetComponent<Text>();
        }
        _brushSizeText.text = slider.value.ToString("f2");
    }

    /// <summary>
    /// 拖拽
    /// </summary>
    public void DragUpdate()
    {
        if (_renderTex && _paintBrushMat)
        {
            if (TouchDown())
            {
                Pointer pointer = Pointer.current;
                LerpPaint(pointer.position.ReadValue());
            }
        }
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    public void DragEnd()
    {
        if (TouchEnd())
            _lastPoint = Vector2.zero;
    }

    #endregion

    #region 内部函数
	
    bool TouchEnd()
    {
        Mouse mouse = Mouse.current; // 鼠标
        Touchscreen touchScreen = Touchscreen.current;
        Pointer pointer = Pointer.current;
        if (touchScreen != null)
        {
            ReadOnlyArray<TouchControl> touches = touchScreen.touches;
            if (touches[0].phase.ReadValue() == TouchPhase.Ended)
            {
                return true;
            }
        }
        else if (mouse != null)
        {
            if (mouse.leftButton.wasReleasedThisFrame)
            {
                return true;
            }
        }
        return false;

    }
    bool TouchDown()
    {
        Mouse mouse = Mouse.current; // 鼠标
        Touchscreen touchScreen = Touchscreen.current;
        Pointer pointer = Pointer.current;
        if (touchScreen != null)
        {
            ReadOnlyArray<TouchControl> touches = touchScreen.touches;
            if (touches[0].phase.ReadValue() == TouchPhase.Began)
            {
                return true;
            }
            if (touches[0].phase.ReadValue() == TouchPhase.Moved)
            {
                return true;
            }
        }
        else if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                return true;
            }
            if (mouse.leftButton.isPressed)
            {
                return true;
            }
        }
        return false;
    }
    //初始化数据
    void InitData()
    {
        _brushSize = 10.0f;
        _brushLerpSize = (_defaultBrushTex.width + _defaultBrushTex.height) / 2.0f / _brushSize;
        _lastPoint = Vector2.zero;

        if (_paintBrushMat == null)
        {
            UpdateBrushMaterial();
        }
        if(_clearBrushMat==null)
        _clearBrushMat = new Material(_clearBrushShader);
        if (_renderTex == null)
        {
            _renderTex = RenderTexture.GetTemporary((int)contextRect.rect.width, (int)contextRect.rect.height, 24);
            _paintCanvas.texture = _renderTex;
        }
        Graphics.Blit(null, _renderTex, _clearBrushMat);
    }

    //更新笔刷材质
    private void UpdateBrushMaterial()
    {
        _paintBrushMat = new Material(_paintBrushShader);
        _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
        _paintBrushMat.SetColor("_Color", _defaultColor);
        _paintBrushMat.SetFloat("_Size", _brushSize);
    }

    //插点
    private void LerpPaint(Vector2 point)
    {
        Paint(point);

        if (_lastPoint == Vector2.zero)
        {
            _lastPoint = point;
            return;
        }

        float dis = Vector2.Distance(point, _lastPoint);
        if (dis > _brushLerpSize)
        {
            Vector2 dir = (point - _lastPoint).normalized;
            int num = (int)(dis / _brushLerpSize);
            for (int i = 0; i < num; i++)
            {
                Vector2 newPoint = _lastPoint + dir * (i + 1) * _brushLerpSize;
                Paint(newPoint);
            }
        }
        _lastPoint = point;
    }

    //画点
    private void Paint(Vector2 point)
    {
        Vector2 localPosition = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(contextRect, point, null, out localPosition);
        if (!contextRect.rect.Contains(localPosition))
        {
            return;
        }
        
        Vector2 uv = new Vector2((localPosition.x + (float)contextRect.rect.width * 0.5f)/ contextRect.rect.width,
            (localPosition.y + (float)contextRect.rect.height * 0.5f)/ contextRect.rect.height);
        Debug.Log(localPosition.x);
        Debug.Log(uv);
        _paintBrushMat.SetVector("_UV", uv);
        Graphics.Blit(_renderTex, _renderTex, _paintBrushMat);
    }
    /// <summary>
    /// 重映射  默认  value 为1-100
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    /// <returns></returns>
    private float Remap(float value, float startValue, float enValue)
    {
        float returnValue = (value - 1.0f) / (100.0f - 1.0f);
        returnValue = (enValue - startValue) * returnValue + startValue;
        return returnValue;
    }

    #endregion

}
