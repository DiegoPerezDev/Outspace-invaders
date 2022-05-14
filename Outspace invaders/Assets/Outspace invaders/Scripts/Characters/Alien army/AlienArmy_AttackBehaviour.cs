using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AlienArmy_AttackBehaviour : MonoBehaviour
{
    public static bool shooting;
    private GameObject bulletPrefab;
    private float shootDelay = 4f;
    private readonly float shootMaxDelay = 2f;
    private Rigidbody2D playerRigidbody;



    void OnEnable() => AlienArmy.OnArmyStart += StartShooting;
    void OnDisable() => AlienArmy.OnArmyStart -= StartShooting;
    void OnDestroy()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Start method for this class, called at the end of the set-up of the 'AlienArmy' script
    /// </summary>
    void StartShooting()
    {
        // Get player rigidbody for the focused shooting
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().rigidBody;

        // Get bullet prefab
        bulletPrefab = Resources.Load<GameObject>("Characters/Aliens/AlienBullet");
        if (bulletPrefab == null)
        {
            print("Alien bullet prefab no found");
            return;
        }

        // Start aliens constant shooting
        if (shootDelay < 0.1f)
            shootDelay = 0.1f;
        StartCoroutine(RhythmicShooting());
    }

    /// <summary>
    /// Make one alien of the army shoot, then wait till its destroyed before shooting again. There is a random plus wait after a bullet is destroyed.
    /// </summary>
    private IEnumerator RhythmicShooting()
    {
        // Stop when there are no more aliens left
        if (AlienArmy.currentAmount <= 0)
            yield break;

        // Dont shoot while there is still an alien bullet in the scene
        while (shooting)
        {
            yield return null;
        }

        // Random delay after shooting, delay is an integer or a .5 number
        var timer = 0f;
        SetShootDelay();
        while (timer < shootDelay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Select alien to shoot
        GameObject alienToShoot = SelectAlienToShoot();
        if (alienToShoot == null)
        {
            StartCoroutine(RhythmicShooting());
            yield break;
        }

        // Shoot
        shooting = true;
        Instantiate(bulletPrefab, alienToShoot.transform.position, Quaternion.identity);

        // Restart
        StartCoroutine(RhythmicShooting());
    }


    /// <summary>
    /// Set the delay of the alien shooting depending on the amount of aliens left, faster with fewer aliens
    /// </summary>
    private void SetShootDelay()
    {
        var randomDelay = UnityEngine.Random.Range(0f, shootMaxDelay);
        var currentAlienPercentaje = (float)AlienArmy.currentAmount / AlienArmy.totalAmount;
        var divisionForLessAliensInGame = Map(currentAlienPercentaje, 0f, 1f, 4f, 1f);
        shootDelay = Map(randomDelay, 0f, shootMaxDelay, 0f, shootMaxDelay / divisionForLessAliensInGame);
    }

    /// <summary>
    /// Get an alien to shoot. it has a low probability to be in the column nearest to the player and a high probability to be at a random position.
    /// </summary>
    /// <returns> Alien gameObject in which we are going to instance the shoot position</returns>
    private GameObject SelectAlienToShoot()
    {
        var focusPlayer = Random.Range(1, 5) == 1;
        var columnIndex = 0;

        if (focusPlayer) // Get an alien from the column nearest to the player horizontally to shoot at it
        {
            var playerPosX = playerRigidbody.position.x;

            // Check if any alien has the player closer than the next aliens
            var currentColumns = AlienArmy.aliensByColumns.Count;
            for (int i = 0; i < currentColumns; i++)
            {
                if (AlienArmy.aliensByColumns[i][0] == null)
                {
                    print("alien not found here");
                    return null;
                }
                var alienPosX = AlienArmy.aliensByColumns[i][0].rigidBody.position.x;
                var distanceAlienPlayer = Mathf.Abs(alienPosX - playerPosX);
                var closeDistance = (AlienArmy.distanceBetweenAliens.x + AlienArmy.alienSize.x) / 2;
                if (distanceAlienPlayer <= closeDistance)
                {
                    //print("shoot aiming player");
                    columnIndex = i;
                    goto SelectRow;
                }

                // check if player is LEFT to the FIRST alien, thus the first alien is the nearest to it
                if(i == 0)
                {
                    if (playerPosX <= alienPosX)
                    {
                        //print("shoot aiming player with first column");
                        columnIndex = 0;
                        goto SelectRow;
                    }
                }

                // check if player is RIGHT to the LAST alien, thus the last alien is the nearest to it
                else if (i == currentColumns - 1)
                {
                    if (playerPosX >= alienPosX)
                    {
                        //print("shoot aiming player with last column");
                        columnIndex = currentColumns - 1;
                        goto SelectRow;
                    }
                }
            }
            print("This should never happen");
        }
        else // Select a random column to shoot
        {
            //print("random shoot");
            columnIndex = UnityEngine.Random.Range(0, AlienArmy.aliensByColumns.Count);
        }

    // Select one alien of a selected row randomly, one of those aliens not destroyed
    SelectRow:
        var rowAmount = AlienArmy.aliensByColumns[columnIndex].Count;
        List<int> randomRowOrder = new List<int>();
        // - set list of aliens with random row order to check another one if the previous has been destroyed
        List<int> possibleAliensToSelect = Enumerable.Range(0, rowAmount).ToList();
        for (int i = 0; i < rowAmount; i++)
        {
            int index = UnityEngine.Random.Range(0, possibleAliensToSelect.Count);
            randomRowOrder.Add(possibleAliensToSelect[index]);
            possibleAliensToSelect.RemoveAt(index);
        }
        for (int i = 0; i < rowAmount; i++)
        {
            var alienFromSelectedRow = AlienArmy.aliensByColumns[columnIndex][randomRowOrder[i]];
            if (alienFromSelectedRow != null)
                return alienFromSelectedRow.gameObject;
        }
        print("This should never happen");
        return null;
    }

    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

}