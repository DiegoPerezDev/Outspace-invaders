using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienArmy : MonoBehaviour
{
    public delegate void AlienBehaviour();
    public static        AlienBehaviour OnAlienDestroyed;
    [HideInInspector] public readonly List<GameObject> aliens = new List<GameObject>();
    [HideInInspector] public readonly List<List<Alien>> aliensByRows = new List<List<Alien>>();
    [HideInInspector] public readonly List<List<Alien>> aliensByColumns = new List<List<Alien>>();
    [HideInInspector] public int      armyRowsAtStart, armyColumnsAtStart;
    [HideInInspector] public int      totalAmount, currentAmount, lastTwentyPercentAliens;
    [HideInInspector] public Vector2  alienSize, distanceBetweenAliens;


    void OnEnable() => OnAlienDestroyed += EnemyDestruction;
    void OnDisable() => OnAlienDestroyed -= EnemyDestruction;
    void Start()
    {
        GetDataFromGeneratorCode();
        SetAliensData();
        // Add the Alien army behaviour scripts after the data setting, because we need that data even at the start method of those scripts. Thats why we cant let happen the posibility of starting this script after those.
        gameObject.AddComponent<AlienArmy_Movement>();
        gameObject.AddComponent<AlienArmy_AttackBehaviour>();
    }

    /// <summary>
    /// Remove an specific alien of all of the lists and other relevant data in this alien army code
    /// </summary>
    /// <param name="alienToRemove">Alien script attached to the alien gameObject to get it specific data for the removal.</param>
    public void RemoveAlienFromArmy(Alien alienToRemove)
    {
        // The destroyed alien is not gonna be in count for the next movements, this evades checking on components constantly
        alienToRemove.isAlive = false;

        // Check if there is any alien alive in the row, else remove the row from the list
        for (int i = 0; i < armyColumnsAtStart; i++)
        {
            if (aliensByRows[alienToRemove.position.y][i].isAlive)
                goto Checkcolumns;
        }
        // A row is being removed so we have to move the rest of the rows positions
        aliensByRows.RemoveAt(alienToRemove.position.y);
        for (int i = 0; i < aliensByRows.Count; i++)
        {
            if (i >= alienToRemove.position.y)
            {
                for (int j = 0; j < armyColumnsAtStart; j++)
                    aliensByRows[i][j].position.y--;
            }
        }
        print($"Removing row #{alienToRemove.position.y}. Number of rows: {aliensByRows.Count}");

    Checkcolumns:
        // Check if there is any alien alive in the column, else remove the column from the list
        for (int i = 0; i < armyRowsAtStart; i++)
        {
            if (aliensByColumns[alienToRemove.position.x][i].isAlive)
                return;
        }
        // A column is being removed so we have to move the rest of the columns positions
        aliensByColumns.RemoveAt(alienToRemove.position.x);
        print($"Removing column #{alienToRemove.position.x}. Number of columns: {aliensByColumns.Count}");
        for (int i = 0; i < aliensByColumns.Count; i++)
        {
            if (i >= alienToRemove.position.x)
            {
                for (int j = 0; j < armyRowsAtStart; j++)
                    aliensByColumns[i][j].position.x--;
            }
        }
    }
    /// <summary>
    /// Get data of the army features from the army generator code, those will be needed for the other scripts that manage the alien army behaviour
    /// </summary>
    private void GetDataFromGeneratorCode()
    {
        var generatorCode = gameObject.GetComponentInParent<AlienArmyGenerator>();
        if (generatorCode != null)
        {
            alienSize = generatorCode.alienSize;
            distanceBetweenAliens = generatorCode.distanceBetweenAliens;
            armyRowsAtStart = generatorCode.armyRows;
            armyColumnsAtStart = generatorCode.armyColumns;
            currentAmount = totalAmount = armyRowsAtStart * armyColumnsAtStart;
        }
        else
        {
            print("army generator code not found and is imprescindible!");
            Destroy(this);
            Debug.Break();
        }
    }
    private void SetAliensData()
    {
        // Get aliens gameObjects
        Transform[] childs = GetComponentsInChildren<Transform>();
        for (int i = 1; i < childs.Length; i++)
            aliens.Add(childs[i].gameObject);

        // Order aliens by rows and columns lists
        for (int y = 0; y < armyRowsAtStart; y++)
        {
            var currentRowOfAliens = new List<Alien>();
            for (int x = 0; x < armyColumnsAtStart; x++)
            {
                var newAlienCode = aliens[((armyColumnsAtStart + 1) * y) + x - y].AddComponent<Alien>();
                newAlienCode.SetAlien(new Vector2Int(x, y), aliens[((armyColumnsAtStart + 1) * y) + x - y].GetComponent<Rigidbody2D>());
                currentRowOfAliens.Add(newAlienCode);
            }
            aliensByRows.Add(currentRowOfAliens);
        }
        for (int x = 0; x < armyColumnsAtStart; x++)
        {
            var currentColumnOfAliens = new List<Alien>();
            for (int y = 0; y < armyRowsAtStart; y++)
            {
                var alienCode = aliens[((armyColumnsAtStart + 1) * y) + x - y].GetComponent<Alien>();
                currentColumnOfAliens.Add(alienCode);
            }
            aliensByColumns.Add(currentColumnOfAliens);
        }
    }
    private void EnemyDestruction()
    {
        currentAmount--;
        if (currentAmount < 1)
            Destroy(this);
    }

}