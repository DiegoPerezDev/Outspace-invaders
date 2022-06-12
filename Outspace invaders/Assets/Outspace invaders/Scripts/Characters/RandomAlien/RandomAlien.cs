using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the enemy that comes time to time in the upper side of the screen and gives a random amount of points.
/// </summary>
public class RandomAlien : MonoBehaviour
{
    public static RandomAlien instanceInScene;
    public static float movementSpeed;
    private static bool moveRight;

    void OnEnable() => GameManager.OnLevelPauseByFreezing += DisableMovement;
    void OnDisable() => GameManager.OnLevelPauseByFreezing -= DisableMovement;
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
    /// Make the alien move to the countrary side of the screen on which the alien was spawned
    /// </summary>
    private static void EnableMovement(bool enabling)
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
    private static void DisableMovement(bool disabling) => EnableMovement(!disabling);
}