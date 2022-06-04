using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public delegate void PlayersDelegates();
    public static PlayersDelegates OnLosingLive;
    public static int score, lives, highScore;
    public static readonly int scoreForKill = 20;
    public static readonly int startLives = 3;
    [HideInInspector] public TextMeshProUGUI scoreTMP;
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public bool movingRight, movingLeft, shootAttempt, canShoot = true;
    [Range(5f, 15f)] public float bulletVel = 6f;

    [Range(0.1f, 1f)][SerializeField] private float movementSpeed = 0.3f;
    [Range(0f, 0.5f)][SerializeField] private float delayAfterBulletDestroyed = 0.2f;
    [SerializeField] private HUD hudCode;
    [SerializeField]  private GameObject bulletPref, losePanel;
    [SerializeField] private AlienArmy alienArmy;
    private Vector2 playerSize, startPosition;
    public static readonly float restartDelay = 4f;
    private Vector2 bulletSize;

    // - - - - MonoBehaviour Methods - - - -
    void OnEnable()  => OnLosingLive += CheckLosingLifeBehaviour;
    void OnDisable() => OnLosingLive -= CheckLosingLifeBehaviour;
    void OnDestroy() 
    {
        StopAllCoroutines();
        score = 0;
    }
    void Start()
    {
        // Get components
        rigidBody = GetComponent<Rigidbody2D>();
        playerSize = GetComponent<SpriteRenderer>().size * transform.lossyScale;
        bulletSize = bulletPref.GetComponent<SpriteRenderer>().size;

        // Set start variable values
        losePanel.SetActive(false);
        scoreTMP = hudCode.scoreTMP;
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
            var originPos = rigidBody.position + new Vector2(0f, (playerSize.y / 2) + (bulletSize.y / 2)) ;
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
    private void RestartPlayerPhysics()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.position = startPosition;
    }
    private void CheckLosingLifeBehaviour()
    {
        lives--;
        if (lives > 0)
            StartCoroutine(LosingLife());
        else if (lives >= 0)
            GameManager.OnLoseGame?.Invoke();
    }
    private IEnumerator LosingLife()
    {
        // Enabel losing panel for afordance
        losePanel.SetActive(true);

        LevelManager.LevelFreeze(true);

        // Delay before restarting
        var timer = 0f;
        while(timer < restartDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Restart player
        RestartPlayerPhysics();                     
        canShoot = true;

        // Disable losing panel for afordance
        losePanel.SetActive(false);

        LevelManager.LevelFreeze(true);
    }
    
}