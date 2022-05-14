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
    public static int armyRowsAtStart = 5, armyColumnsAtStart = 10;
    public static List<GameObject> aliens = new List<GameObject>();
    public static List<List<Alien>> aliensByRows = new List<List<Alien>>();
    public static List<List<Alien>> aliensByColumns = new List<List<Alien>>();

    [SerializeField] private RectTransform HUD_Panel;
    [SerializeField] private float centerHeight;

    [SerializeField] private GameObject alienPrefab;
    private static AlienArmy instance;
    private Vector2 centerPos;
    private int armyRows = 5, armyColumns = 10;


    void Awake() => instance = this;
    void Start()
    {
        print("order this code");
        aliens = new List<GameObject>();
        aliensByRows = new List<List<Alien>>();
        aliensByColumns = new List<List<Alien>>();
        StartCoroutine(SetAlienArmy());
    }

    private IEnumerator SetAlienArmy()
    {
        if (GetComponent<AlienArmyGenerator>() == null) // Alien generator script should only be on a test scene
        {
            SetAlienArmyData();
            GenerateAlienArmy();
            yield return null;
        }
        else
            GetDataFromGeneratorCode();
        SetAliensData();
        OnArmyStart?.Invoke();
        yield return null;
    }
    private void SetAlienArmyData()
    {
        // Get data needed for instancing
        alienSize = alienPrefab.GetComponent<SpriteRenderer>().size * alienPrefab.transform.lossyScale;
        distanceBetweenAliens = new Vector2(0.3f, 0.3f);
        var leftScreenBound = ScreenBounds.leftScreenBound;
        var rightLevelBound = ScreenBounds.rightLevelBound;
        var upScreenBound = ScreenBounds.upperScreenBound;
        var levelWidth = (Mathf.Abs(leftScreenBound) + Mathf.Abs(rightLevelBound));
        centerPos = new Vector2((levelWidth / 2f) + leftScreenBound, centerHeight);

        // Evade math errors with variable values
        if (armyColumns < 1) armyColumns = 1;
        if (armyRows < 1) armyRows = 1;
        if (distanceBetweenAliens.x < 0) distanceBetweenAliens.x = 0;
        if (distanceBetweenAliens.y < 0) distanceBetweenAliens.y = 0;

        // Limit the center position values
        if ((centerPos.y + alienSize.y + distanceBetweenAliens.y / 2) > upScreenBound)
            centerPos.y = upScreenBound - alienSize.y - distanceBetweenAliens.y / 2;
        else if ((centerPos.y - alienSize.y - distanceBetweenAliens.y / 2) < -upScreenBound)
            centerPos.y = -upScreenBound + alienSize.y + distanceBetweenAliens.y / 2;
        if ((centerPos.x + alienSize.x + distanceBetweenAliens.x / 2) > rightLevelBound)
            centerPos.x = rightLevelBound - alienSize.x - distanceBetweenAliens.x / 2;
        else if ((centerPos.x - alienSize.x - distanceBetweenAliens.x / 2) < leftScreenBound)
            centerPos.x = leftScreenBound + alienSize.y + distanceBetweenAliens.y / 2;
    }
    private void GenerateAlienArmy()
    {
        // Destroy existing alien armys if there are any
        Transform[] childs = GetComponentsInChildren<Transform>();
        for (int i = 1; i < childs.Length; i++)
            Destroy(childs[i].gameObject);

        // Instantiate the first alien, the one that tells us the position of the first row and the first column
        var firstPos = new Vector2();
        firstPos.x = -((alienSize.x / 2) + (distanceBetweenAliens.x / 2)) * (armyColumns - 1);
        firstPos.y = ((alienSize.y / 2) + (distanceBetweenAliens.y / 2)) * (armyRows - 1);
        var firstAlienPos = new Vector2(centerPos.x + firstPos.x, centerPos.y + firstPos.y);
        var instantiatedAlien = Instantiate(alienPrefab, transform);
        instantiatedAlien.transform.position = firstAlienPos;

        // Instantiate the rest of the aliens in order
        var currentInstancingPosition = firstAlienPos;
        for (int i = 0; i < armyRows; i++)
        {
            for (int j = 0; j < armyColumns; j++)
            {
                // First alien already instantiated
                if (i == 0 && j == 0)
                    continue;

                // Instance new alien
                instantiatedAlien = Instantiate(alienPrefab, transform);

                // Positioning aliens through the same row, if they are not the first one in the row
                if (j != 0)
                    currentInstancingPosition += new Vector2(alienSize.x + distanceBetweenAliens.x, 0f);
                instantiatedAlien.transform.position = currentInstancingPosition;
            }
            // Positioning aliens from the first column each one in a new row
            currentInstancingPosition = new Vector2(firstAlienPos.x, currentInstancingPosition.y - alienSize.y - distanceBetweenAliens.y);
        }
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
        currentAmount = totalAmount = armyRowsAtStart * armyColumnsAtStart;

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
    }

}