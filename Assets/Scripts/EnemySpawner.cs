using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /*[SerializeField] private List<GameObject> enemiesToSpawn;
    [SerializeField] private float spawnRate;*/

    private int waveNumber = 0;
    private int spawnNumber = 0;

    public bool spawnNextWaveEarly = false;

    [SerializeField] private List<Wave> waves;

    [Serializable]
    private class Spawnable
    {
        public GameObject enemyType;
        public int enemyNumber;
        public float timeBeforeNextSpawn;
    }

    [Serializable]
    private class Wave
    {
        public List<Spawnable> waveSpawns;
    }


    private float timeOfLastSpawn = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(waves.Count > 0)
        {
            StartCoroutine(SpawnNext());
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator SpawnNext()
    {
        Debug.Log(waveNumber + " " + spawnNumber);
        if(waveNumber >= waves.Count)
        {
            Debug.Log("waves are done, should probably go to the next level or something");
        }
        else
        {
            if(spawnNumber >= waves[waveNumber].waveSpawns.Count)
            {
                waveNumber += 1;
                spawnNumber = 0;
                spawnNextWaveEarly = false;
                StartCoroutine(SpawnNext());
            }
            else
            {
                for(int i = 0; i < waves[waveNumber].waveSpawns[spawnNumber].enemyNumber; i++)
                {
                    GameObject.Instantiate(waves[waveNumber].waveSpawns[spawnNumber].enemyType, transform);
                }

                if (!spawnNextWaveEarly)
                {
                    yield return new WaitForSeconds(waves[waveNumber].waveSpawns[spawnNumber].timeBeforeNextSpawn);

                }
                spawnNumber += 1;
                StartCoroutine(SpawnNext());
            }  
        }
    }

    public void SpawnNextWaveEarly()
    {
        spawnNextWaveEarly = true;
    }

}
