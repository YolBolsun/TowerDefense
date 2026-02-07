using UnityEngine;
using TMPro;

public class EcoManager : MonoBehaviour
{
    public static EcoManager instance;
    private float currGold;
    [SerializeField] private TMPro.TextMeshProUGUI goldValue;

    public float CurrGold
    {
        get { return currGold; }
        set {
            goldValue.text = value.ToString();
            currGold = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
