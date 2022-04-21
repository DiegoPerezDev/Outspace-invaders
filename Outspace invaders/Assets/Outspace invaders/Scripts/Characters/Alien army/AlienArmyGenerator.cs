using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

/// <summary>
/// This class generates the gameObjects of the alien army every time the user change the inspector variables on edit mode. Also adds the main script of the army after instancing for their set up.
/// </summary>
[ExecuteInEditMode]
public class AlienArmyGenerator : MonoBehaviour
{
    public int armyRows = 5, armyColumns = 10;
    public Vector2 distanceBetweenAliens = new Vector2(0.3f, 0.3f);
    [HideInInspector] public  Vector2 alienSize;
    [SerializeField]  private GameObject alienPrefab;
    [SerializeField]  private RectTransform HUD_Panel;
    [SerializeField]  private float centerHeight;
    private Vector2 centerPos;
    private float leftScreenBound, rightLevelBound, upScreenBound;
    private static AlienArmyGenerator instance;


    void Awake()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    /// <summary>
    /// Try to re-create the alien army every time that the user change any variable value through inspector, except in playmode.
    /// </summary>
    void OnValidate()
    {
        if (!EditorApplication.isPlaying)
            OnValidateRestart();
    }

    /// <summary>
    /// This delay is needed because Unity doesn't allow the destruction of gameObjects at the start of the editExecution.
    /// </summary>
    async void OnValidateRestart()
    {
        await Task.Delay(50);
        RestartAlienArmy();
    }
    /// <summary>
    /// Create new army of aliens, destroying previus one to avoid multiple armys at the same time.
    /// </summary>
    private void RestartAlienArmy()
    {
        // Evade restarting army an the start of the playtime
        if (EditorApplication.isPlaying)
            return;

        // Dont continue the alien army restart if there is no alien prefab
        if (alienPrefab == null)
        {
            print("Can't get alien army prefab, needed for generating alien army.");
            return;
        }

        // Destroy existing alien armys if there are any
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;
            DestroyImmediate(child.gameObject);
        }

        // Create new army of aliens
        PrepareInstancingData();
        InstantiateAliensInOrder();
    }
    /// <summary>
    /// Get some data from the scene and set data in order to evade math errors
    /// </summary>
    private void PrepareInstancingData()
    {
        // Get data needed for instancing
        alienSize = alienPrefab.GetComponent<SpriteRenderer>().size * alienPrefab.transform.lossyScale;
        leftScreenBound = -(Camera.main.orthographicSize * 2 * Camera.main.aspect) / 2;
        rightLevelBound = -leftScreenBound - (HUD_Panel.rect.size.x * HUD_Panel.lossyScale.x);
        upScreenBound = Camera.main.orthographicSize;
        var levelWidth = (Mathf.Abs(leftScreenBound) + Mathf.Abs(rightLevelBound));
        centerPos = new Vector2((levelWidth / 2f) + leftScreenBound, centerHeight);

        // Evade math errors with variable values
        if (armyColumns < 1) armyColumns = 1;
        if (armyRows < 1) armyRows = 1;
        if (distanceBetweenAliens.x < 0) distanceBetweenAliens.x = 0;
        if (distanceBetweenAliens.y < 0) distanceBetweenAliens.y = 0;

        // - Limit the center position values
        if ((centerPos.y + alienSize.y + distanceBetweenAliens.y / 2) > upScreenBound)
            centerPos.y = upScreenBound - alienSize.y - distanceBetweenAliens.y / 2;
        else if ((centerPos.y - alienSize.y - distanceBetweenAliens.y / 2) < -upScreenBound)
            centerPos.y = -upScreenBound + alienSize.y + distanceBetweenAliens.y / 2;
        if ((centerPos.x + alienSize.x + distanceBetweenAliens.x / 2) > rightLevelBound)
            centerPos.x = rightLevelBound - alienSize.x - distanceBetweenAliens.x / 2;
        else if ((centerPos.x - alienSize.x - distanceBetweenAliens.x / 2) < leftScreenBound)
            centerPos.x = leftScreenBound + alienSize.y + distanceBetweenAliens.y / 2;
    }
    /// <summary>
    /// Instance the alien prefab in the clasic row-column pattern. All of the aliens being children of the alienArmy
    /// </summary>
    private void InstantiateAliensInOrder()
    {
        // Evate creating aliens outside the screen
        var firstPos = new Vector2();
        firstPos.x = -((alienSize.x / 2) + (distanceBetweenAliens.x / 2)) * (armyColumns - 1);
        firstPos.y = ((alienSize.y / 2) + (distanceBetweenAliens.y / 2)) * (armyRows - 1);
        if( (centerPos.x + firstPos.x - alienSize.x / 2 < leftScreenBound) || (centerPos.x - firstPos.x + alienSize.x/2 > rightLevelBound) )
        {
            if (armyColumns > 1) // evade endless iteration 
            {
                armyColumns--;
                InstantiateAliensInOrder();
                return;
            }
        }
        if ( (centerPos.y + firstPos.y + alienSize.y / 2 > upScreenBound) || (centerPos.y - firstPos.y - alienSize.y / 2 < -upScreenBound) )
        {
            if (armyRows > 1) // evade endless iteration 
            {
                armyRows--;
                InstantiateAliensInOrder();
                return;
            }
        }

        // Instantiate the first alien, the one that tells us the position of the first row and the first column
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

}