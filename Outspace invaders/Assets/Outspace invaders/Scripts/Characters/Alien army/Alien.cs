using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    [HideInInspector] public Vector2Int position;
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public bool isAlive;
    private static bool losing;

    public void SetAlien(Vector2Int position, Rigidbody2D rigidBody)
    {
        this.position = position;
        this.rigidBody = rigidBody;
        isAlive = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var collidedGameObjectTag = collision.gameObject.tag;
         if (collidedGameObjectTag == "Barricade") 
            Destroy(collision.gameObject);
         else if(collidedGameObjectTag == "Player" || collidedGameObjectTag == "ScreenCollider")
         {
            if(!losing)
            {
                losing = true;
                GameManager.OnLoseGame?.Invoke();
            }
         }
    }

}