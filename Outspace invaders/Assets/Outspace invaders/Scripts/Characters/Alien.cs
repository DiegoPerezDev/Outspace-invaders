using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    [HideInInspector] public Vector2Int position, firstPosition;
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public bool isAlive; 

    public void SetAlien(Vector2Int position, Rigidbody2D rigidBody)
    {
        this.position = position;
        this.rigidBody = rigidBody;
        isAlive = true;
        firstPosition = this.position;
    }

}