using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int highScore, score, lives;
    public static readonly int scoreForKill = 20;
    public static readonly int startLives = 3;
    [HideInInspector] public bool movingRight, movingLeft, shootAttempt, canShoot = true;
    [Range(5f, 15f)]  public float bulletVel = 6f;
    [Range(0.1f, 1f)] [SerializeField] private float movementSpeed = 0.3f;
    [Range(0f, 0.5f)]   [SerializeField] private float delayAfterBulletDestroyed = 0.2f;
    [SerializeField] private GameObject bulletPref;
    private Rigidbody2D rigidBody;
    private Vector2 playerSize, startPosition;
    
    public delegate void PlayerAchieventsDelegate();
    public static PlayerAchieventsDelegate OnLosingLive, OnDying;


    void OnEnable()
    {
        OnLosingLive += RestartPlayer;
        OnDying += Death;
    }
    void OnDisable()
    {
        OnLosingLive -= RestartPlayer;
        OnDying -= Death;
    }
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerSize = GetComponent<SpriteRenderer>().size;
        canShoot = true;
        var levelCenter = (ScreenBounds.levelWidth / 2) + ScreenBounds.leftScreenBound;
        startPosition = new Vector2(levelCenter, rigidBody.position.y);
        rigidBody.position = startPosition;
        lives = startLives;
    }
    void Update() => Shoot();
    void FixedUpdate() => Move();
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(lives > 0)
        {
            if (collision.gameObject.CompareTag("Enemy"))
                Death();
        }
    }

    private void Move()
    {
        if (movingRight)
        {
            var nextPos = rigidBody.position + new Vector2(movementSpeed, 0);
            if (nextPos.x + playerSize.x/2 <= ScreenBounds.rightLevelBound)
                rigidBody.MovePosition(nextPos);
        }
        else if (movingLeft)
        {
            var nextPos = rigidBody.position - new Vector2(movementSpeed, 0);
            if (nextPos.x - playerSize.x/2 >= ScreenBounds.leftScreenBound)
                rigidBody.MovePosition(nextPos);
        }
    }
    private void Shoot()
    {
        if(shootAttempt)
        {
            shootAttempt = false;
            if (canShoot)
            {
                canShoot = false;
                var holi = rigidBody.position + new Vector2(0f, (playerSize.y / 2) + (bulletPref.GetComponent<SpriteRenderer>().size.y / 2)) ;
                var bullet = Instantiate(bulletPref, holi, Quaternion.identity);
                bullet.GetComponent<PlayerBullet>().playerThatShootThis = this;
            }
        }
    }
    public IEnumerator DelayBeforeShootingAgain()
    {
        // Delay
        var timer = 0f;
        while(timer < delayAfterBulletDestroyed)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Can shoot again
        canShoot = true;
    }
    private void RestartPlayer()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.position = startPosition;
    }
    private void Death()
    {
        if (lives != 0)
        {
            print("Player dead");
            lives = 0;
            GameManager.RestartGame();
        }
    }

}