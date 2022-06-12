using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for both of the bullets shot by the aliens. 
/// <para> There is one bullet that can be destroyed by the player's bullet and other one that doesn't.</para>
/// </summary>
public class AlienBullet : MonoBehaviour
{
    [HideInInspector] public AlienArmy_AttackBehaviour alienArmy_AttackBehaviour;
    [SerializeField] private bool invulnerable;
    private float bulletVel;

    void Start()
    {
        bulletVel = alienArmy_AttackBehaviour.bulletSpeed;
        AddSpeed();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        var collidedObjectTag = collision.gameObject.tag;

        // Dont collide with the aliens
        if (collidedObjectTag == "Enemy")
            return;

        if (!invulnerable || (invulnerable && collidedObjectTag != "PlayerAttack"))
        {
            if(alienArmy_AttackBehaviour)
                alienArmy_AttackBehaviour.Shoot();
            Destroy(gameObject);
        }

        if (collidedObjectTag == "Player")
            Player.OnLosingLive?.Invoke();
        else if (collidedObjectTag == "Barricade")
            Destroy(collision.gameObject);
    }

    private void AddSpeed() => GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -bulletVel);

}