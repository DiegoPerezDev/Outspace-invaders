using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para> This class manages all of the general data of the alien army.</para>
/// <para> For the behaviour of the alien army there are other scripts attached in the same GameObject.</para>
/// <para> This class also spawn the alien army if there is no 'AlienArmyGenerator' script in the same GameObject.</para>
/// </summary>
public class AlienArmy : MonoBehaviour
{
    public delegate void AlienBehaviour();
    public static AlienBehaviour OnArmyStart, OnAlienDestroyed;
    public static Vector2 alienSize, distanceBetweenAliens = new Vector2(0.7f, 0.3f);
    public static int totalAmount, currentAmount;
    public static int armyRowsAtStart = 5, armyColumnsAtStart = 10;
    public static List<GameObject> aliens = new List<GameObject>();
    public static List<List<Alien>> aliensByRows = new List<List<Alien>>();
    public static List<List<Alien>> aliensByColumns = new List<List<Alien>>();
    public static int armyRows, armyColumns;
    public static readonly float enemyDyingDelay = 0.2f;

    private static AlienArmy instance;
    [SerializeField] private int rows = 5, columns = 10;
    [SerializeField] private RectTransform HUD_Panel;
    [SerializeField] private float centerHeight;
    [SerializeField] private GameObject alienPrefab;
    private Vector2 centerPos;

    // - - - - - Monobehaviour methods - - - - -
    void Awake()
    {
        instance = this;
        if (alienPrefab == null || HUD_Panel == null)
        {
            print("Didn't get all of the components needed for this code to work");
            Debug.Break();
            Destroy(this);
        }
    }
    void Start()
    {
        aliens = new List<GameObject>();
        aliensByRows = new List<List<Alien>>();
        aliensByColumns = new List<List<Alien>>();
        armyRowsAtStart = rows > 0? rows : armyRowsAtStart;
        armyColumnsAtStart = columns > 0 ? columns : armyColumnsAtStart;
        armyRows = armyRowsAtStart; 
        armyColumns = armyColumnsAtStart;
        SetAlienArmy();
    }

    // - - - - - Alien army setting - - - - -
    #if UNITY_EDITOR
    /// <summary>
    /// Set the data of the alien amy and generate it if there is no 'AlienArmyGenerator' script in this same gameObject. Alien generator script should only be on a test scene.
    /// </summary>
    private void SetAlienArmy()
    {
        // Just get the data of the aliens from the generator code if there is any and leave the instancing for that generator code.
        if (GetComponent<AlienArmyEditorGenerator>() != null)
            GetDataFromGeneratorCode();
        // Generate the army in this code if there is no generator code present.
        else
        {
            // Destroy existing alien army if there is one already created
            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if (child == transform) continue;
                DestroyImmediate(child.gameObject);
            }

            // Generate an alien army
            SetAlienArmyDataForInstancing();
            GenerateAlienArmy();
        }
            
