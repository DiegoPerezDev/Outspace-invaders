using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienArmy_AttackBehaviour : MonoBehaviour
{
    [SerializeField] private float shootDelay = 4f;
    public static float bulletVel = 6f;
    private GameObject bulletPrefab;
    private AlienArmy aliensFeatures;

    void Start()
    {
        aliensFeatures = gameObject.GetComponentInParent<AlienArmy>();

        // Start aliens constant actions
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Characters/Aliens/AliensBullet");
        if ((bulletPrefab != null) || (shootDelay == 0))
            StartCoroutine(RhythmicShooting());
        else
            print("Cant start the aliens shooting.");
    }
    void OnDestroy()
    {
        StopAllCoroutines();    
    }

    private IEnumerator RhythmicShooting()
    {
        // Stop when there are no more aliens left
        if (aliensFeatures.currentAmount <= 0)
            yield break;

        // Delay
        var timer = 0f;
        while (timer < shootDelay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Select an alien to shoot
        GameObject alienToShoot = null;
        if (aliensFeatures.aliens[0] != null)
            alienToShoot = aliensFeatures.aliens[0];
        else
        {
            StartCoroutine(RhythmicShooting());
            yield break;
        }

        // Shoot
        var bullet = Instantiate(bulletPrefab, alienToShoot.transform.position, Quaternion.identity);
        bullet.transform.parent = transform;

        // Restart
        StartCoroutine(RhythmicShooting());
    }

}