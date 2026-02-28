using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EcoTower : MonoBehaviour
{
    [SerializeField] private float timeBetweenIncomeEvents;
    [SerializeField] private float incomeAmount;
    [SerializeField] private bool isLossCondition;
    [SerializeField] private float health;
    [SerializeField] private TextMeshProUGUI healthDisplayField;

    public float Health
    {
        get { return health; }
        set { 
            health = value; 
            if(healthDisplayField != null)
            {
                healthDisplayField.text = Mathf.RoundToInt(health).ToString();
            }
        }
    }


    private float timeOfLastIncomeEvent = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //quick hack to update UI
        Health = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeOfLastIncomeEvent + timeBetweenIncomeEvents < Time.time)
        {
            //EcoManager.instance.CurrGold += incomeAmount;
            timeOfLastIncomeEvent = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        Camera.main.GetComponent<CameraEffects>().CameraShake(.05f*damage);
        Health -= damage;
        if (Health <= 0f)
        {
            Health = 0f;
            if (isLossCondition){
                LoseGame();
            }
        }
    }

    public void LoseGame()
    {
        SceneManager.LoadScene("Loss Scene");
    }
}
