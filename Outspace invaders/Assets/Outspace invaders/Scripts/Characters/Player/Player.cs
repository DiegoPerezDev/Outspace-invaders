using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// All of the player mechanics are in this one script. For the bullet shot by the player there is another script.
/// </summary>
public class Player : MonoBehaviour
{
    // General
    private Vector2 playerSize;

    // Life 
    public delegate void PlayersDelegates();
    public static PlayersDelegates OnLosingLive;
    public static readonly int startLives = 3;
    public static int lives;
    [SerializeField] private GameObject loseLivePanel;
    [SerializeField] private bool immortal;
    private static readonly float restartDelay = 2.5f;
    private Vector2 startPosition;

    // Score
    public static int score, highScore;
    public static readonly int scoreForKill = 20;
    [HideInInspector] public TextMeshProUGUI scoreTMP;

    // Movement
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public bool movingRight, movingLeft;
    [Range(0.05f, 0.2f)][SerializeField] private float movementSpeed = 0.1f;
    [SerializeField] private HUD hudCode;

    // Attack
    [HideInInspector] public bool shootAttempt, canShoot = true;
    [Range(5f, 15f)] public float bulletVel = 6f;
    public GameObject barricadeBrick;
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private float shootDelay;
    private Vector2 bulletSize;


    // - - - - MonoBehaviour Methods - - - -
    void OnEnable()
    {
        GameManager.OnSceneLoaded += SetPlayerInputsCallbacks;
        GameManager.OnLoseGame    += RemovePlayerInputsCallbacks;
        GameManager.OnWinGame     += RemovePlayerInputsCallbacks;
        OnLosingLive += CheckLosingLifeBehaviour;
    }
    void OnDisable()
    {
        OnLosingLive -= CheckLosingLifeBehaviour;
        GameManager.OnSceneLoaded -= SetPlayerInputsCallbacks;
        GameManager.OnLoseGame    -= RemovePlayerInputsCallbacks;
        GameManager.OnWinGame     -= RemovePlayerInputsCallbacks;
    }
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
        bulletSize = bulletPref.GetComponent<SpriteRenderer>().size * bulletPref.transform.lossyScale;

        // Set start variable values
        loseLivePanel.SetActive(false);
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

    // - - - - Inputs Functions - - - -
    /// <summary>
    /// Set the call backs of the input action maps of the player, this mean that we tell what to do when performing the inputs.
    /// </summary>
    private void SetPlayerInputsCallbacks()
    {
        if (InputsManager.input == null)
        {
            print("Cant initialize the player inputs because there is no input system.");
            return;
        }
        else
            InputsManager.input.PlayerActionMap.Enable();

#if UNITY_STANDALONE
        InputsManager.input.PlayerActionMap.pause.performed += ctx => LevelMenu_Management.PauseButtonPressed();
        InputsManager.input.PlayerActionMap.right.started += ctx => movingRight = true;
        InputsManager.input.PlayerActionMap.right.canceled += ctx => movingRight = false;
        InputsManager.input.PlayerActionMap.left.started += ctx => movingLeft = true;
        InputsManager.input.PlayerActionMap.left.canceled += ctx => movingLeft = false;
        InputsManager.input.PlayerActionMap.shoot.performed += ctx => shootAttempt = true;
#endif
    }
    private void RemovePlayerInputsCallbacks()
    {
        if (InputsManager.input == null)
            return;

#if UNITY_STANDALONE
        InputsManager.input.PlayerActionMap.pause.performed -= ctx => LevelMenu_Management.PauseButtonPressed();
        InputsManager.input.PlayerActionMap.right.started -= ctx => movingRight = true;
        InputsManager.input.PlayerActionMap.right.canceled -= ctx => movingRight = false;
        InputsManager.input.PlayerActionMap.left.started -= ctx => movingLeft = true;
        InputsManager.input.PlayerActionMap.left.canceled -= ctx => movingLeft = false;
        InputsManager.input.PlayerActionMap.shoot.performed -= ctx => shootAttempt = true;
#endif
    }

    // - - - - Moving functions - - - -
    private void Move()
    {
        if (movingRight)
        {
            var nextPos = rigidBody.position + new Vector2(movementSpeed, 0);
            if (nextPos.x + playerSize.x/2 <= ScreenBounds.rightLevelBoundBeforeHUD)
                rigidBody.MovePosition(nextPos);
        }
        else if (movingLeft)
        {
            var nextPos = rigidBody.position - new Vector2(movementSpeed, 0);
            if (nextPos.x - playerSize.x/2 >= ScreenBounds.leftScreenBound)
                rigidBody.MovePosition(nextPos);
        }
    }
    private void RestartPlayerPhysics()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.position = startPosition;
    }

    // - - - - Shooting functions - - - -
    private void Shoot()
    {
        if (canShoot)
        {
            canShoot = false;
            var originPos = rigidBody.position + new Vector2(0f, (playerSize.y / 2) + (bulletSize.y / 2) + 0.02f) ;
            var bullet = Instantiate(bulletPref, originPos, Quaternion.identity);
            bullet.GetComponent<PlayerBullet>().playerThatShootThis = this;
        }
    }
    /// <summary>
    /// Delay between shots. This overload should be called when we dont need a long delay, when colliding with anything than an enemy for example.
    /// </summary>
    public void DelayBeforeShootingAgain() => StartCoroutine(DelayBeforeShootingAgainCoroutine(shootDelay));
    /// <summary>
    /// Delay between shots. This overload should be called when we need a specific delay, waiting for an enemy to die for example.
    /// </summary>
    public void DelayBeforeShootingAgain(float delay) => StartCoroutine(DelayBeforeShootingAgainCoroutine(delay));
    private IEnumerator DelayBeforeShootingAgainCoroutine(float delay)
    {
        var timer = 0f;
        while (timer < delay)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        canShoot = true;
    }

    // - - - - Live functions - - - -
    private void CheckLosingLifeBehaviour()
    {
        if(immortal)
            return;
        lives--;
        if (lives > 0)
            StartCoroutine(LosingLife());
        else if (lives >= 0)
            GameManager.OnLoseGame?.Invoke();
    }
    private IEnumerator LosingLife()
    {
        // Enabel losing panel for afordance
        loseLivePanel.SetActive(true);

        GameManager.OnLevelPauseByFreezing?.Invoke(true);

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
        loseLivePanel.SetActive(false);

        GameManager.OnLevelPauseByFreezing?.Invoke(false);
    }
    
}