        // Get all the data ready for the other codes of the alien army
        SetAliensDataAfterInstancing();
        OnArmyStart?.Invoke();
    }
    private void GetDataFromGeneratorCode()
    {
        var generatorCode = GetComponent<AlienArmyEditorGenerator>();
        alienSize = generatorCode.alienSize;
        distanceBetweenAliens = generatorCode.distanceBetweenAliens;
        armyRowsAtStart = generatorCode.armyRows;
        armyColumnsAtStart = generatorCode.armyColumns;
    }
    #else
    private void SetAlienArmy()
    {
        // Destroy existing alien army if there is one already created
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;
            DestroyImmediate(child.gameObject);
        }

        // Generate an alien army
        SetAlienArmyDataForInstancing();
        GenerateAlienArmy();
            
        // Get all the data ready for the other codes of the alien army
        SetAliensDataAfterInstancing();
        OnArmyStart?.Invoke();
    }
    #endif
    private void SetAlienArmyDataForInstancing()
    {
        // Get data needed for instancing
        alienSize = alienPrefab.GetComponent<SpriteRenderer>().size * alienPrefab.transform.lossyScale;
        var leftScreenBound = ScreenBounds.leftScreenBound;
        var rightLevelBoundBeforeHUD = ScreenBounds.rightLevelBoundBeforeHUD;
        var upScreenBound = ScreenBounds.aboveScreenBound;
        var levelWidth = (Mathf.Abs(leftScreenBound) + Mathf.Abs(rightLevelBoundBeforeHUD));
        centerPos = new Vector2((levelWidth / 2f) + leftScreenBound, centerHeight);

        // Evade math errors with variable values
        if (armyColumns < 1) armyColumns = 1;
        if (armyRows < 1) armyRows = 1;
        if (distanceBetweenAliens.x < 0) distanceBetweenAliens.x = 0;
        if (distanceBetweenAliens.y < 0) distanceBetweenAliens.y = 0;

        // Limit the center position values to evade instancing outside of the screen view.
        if ((centerPos.y + alienSize.y + distanceBetweenAliens.y / 2) > upScreenBound)
            centerPos.y = upScreenBound - alienSize.y - distanceBetweenAliens.y / 2;
        else if ((centerPos.y - alienSize.y - distanceBetweenAliens.y / 2) < -upScreenBound)
            centerPos.y = -upScreenBound + alienSize.y + distanceBetweenAliens.y / 2;
        if ((centerPos.x + alienSize.x + distanceBetweenAliens.x / 2) > rightLevelBoundBeforeHUD)
            centerPos.x = rightLevelBoundBeforeHUD - alienSize.x - distanceBetweenAliens.x / 2;
        else if ((centerPos.x - alienSize.x - distanceBetweenAliens.x / 2) < leftScreenBound)
            centerPos.x = leftScreenBound + alienSize.y + distanceBetweenAliens.y / 2;
    }
    private void GenerateAlienArmy()
    {
        // Set the first alien as the one in the upper left corner of the aliens grid
        var firstPos = new Vector2();
        firstPos.x = -((alienSize.x / 2) + (distanceBetweenAliens.x / 2)) * (armyColumns - 1);
        firstPos.y = ((alienSize.y / 2) + (distanceBetweenAliens.y / 2)) * (armyRows - 1);

        // Evate creating aliens outside the screen
        if ((centerPos.x + firstPos.x - alienSize.x / 2 < ScreenBounds.leftScreenBound) || (centerPos.x - firstPos.x + alienSize.x / 2 > ScreenBounds.rightLevelBoundBeforeHUD))
        {
            if (armyColumns > 1) // evade endless iteration 
            {
                armyColumns--;
                GenerateAlienArmy();
                return;
            }
        }
        if ((centerPos.y + firstPos.y + alienSize.y / 2 > ScreenBounds.aboveScreenBound) || (centerPos.y - firstPos.y - alienSize.y / 2 < -ScreenBounds.aboveScreenBound))
        {
            if (armyRows > 1) // evade endless iteration 
            {
                armyRows--;
                GenerateAlienArmy();
                return;
            }
        }

        // Instantiate the aliens in order
        var firstAlienPos = new Vector2(centerPos.x + firstPos.x, centerPos.y + firstPos.y);
        var currentInstancingPosition = firstAlienPos;
        GameObject instantiatedAlien;
        for (int i = 0; i < armyRows; i++)
        {
            for (int j = 0; j < armyColumns; j++)
            {
                // Instance new alien
                instantiatedAlien = Instantiate(alienPrefab, transform);

                // Change row position
                if (j != 0)
                    currentInstancingPosition += new Vector2(alienSize.x + distanceBetweenAliens.x, 0f);
                instantiatedAlien.transform.position = currentInstancingPosition;
            }
            // Change column position
            currentInstancingPosition = new Vector2(firstAlienPos.x, currentInstancingPosition.y - alienSize.y - distanceBetweenAliens.y);
        }
    }
    
    /// <summary>
    /// Set all of the aliens data, this is the main purpose of this script, it gets ready all of the data for all the other scripts that manages the alien army
    /// </summary>
    private void SetAliensDataAfterInstancing()
    {
        // Get aliens total amount
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

    // - - - - - Public methods - - - - - 
    /// <summary>
    /// Remove an specific alien of all of the lists and from other relevant data.
    /// </summary>
    public static void RemovingAlien(Alien alienToRemove)
    {
        // Substract amount of aliens
        currentAmount--;
        if (currentAmount < 1)
        {
            if (instance != null)
            {

                GameManager.OnWinGame?.Invoke();
                Destroy(instance.gameObject);
                return;
            }
        }

        // The destroyed alien is not gonna be in count for some iterations.
        alienToRemove.isAlive = false;

        // Remove row from the list of rows if we removed the last alien from that row.
        for (int i = 0; i < armyColumnsAtStart; i++)
        {
            if (aliensByRows[alienToRemove.gridPosition.y][i].isAlive)
                goto Checkcolumns;
        }
        aliensByRows.RemoveAt(alienToRemove.gridPosition.y);
        armyRows--;

        // If we remove a row, then we got to update the aliens grid position 
        for (int i = 0; i < aliensByRows.Count; i++)
        {
            if (i >= alienToRemove.gridPosition.y)
            {
                for (int j = 0; j < armyColumnsAtStart; j++)
                    aliensByRows[i][j].gridPosition.y--;
            }
        }

        Checkcolumns:
        // Remove column from the list of column if we removed the last alien from that column.
        for (int i = 0; i < armyRowsAtStart; i++)
        {
            if (aliensByColumns[alienToRemove.gridPosition.x][i].isAlive)
                return;
        }
        aliensByColumns.RemoveAt(alienToRemove.gridPosition.x);
        armyColumns--;

        // If we remove a column, then we got to update the aliens grid position 
        for (int i = 0; i < aliensByColumns.Count; i++)
        {
            if (i >= alienToRemove.gridPosition.x)
            {
                for (int j = 0; j < armyRowsAtStart; j++)
                    aliensByColumns[i][j].gridPosition.x--;
            }
        }
    }
}