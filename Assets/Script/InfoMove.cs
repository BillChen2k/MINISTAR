using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMove : MonoBehaviour
{

    public Transform cubePosition;
    public Camera mainCamera;
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vPos = mainCamera.WorldToScreenPoint(cubePosition.position);
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 oldPos = rectTransform.localPosition;
        rectTransform.localPosition = new Vector3(oldPos.x, GetPosition(vPos).y, oldPos.z);
    }


    private Vector3 GetPosition(Vector3 ScreeenPos)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            ScreeenPos, canvas.worldCamera, out pos);
        return new Vector3(pos.x, pos.y, 0);
    }
}
