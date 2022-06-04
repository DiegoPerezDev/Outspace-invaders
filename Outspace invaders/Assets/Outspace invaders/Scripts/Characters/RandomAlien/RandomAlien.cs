using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAlien : MonoBehaviour
{
    public delegate void randomAlienBehaviour();
    public static randomAlienBehaviour OnRandomAlienDestroyed;
    public static RandomAlien instanceInScene;
    private static bool moveRight;
    public static float movementSpeed;

    void Start()
    {
        instanceInScene = this;
        moveRight = transform.position.x < Camera.main.transform.position.x;
        EnableMovement(true);
    }
    void OnBecameInvisible()
    {
        if (gameObject)
            Destroy(gameObject);
    }

    /// <summary>
    /// Make the alien move to the other side of the screen
    /// </summary>
    public static void EnableMovement(bool enabling)
    {
        if (!instanceInScene)
            return;
        Rigidbody2D rigidbody = instanceInScene.gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody == null)
            return;

        if (enabling)
        {
            if (moveRight)
                rigidbody.velocity = Vector2.right * movementSpeed;
            else
                rigidbody.velocity = Vector2.left * movementSpeed;
        }
        else
            rigidbody.velocity = Vector2.zero;
    }

}