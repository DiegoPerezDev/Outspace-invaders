using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBullet : MonoBehaviour
{
    private float bulletVel;
    [SerializeField] private bool invulnerable;
    [HideInInspector] public AlienArmy_AttackBehaviour alienArmy_AttackBehaviour;

    void Start()
    {
        bulletVel = alienArmy_AttackBehaviour.bulletSpeed;
        AddSpeed();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        var collidedObjectTag = collision.gameObject.tag;

        // Dont collide with enemies
        if (collidedObjectTag == "Enemy")
            return;

        // Auto destruction by collision
        if (!invulnerable)
            Destroy(gameObject);
        else if (collidedObjectTag != "PlayerBullet")
            Destroy(gameObject);
        AlienArmy_AttackBehaviour.shooting = false;

        // Specific effects when colliding with certain gameObjects
        if (collidedObjectTag == "Player")
            Player.OnLosingLive?.Invoke();
        else if (collidedObjectTag == "Barricade")
            Destroy(collision.gameObject);
    }

    private void AddSpeed() => GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -bulletVel);

}