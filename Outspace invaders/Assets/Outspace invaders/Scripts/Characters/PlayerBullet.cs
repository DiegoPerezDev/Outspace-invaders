using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [HideInInspector] public Player playerThatShootThis;
    private readonly float timeForAutoDestruction = 3f;

    void Start()
    {
        StartCoroutine(AutoDestructionDelay());
        AddSpeed();
    }
    void OnDestroy() => playerThatShootThis.StartCoroutine(playerThatShootThis.DelayBeforeShootingAgain());
    void OnCollisionEnter2D(Collision2D collision)
    {
        string colidedObject = collision.gameObject.tag;
        if (colidedObject == "ScreenCollider")
        {
            if (gameObject)
                Destroy(gameObject);
        }
        else if (colidedObject == "Enemy")
        {
            if (gameObject)
                Destroy(gameObject);
            // Destroy game object
            Destroy(collision.gameObject);
            AlienArmy.OnAlienDestroyed?.Invoke();
            var AlienArmyCode = collision.gameObject.GetComponentInParent<AlienArmy>();
            AlienArmyCode.RemoveAlienFromArmy(collision.gameObject.GetComponent<Alien>());
            collision.gameObject.GetComponentInParent<AlienArmy_Movement>().SlowArmyByAlienKill();
        }
        else if (colidedObject == "EnemyAttack")
        {
            Destroy(collision.gameObject);
            if (gameObject)
                Destroy(gameObject);
        }
    }

    private void AddSpeed() => GetComponent<Rigidbody2D>().velocity = new Vector2(0f, playerThatShootThis.bulletVel);
    private IEnumerator AutoDestructionDelay()
    {
        //Delay
        var timer = 0f;
        while(timer < timeForAutoDestruction)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Destroy self gameObject
        if (gameObject)
            Destroy(gameObject);
    }

}