using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This script manages the rhythmic attack of the alien army, similar to the attack of the aliens of 'Space invaders 1978'.
/// </summary>
public class AlienArmy_AttackBehaviour : MonoBehaviour
{
    public float bulletSpeed = 8f;
    [SerializeField] private bool shootAtStart;
    [SerializeField] private float shootMaxDelay = 3f, shootMinDelay = 0f, delayBeforStartMoving = 0.5f;
    private float shootDelay = 4f;
    private GameObject destroyableBulletPrefab, invulnerableBulletPrefab;
    private Rigidbody2D playerRigidbody;
    private IEnumerator shootingCor;

    // - - - - Monobehaviour functions - - - -
    void OnEnable()
    {
        AlienArmy.OnArmyStart += StartScript;
        GameManager.OnLevelPauseByFreezing += DisableShooting;
    }
    void OnDisable()
    {
        AlienArmy.OnArmyStart -= StartScript;
        GameManager.OnLevelPauseByFreezing -= DisableShooting;
    }
    void OnDestroy()
    {
        StopAllCoroutines();
        if (shootingCor != null)
            StopCoroutine(shootingCor);
    }

    // - - - - - Main functions - - - - -
    /// <summary>
    /// Start method for this class, called at the end of the set-up of the 'AlienArmy' script.
    /// </summary>
    private void StartScript()
    {
        // Get the players rigidbody for the focused shooting
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().rigidBody;

        // Get bullets prefabs
        destroyableBulletPrefab = Resources.Load<GameObject>("Characters/Aliens/DestroyableAlienBullet");
        invulnerableBulletPrefab = Resources.Load<GameObject>("Characters/Aliens/InvulnerableAlienBullet");
        if (destroyableBulletPrefab == null || invulnerableBulletPrefab == null)
        {
            print("Alien bullet prefab no found");
            return;
        }

        // Start shooting
        if (shootAtStart)
            Invoke("Shoot", delayBeforStartMoving);
    }
    public void Shoot()
    {
        if (shootingCor != null)
            StopCoroutine(shootingCor);
        shootingCor = RhythmicShooting();
        StartCoroutine(shootingCor);
    }
    /// <summary>
    /// Make one alien of the army shoot, then wait till it gets destroyed before shooting again. There is a short random delay after a bullet is destroyed.
    /// </summary>
    private IEnumerator RhythmicShooting()
    {
        // Stop when there are no more aliens left
        if (AlienArmy.currentAmount <= 0)
            yield break;

        // Delay between shots: A random range value.
        var randomDelay = UnityEngine.Random.Range(shootMinDelay, shootMaxDelay);
        //var currentAlienPercentaje = (float)AlienArmy.currentAmount / AlienArmy.totalAmount; // Min 0 - Max 1.
        //var divisionForLessAliensInGame = Map(currentAlienPercentaje, 0f, 1f, 2f, 1f);       // Divide the max delay time so the fewer the amount of aliens, the shorter the delay range.
        var timer = 0f;
        shootDelay = Map(randomDelay, shootMinDelay, shootMaxDelay, 0f, shootMaxDelay);
        while (timer < shootDelay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Select alien to shoot
        GameObject alienToShoot = SelectAlienToShoot();
        if (alienToShoot == null)
        {
            print("This should not happen. Any alien to shoot");
            yield break;
        }

        // Shoot
        var bulletPrefab = destroyableBulletPrefab;
        if (Random.Range(1, 3) == 1) 
            bulletPrefab = invulnerableBulletPrefab;
        var bulletInstantiated = Instantiate(bulletPrefab, alienToShoot.transform.position, Quaternion.identity);
        bulletInstantiated.GetComponent<AlienBullet>().alienArmy_AttackBehaviour = this;
    }
    /// <summary>
    /// Enables or disable the ryhthmic shooting. Used when freezing the level when the player loses lives of the game itself.
    /// </summary>
    public void DisableShooting(bool disable)
    {
        if (disable)
        {
            if (shootingCor != null)
                StopCoroutine(shootingCor);
        }
        else
            Shoot();
    }
    

    // - - - - - Secondary functions - - - - -
    /// <summary>
    /// Get an alien to shoot. It has a low probability to be in the column nearest to the player and a high probability to be at a random position.
    /// </summary>
    /// <returns> Alien gameObject in which we are going to instance the shoot position</returns>
    private GameObject SelectAlienToShoot()
    {
        var columnIndex = 0;

        // Select if shoot at the player or at a random place. 33% change of focusing the player.
        var focusPlayer = Random.Range(1, 4) == 1;

        // Get the column position of an alien closest to the player to shoot at it
        if (focusPlayer)
        {
            var playerPosX = playerRigidbody.position.x;
            float alienPosX;

            //  - Check if the player is LEFT to any of the aliens of the FIRST column, thus the first column is the nearest to the player
            for (int i = 0; i < AlienArmy.armyRows; i++)
            {
                if (!AlienArmy.aliensByColumns[0][i].isAlive)
                    continue;
                alienPosX = AlienArmy.aliensByColumns[0][i].rigidBody.position.x;
                if (playerPosX <= alienPosX)
                {
                    columnIndex = 0;
                    goto SelectRow;
                }
            }
            //  - Check if the player is RIGHT to any of the aliens of the LAST column, thus the last column is the nearest to the player
            for (int i = 0; i < AlienArmy.armyRows; i++)
            {
                if (!AlienArmy.aliensByColumns[AlienArmy.aliensByColumns.Count - 1][i].isAlive)
                    continue;
                alienPosX = AlienArmy.aliensByColumns[AlienArmy.aliensByColumns.Count - 1][i].rigidBody.position.x;
                if (playerPosX >= alienPosX)
                {
                    columnIndex = AlienArmy.aliensByColumns.Count - 1;
                    goto SelectRow;
                }
            }

            //  - Check the rest of the alies positions, if an alien is close to the player for half of the distance between aliens, then thats the alien closest to the player 
            var closeDistance = (AlienArmy.distanceBetweenAliens.x + AlienArmy.alienSize.x) / 2;
            for (int i = 1; i < AlienArmy.aliensByColumns.Count - 1; i++)
            {
                if (!AlienArmy.aliensByColumns[i][0].isAlive)
                    continue;
                alienPosX = AlienArmy.aliensByColumns[i][0].rigidBody.position.x;
                var distanceAlienToPlayer = Mathf.Abs(alienPosX - playerPosX);
                if (distanceAlienToPlayer <= closeDistance)
                {
                    columnIndex = i;
                    break;
                }
            }
        }
        // Select a random column to shoot
        else
            columnIndex = UnityEngine.Random.Range(0, AlienArmy.aliensByColumns.Count);

        // Randomly select one alien of the picked column to shoot
        SelectRow:
        var rowsAmount = AlienArmy.aliensByColumns[columnIndex].Count;
        List<int> rowIndexesInRandomOrder = new List<int>();
        //  - Set a list of aliens rows with random order to select an available alien to shoot randomly
        List<int> listOfAllRowsIndexes = Enumerable.Range(0, rowsAmount).ToList();
        for (int i = 0; i < rowsAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, listOfAllRowsIndexes.Count);
            rowIndexesInRandomOrder.Add(listOfAllRowsIndexes[randomIndex]);
            listOfAllRowsIndexes.RemoveAt(randomIndex);
        }
        for (int i = 0; i < rowsAmount; i++)
        {
            var alienFromSelectedRow = AlienArmy.aliensByColumns[columnIndex][rowIndexesInRandomOrder[i]];
            if (alienFromSelectedRow != null)
                return alienFromSelectedRow.gameObject;
        }
        print("This should never happen. Any alien available to shoot of all of the alien of the selected column.");
        return null;
    }

    // - - - - Utilities - - - - 
    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

}