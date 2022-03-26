using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienArmy : MonoBehaviour
{
    public float bulletVel = 6f;
    [HideInInspector] public AlienArmyGenerator generatorCode;
    [SerializeField] private float rhythmDelay = 0.2f, shootDelay, xMoveDistance = 0.2f, yMoveDistance = 0.4f;
    private readonly List<GameObject> aliens = new List<GameObject>();
    private readonly List<List<Rigidbody2D>> rowsOfAliens = new List<List<Rigidbody2D>>();
    private readonly List<GameObject> aliensFromUpperColumn = new List<GameObject>();
    private GameObject alienMoreInLeft, alienMoreInRight, bulletPrefab;
    private int armyRows, armyColumns;
    private bool movingRight = true;
    

    void Start()
    {
        // Get data of the army from the army generator code
        if (generatorCode != null)
        {
            armyRows = generatorCode.armyRows;
            armyColumns = generatorCode.armyColumns;
        }
        else
        {
            print("generator code not found and is crutial for the performance!");
            Destroy(this);
            return;
        }

        // Get aliens gameObjects
        Transform[] childs = GetComponentsInChildren<Transform>();
        for (int i = 1; i < childs.Length; i++)
            aliens.Add(childs[i].gameObject);

        // Get the aliens by columns so we can detect the screen bound just with them¿
        SetAliensLists();

        // Start aliens constant actions
        StartCoroutine(RhythmicMovement());
        shootDelay = rhythmDelay * armyRows;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Characters/Aliens/AliensBullet");
        if ( (bulletPrefab != null) || (shootDelay == 0) )
            StartCoroutine(RhythmicShooting());
        else
            print("Cant start the aliens shooting.");
    }

    private void SetAliensLists()
    {
        for (int i = 0; i < armyRows; i++)
        {
            var currentRowOfAliens = new List<Rigidbody2D>();
            for (int j = 0; j < armyColumns; j++)
            {
                // Take the top corner aliens to detect the screen bounds later
                if (i == 0)
                {
                    aliensFromUpperColumn.Add(aliens[j]);
                    if (j == 0)
                        alienMoreInLeft = aliens[j];
                    else if (j == armyColumns - 1)
                        alienMoreInRight = aliens[j];
                }
                // Get all of the aliens of one row for the movement that is done row by row
                var currentAlien = aliens[((armyColumns + 1) * i) + j - i].GetComponent<Rigidbody2D>();
                if (currentAlien != null)
                    currentRowOfAliens.Add(currentAlien);
            }
            rowsOfAliens.Add(currentRowOfAliens);
        }
    }
    private IEnumerator RhythmicMovement()
    {
        // Check Lateral screen bounds to know the movement direction.
        bool moveDown;
        if (movingRight)
        {
            while (alienMoreInRight == null)
            {
                print("right corner alien not found, asigning new right corner alien");
                yield return null;
                aliensFromUpperColumn.RemoveAt(aliensFromUpperColumn.Count - 1);
                if (aliensFromUpperColumn.Count == 0)
                    yield break;
                alienMoreInRight = aliensFromUpperColumn[aliensFromUpperColumn.Count - 1];
            }
            moveDown = (alienMoreInRight.transform.position.x + generatorCode.alienSize.x + generatorCode.xDistanceBetweenAliens >= ScreenBounds.rightLevelBound);
        }
        else
        {
            while (alienMoreInLeft == null)
            {
                print("left corner alien not found, asigning new left corner alien");
                yield return null;
                aliensFromUpperColumn.RemoveAt(0);
                if (aliensFromUpperColumn.Count == 0)
                    yield break;
                alienMoreInLeft = aliensFromUpperColumn[0];
            }
            moveDown = (alienMoreInLeft.transform.position.x - generatorCode.alienSize.x - generatorCode.xDistanceBetweenAliens <= ScreenBounds.leftScreenBound);
        }
        
        // Move aliens
        for(int i = rowsOfAliens.Count - 1; i > -1; i--)
        {
            // Move aliens row by row depending on the direction
            var actualRow = rowsOfAliens[i];
            for(int j = 0; j < armyColumns; j++)
            {
                // Get rid of an alien that is no longer in the row
                if (actualRow[j] == null)
                {
                    if (actualRow.Count < j)
                        actualRow.RemoveAt(j);
                    if (actualRow.Count == 0)
                        rowsOfAliens.RemoveAt(i);
                    else
                        rowsOfAliens[i] = actualRow;
                    continue;
                }
                // Move row of aliens
                if (moveDown)
                {
                    if (movingRight)
                        actualRow[j].MovePosition(actualRow[j].position + new Vector2(-xMoveDistance, -yMoveDistance));
                    else
                        actualRow[j].MovePosition(actualRow[j].position + new Vector2(xMoveDistance, -yMoveDistance));
                }
                else
                {
                    if (movingRight)
                        actualRow[j].MovePosition(actualRow[j].position + new Vector2(xMoveDistance, 0f));
                    else
                        actualRow[j].MovePosition(actualRow[j].position + new Vector2(-xMoveDistance, 0f));
                }
            }

            // Delay between the rows movements
            var timer = 0f;
            while (timer < rhythmDelay)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }

        if (moveDown)
            movingRight = !movingRight;
        

        // Restart
        StartCoroutine(RhythmicMovement());
    }
    private IEnumerator RhythmicShooting()
    {
        // Delay
        var timer = 0f;
        while(timer < shootDelay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Select an alien to shoot
        GameObject alienToShoot = null;
        if(aliens[0] != null)
            alienToShoot = aliens[0];
        else
            yield break;

        // Shoot
        var bullet = Instantiate(bulletPrefab, alienToShoot.transform.position, Quaternion.identity);
        bullet.transform.parent = transform; 
        var bulletCode = bullet.GetComponent<AliensBullet>();
        if(bulletCode)    
            bulletCode.alienArmyCode = this;

        // Restart
        StartCoroutine(RhythmicShooting());
    }

}