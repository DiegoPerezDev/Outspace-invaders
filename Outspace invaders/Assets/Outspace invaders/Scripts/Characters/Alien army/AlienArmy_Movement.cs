using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienArmy_Movement : MonoBehaviour
{
    [Range(0.1f, 1f)] [SerializeField] private float startRhythmDelay = 0.5f;
    [Range(0.005f, 0.02f)][SerializeField] private float rhythmDelayDecrease = 0.01f;
    [SerializeField] private float xMoveDistance = 0.2f, yMoveDistance = 0.6f;
    private float rhythmDelay;
    private readonly float slowDownMaxDelay = 0.5f;
    private float slowDownDelay, previousArmySpeed;
    private bool movingRight = true;
    private int lastTwentyPercentAliens;
    private IEnumerator SlowMovementCor;
    private int rowNumInTheMove;


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
        var maxNumOfFastenByAlienKill = (8f + lastTwentyPercentAliens); // Fasten by alien kill every 10% less of the total amount of aliens till theres only 20% left, also after every single kill of the last 20%
        if (rhythmDelayDecrease < (startRhythmDelay - 0.021f) / maxNumOfFastenByAlienKill)
            rhythmDelayDecrease = (startRhythmDelay - 0.023f) / maxNumOfFastenByAlienKill;
        rhythmDelay = startRhythmDelay;

        // Start aliens constant movement
        movingRight = true;
        SlowMovementCor = RhythmicMovement();
        rowNumInTheMove = AlienArmy.aliensByRows.Count - 1;
        StartCoroutine(SlowMovementCor);
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
        var alienMaxRows = AlienArmy.aliensByRows.Count - 1;
        
        for (int y = alienMaxRows; y > -1; y--)
        {
            if (y <= rowNumInTheMove)
                rowNumInTheMove--;
            else
                continue;

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

        rowNumInTheMove = alienMaxRows;

        // Change the direction of the movement after moving down
        if (moveDown)
            movingRight = !movingRight;

        // Restart
        RestartMovementCor();
    }
    private void RestartMovementCor()
    {
        if (SlowMovementCor != null)
            StopCoroutine(SlowMovementCor);
        SlowMovementCor = RhythmicMovement();
        StartCoroutine(SlowMovementCor);
    }

    public void EnableMovement(bool enable)
    {
        if (enable)
            RestartMovementCor();
        else
        {
            if (SlowMovementCor != null)
                StopCoroutine(SlowMovementCor);
        }
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

        RestartMovementCor();
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
                        return (alienAnalized.rigidBody.position.x + (AlienArmy.alienSize.x + AlienArmy.distanceBetweenAliens.x) / 2f >= ScreenBounds.rightLevelBound);
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
                        return (alienAnalized.rigidBody.position.x - (AlienArmy.alienSize.x + AlienArmy.distanceBetweenAliens.x) / 2f <= ScreenBounds.leftScreenBound);
                }
            }
        }
        return false;
    }

}