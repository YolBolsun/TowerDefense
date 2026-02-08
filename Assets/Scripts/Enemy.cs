using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float minimumProximityToPathPoint;

    private float currHealth;
    private List<Transform> path;
    private int currPathIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currHealth = maxHealth;
        path = MapSetup.instance.path;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (currPathIndex >= path.Count)
        {
            Debug.LogError("Reached Path End without contacting tower to attack");
            return;
        }
        Vector3 destination = path[currPathIndex].position;
        if((destination-transform.position).magnitude < minimumProximityToPathPoint){
            currPathIndex++;
            Move();
        }
        else
        {
            transform.Translate((destination-transform.position).normalized * movementSpeed * Time.deltaTime);
        }
        
    }

    public void TakeDamage(OffensiveTower.AttackData attackData)
    {
        currHealth -= attackData.attackDamage;
        if (currHealth <= 0)
        {
            currHealth = 0;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EcoTower"))
        {
            Debug.Log("damage to towers not implemented");
            Destroy(gameObject);
        }
    }
}
