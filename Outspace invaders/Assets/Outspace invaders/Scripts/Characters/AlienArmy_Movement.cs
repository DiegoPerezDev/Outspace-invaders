using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienArmy_Movement : MonoBehaviour
{
    private AlienArmy aliensFeatures;
    [SerializeField] private float rhythmDelay = 0.4f, xMoveDistance = 0.2f, yMoveDistance = 0.6f;
    private float startRhythmDelay = 0.3f;
    private readonly float lastAliensPercentage = 0.2f, slowDownMaxDelay = 0.5f;
    private float slowDownDelay, previousArmySpeed, rhythmDelayDecrease;
    private bool movingRight = true;
    private IEnumerator SlowMovementCor;


    //void OnEnable() => OnAlienDestroyed += EnemyDestruction;
    //void OnDisable() => OnAlienDestroyed -= EnemyDestruction;
    void OnDestroy()
    {
        if(SlowMovementCor != null)
            StopCoroutine(SlowMovementCor);
        StopAllCoroutines();
    }
    void Start()
    {
        aliensFeatures = gameObject.GetComponentInParent<AlienArmy>();

        aliensFeatures.lastTwentyPercentAliens = (int)((aliensFeatures.totalAmount * lastAliensPercentage) - 1f);
        var maxNumOfFastenByAlienKill = (8f + aliensFeatures.lastTwentyPercentAliens); // Fasten by alien kill every 10% less of the total amount of aliens till theres only 20% left, also after every single kill of the last 20%.
        rhythmDelayDecrease = startRhythmDelay / (maxNumOfFastenByAlienKill + 1);
        if (rhythmDelayDecrease < Time.fixedDeltaTime * 1.1f)
        {
            rhythmDelayDecrease = Time.fixedDeltaTime * 1.1f;
            startRhythmDelay = rhythmDelayDecrease * (maxNumOfFastenByAlienKill + 1);
        }
        rhythmDelay = startRhythmDelay;

        // Start aliens constant actions
        StartCoroutine(RhythmicMovement());
    }


    private void EnemyDestruction()
    {
        // Make the game faster depending on the aliens amount
        if (aliensFeatures.currentAmount < aliensFeatures.totalAmount * lastAliensPercentage)
            IncreaseArmySpeed();
        else
        {
            for (int i = 90; i > 10; i -= 10)
            {
                if (aliensFeatures.currentAmount == aliensFeatures.totalAmount * (i / 100f))
                {
                    //print($"Porcentaje of enemies left: {i}");
                    IncreaseArmySpeed();
                    break;
                }
            }
        }
    }

    private IEnumerator RhythmicMovement()
    {
        // Stop the movement if there is no more aliens left
        if (aliensFeatures.aliensByRows.Count < 1)
        {
            print("No aliens found");
            yield break;
        }

        // Check Lateral screen bounds to know the movement direction.
        bool moveDown = CheckLateralBoundsWithAliens();

        // Move aliens
        for (int y = aliensFeatures.aliensByRows.Count - 1; y > -1; y--)
        {
            // Move aliens row by row depending on the direction
            var actualRow = aliensFeatures.aliensByRows[y];
            for (int x = 0; x < aliensFeatures.armyColumnsAtStart; x++)
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

        if (moveDown)
            movingRight = !movingRight;

        // Restart
        StartCoroutine(RhythmicMovement());
    }
    private void IncreaseArmySpeed()
    {
        if (SlowMovementCor == null)
            rhythmDelay -= rhythmDelayDecrease;
        else
            previousArmySpeed -= rhythmDelayDecrease;
    }
    public void SlowArmyByAlienKill()
    {
        if (aliensFeatures.currentAmount < (aliensFeatures.armyRowsAtStart * aliensFeatures.armyColumnsAtStart / 2))
        {
            if (SlowMovementCor != null)
            {
                print("Ended slow delay method 2");
                StopCoroutine(SlowMovementCor);
                SlowMovementCor = null;
            }
            else
            {
                // Change the movement delay of the army
                previousArmySpeed = rhythmDelay;
            }
            rhythmDelay = startRhythmDelay;
            slowDownDelay = slowDownMaxDelay;
            SlowMovementCor = TemporalSlowMovement();
            StartCoroutine(SlowMovementCor);
        }
    }
    private IEnumerator TemporalSlowMovement()
    {
        print("started slow delay");
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
    private bool CheckLateralBoundsWithAliens()
    {
        if (movingRight)
        {
            for (int i = 0; i < aliensFeatures.aliensByColumns[aliensFeatures.aliensByColumns.Count - 1].Count; i++)
            {
                var alienAnalized = aliensFeatures.aliensByColumns[aliensFeatures.aliensByColumns.Count - 1][i];
                if (alienAnalized.isAlive)
                    return (alienAnalized.rigidBody.position.x + aliensFeatures.alienSize.x + aliensFeatures.distanceBetweenAliens.x >= ScreenBounds.rightLevelBound);
            }
        }
        else
        {
            for (int i = 0; i < aliensFeatures.aliensByColumns[0].Count; i++)
            {
                var alienAnalized = aliensFeatures.aliensByColumns[0][i];
                if (alienAnalized.isAlive)
                    return (alienAnalized.rigidBody.position.x - aliensFeatures.alienSize.x - aliensFeatures.distanceBetweenAliens.x <= ScreenBounds.leftScreenBound);
            }
        }
        return false;
    }

}