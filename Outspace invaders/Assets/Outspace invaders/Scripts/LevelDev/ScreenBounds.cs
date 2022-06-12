using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This code gets the measures for the screen in the gameplay. We need this for limiting the movement of the characters and else.
/// </summary>
public class ScreenBounds : MonoBehaviour
{
    public static float rightLevelBoundBeforeHUD, leftScreenBound, aboveScreenBound, levelWidth;
    [SerializeField] private RectTransform HUD_Panel;

    void Awake()
    {
        leftScreenBound          = -(Camera.main.orthographicSize * 2 * Camera.main.aspect) / 2;
        rightLevelBoundBeforeHUD = -leftScreenBound - (HUD_Panel.rect.size.x * HUD_Panel.lossyScale.x);
        aboveScreenBound         = Camera.main.orthographicSize;
        levelWidth               = Mathf.Abs(leftScreenBound) + Mathf.Abs(rightLevelBoundBeforeHUD);
    }

}