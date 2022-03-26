using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliensBullet : MonoBehaviour
{
    public AlienArmy alienArmyCode;

    void Start() => //Invoke("AddSpeed",0.02f);
                    AddSpeed();
    void OnTriggerEnter2D(Collider2D collision)
    {
        string colidedObject = collision.gameObject.tag;
        if (colidedObject == "ScreenCollider")
        {
            if (gameObject)
                Destroy(gameObject);
        }
        else if (colidedObject == "Player")
        {
            //Destroy(collision.gameObject);
            if (gameObject)
                Destroy(gameObject);
            if(Player.lives > 1)
                Player.OnLosingLive?.Invoke();
            else if(Player.lives > 0)
                Player.OnDying?.Invoke();
        }
    }

    private void AddSpeed() => GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -alienArmyCode.bulletVel);

}