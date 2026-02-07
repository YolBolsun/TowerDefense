using UnityEngine;

public class EcoTower : MonoBehaviour
{
    [SerializeField] private float timeBetweenIncomeEvents;
    [SerializeField] private float incomeAmount;


    private float timeOfLastIncomeEvent = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeOfLastIncomeEvent + timeBetweenIncomeEvents < Time.time)
        {
            EcoManager.instance.CurrGold += incomeAmount;
            timeOfLastIncomeEvent = Time.time;
        }
    }
}
