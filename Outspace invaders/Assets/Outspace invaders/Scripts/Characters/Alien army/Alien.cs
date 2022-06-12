using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for each alien of the alien army.
/// </summary>
public class Alien : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public bool isAlive;
    private static bool losing;

    /// <summary>
    /// Kind of a constructor for each alien.
    /// </summary>
    public void SetAlien(Vector2Int gridPosition, Rigidbody2D rigidBody)
    {
        this.gridPosition = gridPosition;
        this.rigidBody = rigidBody;
        isAlive = true;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        var collidedGameObjectTag = collision.gameObject.tag;

        // Destroy the barricade part when colliding with them
         if (collidedGameObjectTag == "Barricade") 
            Destroy(collision.gameObject);

         // If any alien collides with the bottom collider or the player, then the user loses.
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