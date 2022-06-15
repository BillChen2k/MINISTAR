using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class painting : MonoBehaviour
{

    private RenderTexture texRender;   //����
    public Material mat;     //������shader�½�����
    public Texture brushTypeTexture;   //����������͸��
    private Camera mainCamera;
    private float brushScale = 100f;
    public Color brushColor = Color.black;
    public RawImage raw;                   //ʹ��UGUI��RawImage��ʾ������������UI
    private float lastDistance;
    private Vector3[] PositionArray = new Vector3[3];
    private int a = 0;
    private Vector3[] PositionArray1 = new Vector3[4];
    private int b = 0;
    private float[] speedArray = new float[4];
    private int s = 0;
    public int num = 50;
    public RectTransform contextRect;
    void Start()
    {
        texRender = new RenderTexture((int)contextRect.rect.width, (int)contextRect.rect.height, 24, RenderTextureFormat.ARGB32);
        Clear(texRender);
    }

    Vector3 startPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;

    bool TouchEnd()
    {
        Mouse mouse = Mouse.current; // ���
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
        Mouse mouse = Mouse.current; // ���
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
    void Update()
    {
        Pointer pointer = Pointer.current;
        if (TouchDown())
        {
            OnMouseMove(new Vector3(pointer.position.ReadValue().x, pointer.position.ReadValue().y, 0));
        }
        if (TouchEnd())
        {
            OnMouseUp();
        }
        DrawImage();
    }

    void OnMouseUp()
    {
        startPosition = Vector3.zero;
        //brushScale = 0.5f;
        a = 0;
        b = 0;
        s = 0;
    }
    //���û��ʿ��
    float SetScale(float distance)
    {
        float Scale = 0;
        if (distance < 100)
        {
            Scale = 0.8f - 0.005f * distance;
        }
        else
        {
            Scale = 0.425f - 0.00125f * distance;
        }
        if (Scale <= 0.05f)
        {
            Scale = 0.05f;
        }
        return Scale;
    }

    void OnMouseMove(Vector3 pos)
    {
        Pointer pointer = Pointer.current;
        if (startPosition == Vector3.zero)
        {
            startPosition = new Vector3(pointer.position.ReadValue().x, pointer.position.ReadValue().y, 0);
        }

        endPosition = pos;
        float distance = Vector3.Distance(startPosition, endPosition);
        brushScale = SetScale(distance);
        ThreeOrderB��zierCurse(pos, distance, 4.5f);

        startPosition = endPosition;
        lastDistance = distance;
    }

    void Clear(RenderTexture destTexture)
    {
        Graphics.SetRenderTarget(destTexture);
        GL.PushMatrix();
        GL.Clear(true, true, Color.white);
        GL.PopMatrix();
    }

    void DrawBrush(RenderTexture destTexture, int x, int y, Texture sourceTexture, Color color, float scale)
    {
        DrawBrush(destTexture, new Rect(x, y, sourceTexture.width, sourceTexture.height), sourceTexture, color, scale);
    }
    void DrawBrush(RenderTexture destTexture, Rect destRect, Texture sourceTexture, Color color, float scale)
    {
        float left = destRect.xMin - destRect.width * scale / 2.0f;
        float right = destRect.xMin + destRect.width * scale / 2.0f;
        float top = destRect.yMin - destRect.height * scale / 2.0f;
        float bottom = destRect.yMin + destRect.height * scale / 2.0f;

        Graphics.SetRenderTarget(destTexture);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetTexture("_MainTex", brushTypeTexture);
        mat.SetColor("_Color", color);
        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(left / contextRect.rect.width, top / contextRect.rect.height, 0);
        GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(right / contextRect.rect.width, top / contextRect.rect.height, 0);
        GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(right / contextRect.rect.width, bottom / contextRect.rect.height, 0);
        GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(left / contextRect.rect.width, bottom / contextRect.rect.height, 0);

        GL.End();
        GL.PopMatrix();
    }
    bool bshow = true;
    void DrawImage()
    {
        raw.texture = texRender;
    }
    public void OnClickClear()
    {
        Clear(texRender);
    }

    //���ױ���������
    public void TwoOrderB��zierCurse(Vector3 pos, float distance)
    {
        PositionArray[a] = pos;
        a++;
        if (a == 3)
        {
            for (int index = 0; index < num; index++)
            {
                Vector3 middle = (PositionArray[0] + PositionArray[2]) / 2;
                PositionArray[1] = (PositionArray[1] - middle) / 2 + middle;

                float t = (1.0f / num) * index / 2;
                Vector3 target = Mathf.Pow(1 - t, 2) * PositionArray[0] + 2 * (1 - t) * t * PositionArray[1] +
                                 Mathf.Pow(t, 2) * PositionArray[2];
                float deltaSpeed = (float)(distance - lastDistance) / num;
                DrawBrush(texRender, (int)target.x, (int)target.y, brushTypeTexture, brushColor, SetScale(lastDistance + (deltaSpeed * index)));
            }
            PositionArray[0] = PositionArray[1];
            PositionArray[1] = PositionArray[2];
            a = 2;
        }
        else
        {
            DrawBrush(texRender, (int)endPosition.x, (int)endPosition.y, brushTypeTexture,
                brushColor, brushScale);
        }
    }
    //���ױ��������ߣ���ȡ����4�������꣬ͨ�������м�2�����꣬�������֣���ʹ����num/1.5ʵ�ֻ����������ߣ���ʹ����ƽ��;ͨ���ٶȿ������߿�ȡ�
    private void ThreeOrderB��zierCurse(Vector3 pos, float distance, float targetPosOffset)
    {
        //��¼����
        PositionArray1[b] = pos;
        b++;
        //��¼�ٶ�
        speedArray[s] = distance;
        s++;
        if (b == 4)
        {
            Vector3 temp1 = PositionArray1[1];
            Vector3 temp2 = PositionArray1[2];

            //�޸��м���������
            Vector3 middle = (PositionArray1[0] + PositionArray1[2]) / 2;
            PositionArray1[1] = (PositionArray1[1] - middle) * 1.5f + middle;
            middle = (temp1 + PositionArray1[3]) / 2;
            PositionArray1[2] = (PositionArray1[2] - middle) * 2.1f + middle;

            for (int index1 = 0; index1 < num / 1.5f; index1++)
            {
                float t1 = (1.0f / num) * index1;
                Vector3 target = Mathf.Pow(1 - t1, 3) * PositionArray1[0] +
                                 3 * PositionArray1[1] * t1 * Mathf.Pow(1 - t1, 2) +
                                 3 * PositionArray1[2] * t1 * t1 * (1 - t1) + PositionArray1[3] * Mathf.Pow(t1, 3);
                //float deltaspeed = (float)(distance - lastDistance) / num;
                //��ȡ�ٶȲ�ֵ���������⣬�ο���
                float deltaspeed = (float)(speedArray[3] - speedArray[0]) / num;
                //float randomOffset = Random.Range(-1/(speedArray[0] + (deltaspeed * index1)), 1 / (speedArray[0] + (deltaspeed * index1)));
                //ģ��ë��Ч��
                float randomOffset = Random.Range(-targetPosOffset, targetPosOffset);
                DrawBrush(texRender, (int)(target.x + randomOffset), (int)(target.y + randomOffset), brushTypeTexture, brushColor, SetScale(speedArray[0] + (deltaspeed * index1)));
            }

            PositionArray1[0] = temp1;
            PositionArray1[1] = temp2;
            PositionArray1[2] = PositionArray1[3];

            speedArray[0] = speedArray[1];
            speedArray[1] = speedArray[2];
            speedArray[2] = speedArray[3];
            b = 3;
            s = 3;
        }
        else
        {
            DrawBrush(texRender, (int)endPosition.x, (int)endPosition.y, brushTypeTexture,
                brushColor, brushScale);
        }

    }
}
