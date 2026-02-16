using System.Runtime.CompilerServices;
using UnityEngine;
using static OffensiveTower;

public class DamageHitboxScript : MonoBehaviour
{
    public OffensiveTower.AttackData attackData;

    //private Animator animator;
    private Collider2D coll;
    private bool hasDamagedEnemies = false;
    private float spawnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackData = new AttackData(attackData);
        transform.localScale = new Vector3((attackData.splashRadius * 2), (attackData.splashRadius * 2), 1);
        spawnTime = Time.time;
        Destroy(gameObject, attackData.attackLifetime);
        coll = GetComponent<Collider2D>();
        if(attackData.timeToDamage > 0f)
        {
            coll.enabled = false;
        }
        //animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(attackData.singleTarget && hasDamagedEnemies)
        {
            return;
        }
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(attackData);
            hasDamagedEnemies = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if(Time.time - spawnTime > attackData.timeToDamage)
        {
            coll.enabled = true;
        }
    }

    private void Move()
    {
        Vector3 targetLocation = attackData.targetLocation;
        if (attackData.targetTransform != null)
        {
            targetLocation = attackData.targetTransform.position;
        }
        Vector3 direction = targetLocation - transform.position;
        if (direction.magnitude > .1f)
        {
            transform.Translate(direction.normalized * Time.deltaTime * attackData.projectileSpeed);
        }
    }
}
