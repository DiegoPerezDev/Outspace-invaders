using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

[ExecuteInEditMode]
public class AlienArmyGenerator : MonoBehaviour
{
    public int armyRows = 4, armyColumns = 5;
    [HideInInspector] public Vector2 alienSize;
    [Range(0f, 1f)] public float xDistanceBetweenAliens = 1f, yDistanceBetweenAliens = 1f;
    [SerializeField] private GameObject alienPrefab;
    [SerializeField] private Camera currentCamera;
    [SerializeField] private float centerHeight = 0f;
    private Vector2 centerPos = new Vector2(0f, 0f);
    private float leftScreenBound, rightLevelBound, upScreenBound;
    private GameObject aliensParent;
    [SerializeField] private RectTransform HUD_Panel;

    void Awake()
    {
        alienSize = alienPrefab.GetComponent<SpriteRenderer>().size * alienPrefab.transform.lossyScale;
        leftScreenBound = -(currentCamera.orthographicSize * 2 * Camera.main.aspect) / 2;
        rightLevelBound = -leftScreenBound - (HUD_Panel.rect.size.x * HUD_Panel.lossyScale.x);
        upScreenBound = currentCamera.orthographicSize;
    }
    void OnValidate()
    {
        // Evade the function in playTime
        if (!EditorApplication.isPlaying)
            OnValidateRestart();
    }

    async void OnValidateRestart()
    {
        await Task.Delay(50);
        RestartAlienArmy();
    }
    private void RestartAlienArmy()
    {
        // Evade restarting army in the start of the playtime
        if (EditorApplication.isPlaying)
            return;

        // Destroy existing alien armys if there are any.
        var AlienArmy = GameObject.FindGameObjectWithTag("AlienArmy");
        if (AlienArmy != null)
            DestroyImmediate(AlienArmy);

        // Create new army of aliens
        aliensParent = new GameObject("AlienArmy");
        aliensParent.transform.parent = GameObject.Find("Characters").transform;
        aliensParent.tag = "AlienArmy";
        var aliensArmyCode = aliensParent.AddComponent<AlienArmy>();
        aliensArmyCode.generatorCode = this;
        if (alienPrefab != null)
            InstantiateAliensInOrder();
        else
            print("Can't get alien army prefab.");
    }
    private void InstantiateAliensInOrder()
    {
        // Get again fome values that might have changed
        var levelWidth = (Mathf.Abs(leftScreenBound) + Mathf.Abs(rightLevelBound));
        centerPos = new Vector2((levelWidth/2f) + leftScreenBound, centerHeight);

        // Evade math errors
        if (armyColumns < 1) armyColumns = 1;
        if (armyRows < 1) armyRows = 1;
        if (xDistanceBetweenAliens < 0) xDistanceBetweenAliens = 0;
        if (yDistanceBetweenAliens < 0) yDistanceBetweenAliens = 0;

        // Limit the center position values
        if ((centerPos.y + alienSize.y + yDistanceBetweenAliens / 2) > upScreenBound)
            centerPos.y = upScreenBound - alienSize.y - yDistanceBetweenAliens / 2;
        else if ((centerPos.y - alienSize.y - yDistanceBetweenAliens / 2) < -upScreenBound)
            centerPos.y = -upScreenBound + alienSize.y + yDistanceBetweenAliens / 2;
        if ((centerPos.x + alienSize.x + xDistanceBetweenAliens / 2) > rightLevelBound)
            centerPos.x = rightLevelBound - alienSize.x - xDistanceBetweenAliens / 2;
        else if ((centerPos.x - alienSize.x - xDistanceBetweenAliens / 2) < leftScreenBound)
            centerPos.x = leftScreenBound + alienSize.y + yDistanceBetweenAliens / 2;

        // Evate creating aliens outside the screen
        var xFirstPos = -((alienSize.x / 2) + (xDistanceBetweenAliens / 2)) * (armyColumns - 1);
        var yFirstPos = ((alienSize.y / 2) + (yDistanceBetweenAliens / 2)) * (armyRows - 1);
        if (armyColumns > 1)
        { 
            if( (xFirstPos + centerPos.x < leftScreenBound) || (-xFirstPos + centerPos.x > rightLevelBound) )
            {
                armyColumns--;
                InstantiateAliensInOrder();
                return;
            }
        }
        if (armyRows > 1)
        {
            if ( (yFirstPos + centerPos.y > upScreenBound) || (-yFirstPos + centerPos.y < -upScreenBound) )
            {
                armyRows--;
                InstantiateAliensInOrder();
                return;
            }
        }

        // Instantiate first alien, the one that tells us the position of the first row and the first column
        var firstAlienPos = new Vector2(centerPos.x + xFirstPos, centerPos.y + yFirstPos);
        var instantiatedAlien = Instantiate(alienPrefab, aliensParent.transform);
        instantiatedAlien.transform.position = firstAlienPos;

        // Instantiate the rest of the aliens in order
        var currentInstancingPosition = firstAlienPos;
        for (int i = 0; i < armyRows; i++)
        {
            for (int j = 0; j < armyColumns; j++)
            {
                // First alien already instantiated, no need for instancing
                if (i == 0 && j == 0) 
                    continue;

                // Instance new alien
                instantiatedAlien = Instantiate(alienPrefab, aliensParent.transform);

                // Positioning alien in the same row, but in a new column
                if(j != 0)
                    currentInstancingPosition += new Vector2(alienSize.x + xDistanceBetweenAliens, 0f);
                instantiatedAlien.transform.position = currentInstancingPosition;
            }
            // adjust positioning in the next row and in the first column
            currentInstancingPosition = new Vector2(firstAlienPos.x, currentInstancingPosition.y - alienSize.y - yDistanceBetweenAliens);
        }
            
    }

}