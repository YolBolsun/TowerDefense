using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NextLevelButtonScript : MonoBehaviour
{
    public Button thisButton;
    private EnemySpawner enemySpawnerScriptReference;
    void StartNextLevel()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex+1) % 5);
        Invoke("InvokedSetupTheNextLevelButton", 3);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisButton = GetComponent<Button>();

        if (thisButton != null)
        {
            thisButton.onClick.AddListener(StartNextLevel);
        }
        else
        {
            Debug.LogError("Error: The button couldn't find itself");
        }
    }

    void InvokedSetupTheNextLevelButton()
    {
        GameObject enemySpawnerReference = GameObject.Find("EnemySpawner");

        if (enemySpawnerReference != null)
        {
            enemySpawnerScriptReference = enemySpawnerReference.GetComponent<EnemySpawner>();
            if (enemySpawnerScriptReference != null)
            {
                enemySpawnerScriptReference.SetupTheNextLevelButton();
            }
            else
            {
                Debug.Log("Bestie the Invoke worked but I cant find the scripts...");
            }
        }
        else
        {
            Debug.Log("Hey Echo Nefarious, that super awesome Invoke idea you had didn't work");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
