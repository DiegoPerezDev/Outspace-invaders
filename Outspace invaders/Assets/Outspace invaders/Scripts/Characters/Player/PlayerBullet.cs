using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the bullet that the player shoots. It only shoot one at a time and it last till it get's destroyed.
/// </summary>
public class PlayerBullet : MonoBehaviour
{
    [HideInInspector] public Player playerThatShootThis;
    private readonly float timeForAutoDestruction = 3f;
    private bool destroyingBarricade;
    private float brickHeight;
    private int brickLayer = 6;

    void Start()
    {
        StartCoroutine(AutoDestructionDelay());
        AddSpeed();
    }
    void OnDestroy() => StopAllCoroutines();
    void OnCollisionEnter2D(Collision2D collision)
    {
        string colidedObject = collision.gameObject.tag;
        if (gameObject)
            Destroy(gameObject);
        if (colidedObject == "Enemy")
        {
            Destroy(collision.gameObject);
            AlienArmy.OnAlienDestroyed?.Invoke();
            AlienArmy.RemovingAlien(collision.gameObject.GetComponent<Alien>());
            playerThatShootThis.DelayBeforeShootingAgain(AlienArmy.enemyDyingDelay);
        }
        else
            playerThatShootThis.DelayBeforeShootingAgain(); 

        if(colidedObject == "RandomEnemy")
        {
            Destroy(collision.gameObject);
            HUD.AddRandomScore();
        }
        else if (colidedObject == "Barricade")
        {
            if(!destroyingBarricade)
            {
                destroyingBarricade = true;
                //Destroy(collision.contacts[collision.contacts.Length - 1].collider.gameObject);
                if (brickHeight <= 0) GetBrickHeight(collision.gameObject);
                CheckForBricksUnder(collision.gameObject);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject)
            Destroy(gameObject);
        playerThatShootThis.DelayBeforeShootingAgain();
        if (collision.CompareTag("DestroyableEnemyAttack"))
            Destroy(collision.gameObject);
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

    private void GetBrickHeight(GameObject brick) => brickHeight = brick.GetComponent<SpriteRenderer>().size.y * brick.transform.lossyScale.y;
    private void CheckForBricksUnder(GameObject brick)
    {
        var brickLayerMask = 1 << brickLayer;
        brick.layer = 0;
        var Ray = Physics2D.Raycast(brick.transform.position, Vector2.down, brickHeight, brickLayerMask);
        if (Ray.collider != null)
        {
            brick.layer = brickLayer;
            CheckForBricksUnder(Ray.transform.gameObject);
        }
        else if(brick)
            Destroy(brick);
    }

}