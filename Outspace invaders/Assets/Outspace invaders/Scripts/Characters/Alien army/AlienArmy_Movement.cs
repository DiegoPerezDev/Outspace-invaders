using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienArmy_Movement : MonoBehaviour
{
    [SerializeField] private float rhythmDelay = 0.4f, xMoveDistance = 0.2f, yMoveDistance = 0.6f;
    private float startRhythmDelay = 0.25f;
    private readonly float slowDownMaxDelay = 0.5f;
    private float slowDownDelay, previousArmySpeed, rhythmDelayDecrease;
    private bool movingRight = true;
    private int lastTwentyPercentAliens;
    private IEnumerator SlowMovementCor;


    void OnEnable()
    {
        AlienArmy.OnArmyStart += StartMovement;
        AlienArmy.OnAlienDestroyed += CheckForSpeedIncrease;
    }
    void OnDisable()
    {
        AlienArmy.OnArmyStart -= StartMovement;
        AlienArmy.OnAlienDestroyed -= CheckForSpeedIncrease;
    }
    void OnDestroy()
    {
        if(SlowMovementCor != null)
            StopCoroutine(SlowMovementCor);
        StopAllCoroutines();
    }

    /// <summary>
    /// Start method for this class, called at the end of the set-up of the 'AlienArmy' script
    /// </summary>
    private void StartMovement()
    {
        // Set rhythm of the movement, the delay between the movement of the rows of the aliens
        lastTwentyPercentAliens = (int)((AlienArmy.totalAmount * 0.2f) - 1f);
        var maxNumOfFastenByAlienKill = (8f + lastTwentyPercentAliens); // Fasten by alien kill every 10% less of the total amount of aliens till theres only 20% left, also after every single kill of the last 20%.
        rhythmDelayDecrease = startRhythmDelay / (maxNumOfFastenByAlienKill + 1);
        if (rhythmDelayDecrease < Time.fixedDeltaTime * 1.1f)
        {
            rhythmDelayDecrease = Time.fixedDeltaTime * 1.1f;
            startRhythmDelay = rhythmDelayDecrease * (maxNumOfFastenByAlienKill + 1);
        }
        rhythmDelay = startRhythmDelay;

        // Start aliens constant movement
        movingRight = true;
        StartCoroutine(RhythmicMovement());
    }
    /// <summary>
    /// Speed up the aliens ryhthm every time there is 10% less aliens, only if there are more than 20%. In case of less than 20% increase speed for each alien destroyed.
    /// </summary>
    private void CheckForSpeedIncrease()
    {
        if (AlienArmy.currentAmount < lastTwentyPercentAliens)
            IncreaseArmySpeed();
        else
        {
            for (int i = 90; i > 20; i -= 10)
            {
                if (AlienArmy.currentAmount == AlienArmy.totalAmount * (i / 100f))
                {
                    //print($"Porcentaje of enemies left: {i}");
                    IncreaseArmySpeed();
                    break;
                }
            }
        }
    }
    /// <summary>
    /// Move all the army of aliens row by row with a certain rhythm
    /// </summary>
    private IEnumerator RhythmicMovement()
    {
        // Stop the movement if there is no more aliens left
        if (AlienArmy.aliensByRows.Count < 1)
        {
            print("No aliens found");
            yield break;
        }

        // Check Lateral screen bounds to know if the movement should be down or to the sides.
        bool moveDown = CheckLateralBounds();

        // Move aliens row by row depending on the direction
        for (int y = AlienArmy.aliensByRows.Count - 1; y > -1; y--)
        {
            var actualRow = AlienArmy.aliensByRows[y];
            for (int x = 0; x < AlienArmy.armyColumnsAtStart; x++)
            {
                // Evade movement attempt if there is no alien to move
                if (actualRow[x] == null)
                    continue;
                if (!actualRow[x].isAlive)
                    continue;

                // Move row of aliens
                if (moveDown)
                {
                    if (movingRight)
                        actualRow[x].rigidBody.MovePosition(actualRow[x].rigidBody.position + new Vector2(-xMoveDistance, -yMoveDistance));
                    else
                        actualRow[x].rigidBody.MovePosition(actualRow[x].rigidBody.position + new Vector2(xMoveDistance, -yMoveDistance));
                }
                else
                {
                    if (movingRight)
                        actualRow[x].rigidBody.MovePosition(actualRow[x].rigidBody.position + new Vector2(xMoveDistance, 0f));
                    else
                        actualRow[x].rigidBody.MovePosition(actualRow[x].rigidBody.position + new Vector2(-xMoveDistance, 0f));
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

        // Change the direction of the movement after moving down
        if (moveDown)
            movingRight = !movingRight;

        // Restart
        StartCoroutine(RhythmicMovement());
    }
    /// <summary>
    /// Increase the speed of the army movement by reducing the rhythm delay
    /// </summary>
    private void IncreaseArmySpeed()
    {
        if (SlowMovementCor == null)
            rhythmDelay -= rhythmDelayDecrease;
        else
            previousArmySpeed -= rhythmDelayDecrease;
    }
    /// <summary>
    /// Slow the movement of the army every time an alien gets destroyed when they are less than half of the total aliens
    /// </summary>
    public void TemporalArmySlow()
    {
        if (AlienArmy.currentAmount > (AlienArmy.armyRowsAtStart * AlienArmy.armyColumnsAtStart / 2))
            return;

        if (SlowMovementCor != null)
        {
            StopCoroutine(SlowMovementCor);
            rhythmDelay = previousArmySpeed;
            print("Ended slow delay method 2");
            SlowMovementCor = null;
        }
        else
        {
            previousArmySpeed = rhythmDelay;
        }
        
        SlowMovementCor = TemporalSlowMovement();
        StartCoroutine(SlowMovementCor);
    }
    private IEnumerator TemporalSlowMovement()
    {
        print("started slow delay");

        // Make the movement slow
        rhythmDelay = startRhythmDelay;
        slowDownDelay = slowDownMaxDelay;

        // Wait the respected delay
        while (slowDownDelay > 0)
        {
            yield return null;
            slowDownDelay -= Time.deltaTime;
        }

        // Turn the movement back to normal
        rhythmDelay = previousArmySpeed;

        print("Ended slow delay method 1");
    }
    /// <summary>
    /// Check the bounds of the level with the aliens columns more in left and right 
    /// </summary>
    private bool CheckLateralBounds()
    {
        if (movingRight)
        {
            for (int i = 0; i < AlienArmy.aliensByColumns[AlienArmy.aliensByColumns.Count - 1].Count; i++)
            {
                var alienAnalized = AlienArmy.aliensByColumns[AlienArmy.aliensByColumns.Count - 1][i];
                if (alienAnalized)
                {
                    if (alienAnalized.isAlive)
                        return (alienAnalized.rigidBody.position.x + AlienArmy.alienSize.x + AlienArmy.distanceBetweenAliens.x >= ScreenBounds.rightLevelBound);
                }
            }
        }
        else
        {
            for (int i = 0; i < AlienArmy.aliensByColumns[0].Count; i++)
            {
                var alienAnalized = AlienArmy.aliensByColumns[0][i];
                if (alienAnalized)
                {
                    if (alienAnalized.isAlive)
                        return (alienAnalized.rigidBody.position.x - AlienArmy.alienSize.x - AlienArmy.distanceBetweenAliens.x <= ScreenBounds.leftScreenBound);
                }
            }
        }
        return false;
    }

}