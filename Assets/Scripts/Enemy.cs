using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float minimumProximityToPathPoint;
    [SerializeField] private float damage;
    [SerializeField] private AudioClip deathSound;

    [SerializeField] private Slider healthBarSlider;

    private float currHealth;
    private List<Transform> path;
    private int currPathIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currHealth = maxHealth;
        path = MapSetup.instance.path;
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
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
            GetComponent<AudioSource>().PlayOneShot(deathSound);
            Destroy(gameObject, .25f);
            this.enabled = false;
        }
        healthBarSlider.value = currHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EconomyTower"))
        {
            collision.gameObject.GetComponent<EcoTower>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
