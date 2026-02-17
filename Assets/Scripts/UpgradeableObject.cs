using UnityEngine;

public class UpgradeableObject : MonoBehaviour
{
    // currently hardcoding these per upgradeable object - generating these dynamically based on the actual stats of each tower
    // is better but will be left out of scope for this demo project
    public GameObject upgradeOption1;
    public float cost1;
    public GameObject upgradeOption2;
    public float cost2;
    public string tooltipText1;
    public string tooltipText2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
