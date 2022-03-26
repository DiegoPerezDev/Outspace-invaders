using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        string colidedObject = collision.gameObject.tag;
        if (colidedObject == "PlayerAttack")
        {
            // Destroy bullet and self
            if (collision.gameObject)
                Destroy(collision.gameObject);
            if (gameObject)
                Destroy(gameObject);

            // Make the player gather point
            Player.OnGatheringScore?.Invoke();
        }
    }

}