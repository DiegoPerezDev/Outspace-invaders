using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

#if UNITY_EDITOR
/// <summary>
/// This class generates the gameObjects of the alien army every time the user changes the inspector variables on edit mode.
/// <para> IMPORTANT: This script is not required, its only for testing purposes.</para>
/// <para> For the behaviour of the alien army there are other scripts attached in the same GameObject that contains this code.</para>
/// </summary>
[ExecuteInEditMode]
public class AlienArmyEditorGenerator : MonoBehaviour
{
    public int armyRows = 5, armyColumns = 10;
    public Vector2 distanceBetweenAliens = new Vector2(0.3f, 0.3f);
    [HideInInspector] public  Vector2 alienSize;
    [SerializeField]  private GameObject alienPrefab;
    [SerializeField]  private RectTransform HUD_Panel;
    [SerializeField]  private float centerHeight;
    private float leftScreenBound, rightLevelBoundBeforeHUD, upScreenBound;
    private static AlienArmyEditorGenerator instance;
    private Vector2 centerPos;

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
        if (alienPrefab != null && HUD_Panel != null)
            RestartAlienArmy();
        else
            print("Didn't get all of the components needed for this code to work");
    }
    /// <summary>
    /// Create new army of aliens, destroying the previous one to avoid multiple armys at the same time.
    /// </summary>
    private void RestartAlienArmy()
    {
        // Evade restarting army an the start of the playtime
        if (EditorApplication.isPlaying)
            return;

        // Destroy existing alien army if there is one already created
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
        rightLevelBoundBeforeHUD = -leftScreenBound - (HUD_Panel.rect.size.x * HUD_Panel.lossyScale.x);
        upScreenBound = Camera.main.orthographicSize;
        var levelWidth = (Mathf.Abs(leftScreenBound) + Mathf.Abs(rightLevelBoundBeforeHUD));
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
        if ((centerPos.x + alienSize.x + distanceBetweenAliens.x / 2) > rightLevelBoundBeforeHUD)
            centerPos.x = rightLevelBoundBeforeHUD - alienSize.x - distanceBetweenAliens.x / 2;
        else if ((centerPos.x - alienSize.x - distanceBetweenAliens.x / 2) < leftScreenBound)
            centerPos.x = leftScreenBound + alienSize.y + distanceBetweenAliens.y / 2;
    }
    /// <summary>
    /// Instance the alien prefab in the clasic row-column pattern. All of the aliens being children of the alienArmy
    /// </summary>
    private void InstantiateAliensInOrder()
    {
        // Set the first alien as the one in the upper left corner of the aliens grid
        var firstPos = new Vector2();
        firstPos.x = -((alienSize.x / 2) + (distanceBetweenAliens.x / 2)) * (armyColumns - 1);
        firstPos.y = ((alienSize.y / 2) + (distanceBetweenAliens.y / 2)) * (armyRows - 1);

        // Evate creating aliens outside the screen
        if ( (centerPos.x + firstPos.x - alienSize.x / 2 < leftScreenBound) || (centerPos.x - firstPos.x + alienSize.x/2 > rightLevelBoundBeforeHUD) )
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

}
#endif