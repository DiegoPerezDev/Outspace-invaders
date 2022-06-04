using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAlienSpawner : MonoBehaviour
{
    public float movementSpeed = 3.5f;
    [SerializeField] private float spawnDelay = 30f, SpawnPosY = 4f;
    [SerializeField] private GameObject randomAlienPrefab;
    private Vector2 size;

    void Start()
    {
        // Dont continue if there is no prefav
        if (randomAlienPrefab == null)
        {
            print("No random alien prefab found");
            Destroy(this);
        }
        // Get components
        size = randomAlienPrefab.GetComponent<SpriteRenderer>().bounds.size;
        // Spawn iteration
        RandomAlien.movementSpeed = movementSpeed;
        StartCoroutine(RandomAlienShipSpawn());
    }

    private IEnumerator RandomAlienShipSpawn()
    {
        // delay
        var timer = 0f;
        while(timer < spawnDelay)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Spawn ship randomly in the left or the right of the screen and adds its movement
        var spawnPosition = new Vector2(ScreenBounds.rightLevelBound + size.x / 2 + 0.1f, SpawnPosY);
        if (Random.Range(1, 3) == 1)
            spawnPosition = new Vector2(ScreenBounds.leftScreenBound - size.x / 2 - 0.1f, SpawnPosY);
        Instantiate(randomAlienPrefab, spawnPosition, Quaternion.identity, transform.parent);
        
        // Restart
        StartCoroutine(RandomAlienShipSpawn());
    }

}