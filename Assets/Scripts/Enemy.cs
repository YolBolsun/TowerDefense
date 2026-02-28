using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats and Movement")]
    [SerializeField] public float maxHealth;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float minimumProximityToPathPoint;
    [SerializeField] private float damage;
    [SerializeField] private int goldDropped = 1;

    [Header("Sound FX and Visuals")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private Slider healthBarSlider;

    [Header("Burn Damage")]
    [SerializeField] private float burnTickAmount;
    [SerializeField] private float burnTickCooldown;
    [SerializeField] private GameObject burningAnimationObject;

    [Header("Splitter Settings")]
    [SerializeField] private bool splitter = false; 
    [SerializeField] private int splitterCount;
    [SerializeField] private float spawnLocRandomness = .5f;
    [SerializeField] private GameObject splitVersionPrefab;

    private List<OffensiveTower.StatusEffect> statusEffects = new List<OffensiveTower.StatusEffect>();
    private List<float> statusEffectEndTimes = new List<float>();

    private float currHealth;
    private List<Transform> path;
    [HideInInspector]
    public int currPathIndex = 0;

    [HideInInspector]
    public float distanceMoved = 0f;

    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ModifyEnemyHealth(maxHealth);
        path = MapSetup.instance.path;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ModifyEnemyHealth(float scaledMaxHealth)
    {
        this.maxHealth = scaledMaxHealth;
        currHealth = maxHealth;
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
            Vector3 direction = (destination - transform.position).normalized;
            spriteRenderer.flipX = direction.x < 0f;
            transform.Translate(direction * adjustedMovementSpeed * Time.deltaTime);
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
            Die();
        }
        healthBarSlider.value = currHealth;
    }

    private void Die()
    {
        if (splitter)
        {
            for(int i = 0; i < splitterCount; i++)
            {
                float randX = Random.Range(-1 * spawnLocRandomness, spawnLocRandomness);
                float randY = Random.Range(-1 * spawnLocRandomness, spawnLocRandomness);
                Vector3 spawnLoc = new Vector3(transform.position.x + randX, transform.position.y + randY, transform.position.z);
                Enemy splitterChild = GameObject.Instantiate(splitVersionPrefab, spawnLoc, Quaternion.identity).GetComponent<Enemy>();
                splitterChild.currPathIndex = currPathIndex;
                splitterChild.distanceMoved = distanceMoved;
            }
        }

        EcoManager.instance.CurrGold += goldDropped;
        GetComponent<AudioSource>().PlayOneShot(deathSound);
        Destroy(gameObject, .25f);
        this.enabled = false;
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
