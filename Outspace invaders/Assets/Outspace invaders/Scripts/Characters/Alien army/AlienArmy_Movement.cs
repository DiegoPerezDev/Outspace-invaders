using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script manages the movement of the alien army, similar to the movement of 'Space invaders 1978'.
/// </summary>
public class AlienArmy_Movement : MonoBehaviour
{
    public static AlienArmy_Movement instance;
    [Range(0.4f, 2f)] [SerializeField] private float startArmyRhythmDelay = 1.5f;
    [Range(0.04f, 0.09f)][SerializeField] private float lastRhythmDelay = 0.02f;
    [SerializeField] private float xMoveDistance = 0.2f, yMoveDistance = 0.6f, delayBeforStartMoving = 0.1f;
    private float armyRhythmDelay, rhythmDelayDecrease, slowDownDelay;
    private bool movingRight = true;
    private int lastAliensAmount, rowIndexInTheMove;
    private IEnumerator movementCor, slowMovementCor;
    private bool slowMoving;
    

    void OnEnable()
    {
        instance = this;
        AlienArmy.OnArmyStart += StartScript;
        AlienArmy.OnAlienDestroyed += SpeedChangingWhenAlienDead;
        GameManager.OnLevelPauseByFreezing += DisableMovement;
    }
    void OnDisable()
    {
        instance = null;
        AlienArmy.OnArmyStart -= StartScript;
        AlienArmy.OnAlienDestroyed -= SpeedChangingWhenAlienDead;
        GameManager.OnLevelPauseByFreezing -= DisableMovement;
    }
    void OnDestroy()
    {
        if(movementCor != null)
            StopCoroutine(movementCor);
        StopAllCoroutines();
    }

    /// <summary>
    /// Start moving when the alien army is already set.
    /// </summary>
    private void StartScript()
    {
        SetRhythmVariableValues();
        movingRight = true;
        rowIndexInTheMove = AlienArmy.aliensByRows.Count - 1;
        Invoke("StartMovement", delayBeforStartMoving);
    }
    /// <summary>
    /// Set all of the variables for managing the rhythm of the movement through the game. See the documentation for specific details.
    /// <para> Make the army movement faster every time there is 10% less aliens of the total amount, till there is only 10% left, after that it will happen every single alien death.</para>
    /// <para> The previous statement can only happen if there are at least 10 aliens in the army at the beginning.</para>
    /// </summary>
    private void SetRhythmVariableValues()
    {
        // Set the amount of times we are going to speed up the alien army
        int maxNumOfFastenByAlienKill;
        if (AlienArmy.totalAmount > 9)
        {
            lastAliensAmount = (int)(AlienArmy.totalAmount * 0.1f);
            maxNumOfFastenByAlienKill = (7 + lastAliensAmount);
        }
        else
            maxNumOfFastenByAlienKill = lastAliensAmount = AlienArmy.totalAmount - 1;

        // the last rhythm should not be that fast as the engine speed so we limit this here.
        lastRhythmDelay = (lastRhythmDelay < Time.fixedDeltaTime * 2) ? Time.fixedDeltaTime * 2 : lastRhythmDelay;

        // The first row delay also cannot be faster than the last row delay.
        armyRhythmDelay = startArmyRhythmDelay;
        if (armyRhythmDelay / AlienArmy.armyRows < lastRhythmDelay)
        {
            print("This shouldn't happen.");
            Debug.Break();
        }

        // Set the decrease time depending on the start delay wanted, the last delay wanted and the times we are going to speed up the army.
        rhythmDelayDecrease = (startArmyRhythmDelay - lastRhythmDelay) / (maxNumOfFastenByAlienKill + 1);
    }
    private void StartMovement()
    {
        movementCor = RhythmicMovement();
        StartCoroutine(movementCor);
    }

