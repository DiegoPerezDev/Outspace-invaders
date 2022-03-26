using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScreenBounds : MonoBehaviour
{
    public static float rightLevelBound, leftScreenBound, upScreenBound, downScreenBound, levelWidth;
    [SerializeField] private RectTransform HUD_Panel;

    void Awake()
    {
        leftScreenBound = -(Camera.main.orthographicSize * 2 * Camera.main.aspect) / 2;
        rightLevelBound = -leftScreenBound - (HUD_Panel.rect.size.x * HUD_Panel.lossyScale.x);
        upScreenBound = Camera.main.orthographicSize;
        downScreenBound = upScreenBound;
        levelWidth = Mathf.Abs(leftScreenBound) + Mathf.Abs(rightLevelBound);
    }

    void Start()
    {
        
    }

}