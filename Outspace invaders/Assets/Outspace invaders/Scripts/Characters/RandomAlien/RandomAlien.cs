using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAlien : MonoBehaviour
{
    public delegate void randomAlienBehaviour();
    public static randomAlienBehaviour OnRandomAlienDestroyed;
    public bool havePassedThroughScreen;

    void OnBecameInvisible()
    {
        if (!havePassedThroughScreen)
            havePassedThroughScreen = true;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Make the alien move to the other side of the screen
    /// </summary>
    public void AddStartMovement(bool moveRight, float movementSpeed)
    {
        Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody == null)
            return;
        if (moveRight)
            rigidbody.velocity = Vector2.right * movementSpeed;
        else
            rigidbody.velocity = Vector2.left * movementSpeed;
    }

}