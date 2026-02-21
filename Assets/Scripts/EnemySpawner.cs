using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    /*[SerializeField] private List<GameObject> enemiesToSpawn;
    [SerializeField] private float spawnRate;*/

    private int waveNumber = 0;
    private int spawnNumber = 0;

    public bool spawnNextWaveEarly = false;

    [SerializeField] private List<Wave> waves;
    [SerializeField] private float spawnLocRandomness = .5f;

    [Header("Auto Wave Generation")]
    [SerializeField] private bool useAutoWaves;
    [SerializeField] private List<GameObject> prefabsToUse;
    [SerializeField] private float perWaveTime;
    [SerializeField] private int wavesTotal;
    [SerializeField] private float firstWaveTotalHealth;
    [SerializeField] private float lastWaveTotalHealth;
    [Tooltip("Specify sets of indices in prefabs to use to spawn together in a short duration")]
    [SerializeField] private List<SpawnCluster> enemySingleSpawnCluster;
    [SerializeField] private float timeBetweenSpawnInClusters;

    [Serializable]
    private class SpawnCluster
    {
        public List<GameObject> enemyType;
    }

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (useAutoWaves)
        {
            GenerateWaveSetup();
        }
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
                    float randX = Random.Range(-1*spawnLocRandomness, spawnLocRandomness);
                    float randY = Random.Range(-1 * spawnLocRandomness, spawnLocRandomness);
                    Vector3 spawnLoc = new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
                    GameObject.Instantiate(waves[waveNumber].waveSpawns[spawnNumber].enemyType,spawnLoc, Quaternion.identity, transform);
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

    
    public void GenerateWaveSetup()
    {
        waves = new List<Wave>();
        /*Spawnable spawn = new Spawnable();
        spawn.enemyType = prefabsToUse[0];
        spawn.enemyNumber = 1;
        spawn.timeBeforeNextSpawn = 1f;
        Wave newWave = new Wave();
        newWave.waveSpawns = new List<Spawnable> { spawn };
        waves.Add(newWave);
        return;*/


        for( int i = 0; i < wavesTotal; i++)
        {
            float healthForThisWave = (lastWaveTotalHealth - firstWaveTotalHealth) / wavesTotal * i + firstWaveTotalHealth;
            float healthSpawned = 0;
            float timeUsed = 0;
            Wave currWave = new Wave();
            currWave.waveSpawns = new List<Spawnable>();
            while (healthSpawned < healthForThisWave)
            {
                List<GameObject> prefabsToSpawn = GetPrefabsToSpawn();
                int spawnedThisCluster = 0;
                foreach(GameObject prefab in prefabsToSpawn)
                {
                    Enemy enemy = prefab.GetComponent<Enemy>();
                    healthSpawned += enemy.maxHealth;
                    Spawnable toSpawn = new Spawnable();
                    toSpawn.enemyType = prefab;
                    toSpawn.enemyNumber = 1;
                    spawnedThisCluster += 1;
                    // if we are not on the last spawn of the cluster
                    if (prefabsToSpawn.Count > spawnedThisCluster)
                    {
                        toSpawn.timeBeforeNextSpawn = timeBetweenSpawnInClusters;
                    }
                    else
                    {
                        toSpawn.timeBeforeNextSpawn = ((healthSpawned / healthForThisWave)-(timeUsed/perWaveTime)) * perWaveTime;
                        toSpawn.timeBeforeNextSpawn = toSpawn.timeBeforeNextSpawn < 0f ? 0f : toSpawn.timeBeforeNextSpawn;
                    }
                    currWave.waveSpawns.Add(toSpawn);
                    timeUsed += toSpawn.timeBeforeNextSpawn;
                }
            }
            waves.Add(currWave);
        }
    }

    public List<GameObject> GetPrefabsToSpawn()
    {
        

        if(enemySingleSpawnCluster != null && enemySingleSpawnCluster.Count > 0)
        {
            int cluster = Random.Range(0,enemySingleSpawnCluster.Count);
            return enemySingleSpawnCluster[cluster].enemyType;
        }
        else
        {
            List<GameObject> toSpawn = new List<GameObject>();
            int prefabIndex = Random.Range(0, prefabsToUse.Count);
            toSpawn.Add(prefabsToUse[prefabIndex]);
            return toSpawn;
        }
    }

}
