using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliensBullet : MonoBehaviour
{
    [SerializeField] private float bulletVel = 6f;

    void Start() => AddSpeed();
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Every time the bullet collide anything, destroy the bullet and tell the alien attack script that it can now attack again
        if (gameObject)
            Destroy(gameObject);
        AlienArmy_AttackBehaviour.shooting = false;

        if (collision.gameObject.CompareTag("Player"))
            Player.OnLosingLive?.Invoke();
    }

    private void AddSpeed() => GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -bulletVel);

}