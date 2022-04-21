using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienArmy_AttackBehaviour : MonoBehaviour
{
    public static bool shooting;
    private GameObject bulletPrefab;
    private float shootDelay = 4f;

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
        while(shooting)
        {
            yield return null;
        }

        // Random plus delay of the shoot
        var timer = 0f;
        shootDelay = Random.Range(0f, 2f);
        while (timer < shootDelay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Shoot
        shooting = true;
        GameObject alienToShoot = null;
        if (AlienArmy.aliens[0] != null)
            alienToShoot = AlienArmy.aliens[0];
        else
        {
            StartCoroutine(RhythmicShooting());
            yield break;
        }
        var bullet = Instantiate(bulletPrefab, alienToShoot.transform.position, Quaternion.identity);
        bullet.transform.parent = transform;

        // Restart
        StartCoroutine(RhythmicShooting());
    }

}