using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public delegate void PlayerAchieventsDelegate();
    public static PlayerAchieventsDelegate OnLosingLive;
    public static int score, lives;
    public static readonly int scoreForKill = 20;
    public static readonly int startLives = 3;
    [HideInInspector] public TextMeshProUGUI scoreTMP;
    [HideInInspector] public bool movingRight, movingLeft, shootAttempt, canShoot = true;
    [Range(5f, 15f)]  public float bulletVel = 6f;
    [Range(0.1f, 1f)] 
    [SerializeField]  private float movementSpeed = 0.3f;
    [Range(0f, 0.5f)]   
    [SerializeField]  private float delayAfterBulletDestroyed = 0.2f;
    [SerializeField]  private GameObject bulletPref;
    private Rigidbody2D rigidBody;
    private Vector2 playerSize, startPosition;

    // - - - - MonoBehaviour Methods - - - -
    void OnEnable() => OnLosingLive += CheckLosingLifeBehaviour;
    void OnDisable() => OnLosingLive -= CheckLosingLifeBehaviour;
    void OnDestroy() => StopAllCoroutines();
    void Start()
    {
        // Get components
        rigidBody = GetComponent<Rigidbody2D>();
        playerSize = GetComponent<SpriteRenderer>().size;

        // Set start variable values
        var ui = GameObject.Find("UI");
        foreach (Transform child in ui.GetComponentsInChildren<Transform>())
        {
            var hudCode = child.GetComponent<UI_HUD>();
            if(hudCode != null)
            {
                scoreTMP = hudCode.scoreTMP;
                break;
            }
        }
        var levelCenter = (ScreenBounds.levelWidth / 2) + ScreenBounds.leftScreenBound;
        startPosition = new Vector2(levelCenter, rigidBody.position.y);
        rigidBody.position = startPosition;
        lives = startLives;
    }
    void Update()
    {
        if (shootAttempt)
        {
            shootAttempt = false;
            Shoot();
        }
    }
    void FixedUpdate() => Move();

    // - - - - Own Class Functions - - - -
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
        if (canShoot)
        {
            canShoot = false;
            var originPos = rigidBody.position + new Vector2(0f, (playerSize.y / 2) + (bulletPref.GetComponent<SpriteRenderer>().size.y / 2)) ;
            var bullet = Instantiate(bulletPref, originPos, Quaternion.identity);
            bullet.GetComponent<PlayerBullet>().playerThatShootThis = this;
        }
    }
    public IEnumerator DelayBeforeShootingAgain()
    {
        var timer = 0f;
        while(timer < delayAfterBulletDestroyed)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        canShoot = true;
    }
    private void RestartPlayer()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.position = startPosition;
        canShoot = true;
    }
    private void CheckLosingLifeBehaviour()
    {
        lives--;
        if (lives > 0)
            RestartPlayer();
        else if (lives >= 0)
            GameManager.RestartGame();
    }

}