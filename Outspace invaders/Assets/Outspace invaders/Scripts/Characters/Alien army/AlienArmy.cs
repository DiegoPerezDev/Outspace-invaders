using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para> This class manages all of the general data of the alien army for their use in other scripts that the alien army may use.</para>
/// <para>It also has methods and delegates for all actions that manages the general data of the aliens. Like when they get killed.</para>
/// </summary>
public class AlienArmy : MonoBehaviour
{
    public delegate void AlienBehaviour();
    public static AlienBehaviour OnArmyStart, OnAlienDestroyed;
    public static Vector2 alienSize, distanceBetweenAliens;
    public static int totalAmount, currentAmount;
    public static int armyRowsAtStart, armyColumnsAtStart;
    public static List<GameObject> aliens = new List<GameObject>();
    public static List<List<Alien>> aliensByRows = new List<List<Alien>>();
    public static List<List<Alien>> aliensByColumns = new List<List<Alien>>();
    private static AlienArmy instance;


    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GetDataFromGeneratorCode();
        SetAliensData();
        OnArmyStart?.Invoke();
    }
    /// <summary>
    /// Get data of the army features from the army generator code, those will be needed for the other scripts that also manage the alien army behaviour
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
    /// <summary>
    /// Set all of the aliens data, this is the main purpose of this script, it gets ready all of the data for all the other scripts that manages the alien army
    /// </summary>
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
                var alienToAdd = aliens[((armyColumnsAtStart + 1) * y) + x - y];
                var newAlienCode = alienToAdd.GetComponent<Alien>();
                newAlienCode.SetAlien(new Vector2Int(x, y), alienToAdd.GetComponent<Rigidbody2D>());
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
    /// <summary>
    /// Behaviour when any alien gets destroyed. Remove an specific alien of all of the lists and from other relevant data.
    /// </summary>
    public static void RemovingAlien(Alien alienToRemove)
    {
        // Substract amount of aliens
        currentAmount--;
        if (currentAmount < 1)
        {
            if (instance != null)
            {
                Destroy(instance);
                return;
            }
        }

        // Slow the movement of the army for a short time
        instance.gameObject.GetComponent<AlienArmy_Movement>().TemporalArmySlow();

        print("removing alien");
        // The destroyed alien is not gonna be in count for the next movements (evades checking on components constantly)
        alienToRemove.isAlive = false;

        // Check if there is any alien alive in the row, else remove the row from the list of rows and adjust all rows position
        for (int i = 0; i < armyColumnsAtStart; i++)
        {
            if (aliensByRows[alienToRemove.position.y][i].isAlive)
                goto Checkcolumns;
        }
        aliensByRows.RemoveAt(alienToRemove.position.y);
        for (int i = 0; i < aliensByRows.Count; i++)
        {
            if (i >= alienToRemove.position.y)
            {
                for (int j = 0; j < armyColumnsAtStart; j++)
                    aliensByRows[i][j].position.y--;
            }
        }
    //print($"Removing row #{alienToRemove.position.y}. Number of rows: {aliensByRows.Count}");

    Checkcolumns:
        // Check if there is any alien alive in the column, else remove the column from the list and adjust all columns position
        for (int i = 0; i < armyRowsAtStart; i++)
        {
            if (aliensByColumns[alienToRemove.position.x][i].isAlive)
                return;
        }
        aliensByColumns.RemoveAt(alienToRemove.position.x);
        for (int i = 0; i < aliensByColumns.Count; i++)
        {
            if (i >= alienToRemove.position.x)
            {
                for (int j = 0; j < armyRowsAtStart; j++)
                    aliensByColumns[i][j].position.x--;
            }
        }
        //print($"Removing column #{alienToRemove.position.x}. Number of columns: {aliensByColumns.Count}");
    }

}