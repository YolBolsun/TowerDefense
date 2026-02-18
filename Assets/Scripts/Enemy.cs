using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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

    [SerializeField] private float burnTickAmount;
    [SerializeField] private float burnTickCooldown;
    [SerializeField] private GameObject burningAnimationObject;

    private List<OffensiveTower.StatusEffect> statusEffects = new List<OffensiveTower.StatusEffect>();
    private List<float> statusEffectEndTimes = new List<float>();

    private float currHealth;
    private List<Transform> path;
    private int currPathIndex;

    public float distanceMoved = 0f;

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
        for(int i = statusEffects.Count-1; i >= 0; i--)
        {
            if (Time.time > statusEffectEndTimes[i])
            {
                statusEffects.RemoveAt(i);
                statusEffectEndTimes.RemoveAt(i);
            }
        }
    }

    private void Move()
    {
        float adjustedMovementSpeed = movementSpeed;
        if(statusEffects.Contains(OffensiveTower.StatusEffect.Stun))
        {
            adjustedMovementSpeed = 0f;
        }
        else if(statusEffects.Contains(OffensiveTower.StatusEffect.Slow))
        {
            adjustedMovementSpeed = .5f * movementSpeed;
        }
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
            transform.Translate((destination-transform.position).normalized * adjustedMovementSpeed * Time.deltaTime);
            distanceMoved += adjustedMovementSpeed;
        }
        
    }

    public void TakeDamage(OffensiveTower.AttackData attackData)
    {
        TakeDamage(attackData.attackDamage);

        statusEffects.Add(attackData.statusEffect);
        statusEffectEndTimes.Add(Time.time + attackData.statusEffectDuration);

        if(attackData.statusEffect == OffensiveTower.StatusEffect.Burn)
        {
            StartBurn();
        }
    }

    //this is only used internally for additional damage effects - attacks should use the public version
    private void TakeDamage(float damage)
    {
        currHealth -= damage;
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

    private void StartBurn()
    {
        burningAnimationObject.SetActive(true);
        StartCoroutine(HandleBurn());
    }

    private void StopBurn()
    {
        burningAnimationObject.SetActive(false);
    }

    // A coroutine must have the return type IEnumerator
    IEnumerator HandleBurn()
    {
        if (!statusEffects.Contains(OffensiveTower.StatusEffect.Burn))
        {
            StopBurn();
            yield break;
        }

        TakeDamage(burnTickAmount);
        yield return new WaitForSeconds(burnTickCooldown);

        StartCoroutine(HandleBurn());

        
    }
}
