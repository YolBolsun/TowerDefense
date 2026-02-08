using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSetup : MonoBehaviour
{
    [SerializeField] private Tilemap towerBases;
    [SerializeField] private GameObject towerBaseObject;


    [SerializeField] private GameObject pathParentObject;
    public List<Transform> path;

    public static MapSetup instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        PlacePrefabs(towerBases, towerBaseObject);
        path = new List<Transform>();
        foreach(Transform t in pathParentObject.transform)
        {
            path.Add(t);
        }
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
