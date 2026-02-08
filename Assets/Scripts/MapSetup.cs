using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSetup : MonoBehaviour
{
    [SerializeField] private Tilemap towerBases;
    [SerializeField] private GameObject towerBaseObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlacePrefabs(towerBases, towerBaseObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlacePrefabs(Tilemap toUse, GameObject prefabToPlace)
    {
        BoundsInt bounds = towerBases.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (towerBases.HasTile(pos))
            {
                Vector3 worldPos = towerBases.GetCellCenterWorld(pos);
                GameObject.Instantiate(prefabToPlace, worldPos, Quaternion.identity, transform);
            }
        }
    }
}
