using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesToSpawn;
    [SerializeField] private float spawnRate;


    private float timeOfLastSpawn = 0f;
    private float secondsPerSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondsPerSpawn = 1 / spawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if(Time.time > timeOfLastSpawn + secondsPerSpawn)
        {
            timeOfLastSpawn = Time.time;
            foreach(GameObject enemy in enemiesToSpawn)
            {
                GameObject.Instantiate(enemy, transform);
            }
        }
    }
}