    /// <summary>
    /// Move all the army of aliens row by row with a certain rhythm
    /// </summary>
    private IEnumerator RhythmicMovement()
    {
        // Check Lateral screen bounds to know if the movement should be down or to the sides.
        bool moveDown = CheckLateralBounds();

        // Move aliens row by row depending on the direction
        var alienMaxRowIndex = AlienArmy.aliensByRows.Count - 1;
        for (int y = alienMaxRowIndex; y > -1; y--)
        {
            // Remember which row we are moving for continuing where we left when we stop the movement on the player dying.
            if (y <= rowIndexInTheMove)
                rowIndexInTheMove--;
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

            // Delay between each row movement
            var timer = 0f;
            var delayBetweenRowMovements = slowMoving? startArmyRhythmDelay : armyRhythmDelay / AlienArmy.armyRows;
            while (timer < delayBetweenRowMovements)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }

        // Change the direction of the movement after moving down
        if (moveDown)
            movingRight = !movingRight;

        // Restart
        rowIndexInTheMove = alienMaxRowIndex;
        yield return null;
        RestartMovementCor();
    }
    
    /// <summary>
    /// Speed up the aliens ryhthm every time there is 10% less aliens of the total amount, till there is only 20% left, after that it will happen every single alien death.
    /// <para> If there is less than 25 aliens at the beginning then just speed up for every kill.</para>
    /// </summary>
    private void SpeedChangingWhenAlienDead()
    {
        if (AlienArmy.currentAmount <= lastAliensAmount + 1)
        {
            IncreaseArmySpeed(true);
            TemporalArmySlow();
        }
        else
        {
            for (int i = 90; i > 10; i -= 10)
            {
                if (AlienArmy.currentAmount == (int)(AlienArmy.totalAmount * (i / 100f)))
                {
                    //print($"Porcentaje of enemies left: {i}");
                    IncreaseArmySpeed(false);
                    TemporalArmySlow();
                    break;
                }
                else
                {
                    //print($"AlienArmy.currentAmount: {AlienArmy.currentAmount}");
                    //print($"AlienArmy.totalAmount * (i / 100f): {(int)(AlienArmy.totalAmount * (i / 100f))}");
                }
            }
        }
    }
    private void RestartMovementCor()
    {
        if (movementCor != null)
            StopCoroutine(movementCor);
        movementCor = RhythmicMovement();
        StartCoroutine(movementCor);
    }
    private void DisableMovement(bool disable)
    {
        if (disable)
        {
            if(movementCor != null)
                StopCoroutine(movementCor);
        }
        else
            RestartMovementCor();
    }
    /// <summary>
    /// Increase the speed of the army movement by reducing the rhythm delay 
    /// </summary>
    private void IncreaseArmySpeed(bool lastAliens)
    {
        if(lastAliens)
            armyRhythmDelay -= (rhythmDelayDecrease * 0.4f);
        else
            armyRhythmDelay -= (rhythmDelayDecrease * 1.6f);
            
        if (armyRhythmDelay * AlienArmy.armyRows < Time.fixedDeltaTime)
        {
            print("This should never happen");
            armyRhythmDelay = Time.fixedDeltaTime * AlienArmy.armyRows;
        }
        //print($"rowRhythmDelay: {armyRhythmDelay / AlienArmy.armyRows}");
        print($"armyRhythmDelay: {armyRhythmDelay}");
    }
    /// <summary>
    /// Slow the movement of the army every time an alien gets destroyed when they are less than half of the total aliens
    /// </summary>
    private void TemporalArmySlow()
    {
        if(slowMovementCor != null) StopCoroutine(slowMovementCor);
        slowMovementCor = null;
        slowMovementCor = TemporalSlowMovement();
        StartCoroutine(slowMovementCor);
    }
    private IEnumerator TemporalSlowMovement()
    {
        // Make the movement slow
        slowMoving = true;

        // Wait the respected delay
        while (slowDownDelay > 0)
        {
            yield return null;
            slowDownDelay -= Time.deltaTime;
        }

        // Turn the movement back to normal
        slowMoving = false;
    }
    /// <summary>
    /// Check the bounds of the level with the aliens from the first and last columns. 
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
                        return (alienAnalized.rigidBody.position.x + (AlienArmy.alienSize.x + AlienArmy.distanceBetweenAliens.x) / 2f >= ScreenBounds.rightLevelBoundBeforeHUD);
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