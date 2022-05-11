using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliensBullet : MonoBehaviour
{
    private float bulletVel = 6f;

    void Start() => AddSpeed();
    void OnTriggerEnter2D(Collider2D collision)
    {
        var collidedObjectTag = collision.gameObject.tag;

        // Dont collide with enemies or other bullets
        if (collidedObjectTag == "Enemy")
            return;

        // Collide with anything
        if (gameObject)
            Destroy(gameObject);
        AlienArmy_AttackBehaviour.shooting = false;

        if (collision.gameObject.CompareTag("Player"))
            Player.OnLosingLive?.Invoke();
    }

    private void AddSpeed() => GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -bulletVel);

}