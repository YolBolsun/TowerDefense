using UnityEngine;
using UnityEngine.UI;

public class TimeScaleManager : MonoBehaviour
{
    private Slider timeScaleSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeScaleSlider = GetComponent<Slider>();
        OnValueChanged(); //set time scale at start
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueChanged()
    {
        Time.timeScale = timeScaleSlider.value;
    }
}
