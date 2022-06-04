using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public delegate void LevelPauseDelegates(bool pausing);
    public static LevelPauseDelegates OnLevelPause;
    private static GameObject alienArmy;
    private static AlienArmy_AttackBehaviour alienArmy_AttackBehaviour;
    private static AlienArmy_Movement alienArmy_Movement;

    void Awake()
    {
        alienArmy = GameObject.Find("Characters/AlienArmy");
        if (alienArmy == null)
        {
            print("Alien army gameObject not found");
            Debug.Break();
        }
        alienArmy_AttackBehaviour = alienArmy.GetComponent<AlienArmy_AttackBehaviour>();
        alienArmy_Movement = alienArmy.GetComponent<AlienArmy_Movement>();
    }

    public static void LevelFreeze(bool freezing)
    {
        // Disable alien army behaviour
        alienArmy_AttackBehaviour.EnableShooting(!freezing);
        alienArmy_Movement.EnableMovement(!freezing);

        // Disable random alien behaviour
        RandomAlien.EnableMovement(!freezing);

        // Disable player behaviour
        InputsManager.EnablePlayerActionMaps(!freezing);
    }

}