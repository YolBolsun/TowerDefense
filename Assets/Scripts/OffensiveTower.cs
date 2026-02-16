using System;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveTower : MonoBehaviour
{
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float splashRadius;
    [SerializeField] private bool projectile = false;
    [SerializeField] private bool splashDamage = false;
    [SerializeField] private float splashDuration;
    [SerializeField] private bool omniHit = false;
    [SerializeField] private GameObject DamageHitboxPrefab;

    private float timeOfLastAttack = 0f;
    private float secondsPerAttack;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Enemy currentTarget;

    public Enemy CurrentTarget
    {
        get
        {
            if (!CheckIfTargetIsStillValid())
            {
                AcquireNewTarget();
            }
            return currentTarget;
        }
        set
        {
            currentTarget = value;
        }
    }


    [Serializable]
    public class AttackData
    {
        public float attackDamage;
        public float attackRange;
        public float projectileSpeed;
        public float splashRadius;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondsPerAttack = 1 / attackSpeed;
        if (splashDuration == 0)
        {
            splashDuration = 0.1f;
        }
        if (splashRadius == 0)
        {
            splashRadius = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeOfLastAttack + secondsPerAttack)
        {
            BeginAttack();
         }
        
    }

    private void BeginAttack()
    {
        enemiesInRange.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit != null && (hit.CompareTag("Enemy")))
            {
                // Clicked on a collider with the matching tag
                Enemy currEnemy = hit.gameObject.GetComponent<Enemy>();

                if (!enemiesInRange.Contains(currEnemy)) //This should be true, but its here for extra handling
                {
                    enemiesInRange.Add(currEnemy);
                }
            }
        }
        PerformAttack();
    }

    private void PerformAttack()
    {
        timeOfLastAttack = Time.time;
        AttackData data = new AttackData();
        data.attackDamage = attackDamage;
        data.projectileSpeed = projectileSpeed;
        data.splashRadius = splashRadius;

        if (omniHit)
        {
            foreach (Enemy thisEnemy in enemiesInRange)
            {
                thisEnemy.TakeDamage(data);
            }
        }
        else if (!projectile) //hitscan
        {
            if (CurrentTarget == null)
            {
                return;
            }
            else
            {
                if (splashDamage)
                {
                    GameObject DamageHitBox = GameObject.Instantiate(DamageHitboxPrefab, CurrentTarget.gameObject.transform.position, UnityEngine.Quaternion.identity);
                    var hitBoxScript = DamageHitBox.GetComponent<DamageHitboxScript>();
                    hitBoxScript.attackDamage = data.attackDamage;
                    hitBoxScript.splashRadius = data.splashRadius;
                    hitBoxScript.attackDuration = splashDuration;
                }
                else
                {
                    CurrentTarget.TakeDamage(data);
                }
            }
        }
        else if (projectile)
        {
            if (CurrentTarget == null)
            {
                return;
            }
            else
            {
                if (splashDamage)
                {
                    GameObject DamageHitBox = GameObject.Instantiate(DamageHitboxPrefab, CurrentTarget.gameObject.transform.position, UnityEngine.Quaternion.identity);
                    var hitBoxScript = DamageHitBox.GetComponent<DamageHitboxScript>();
                    hitBoxScript.attackDamage = data.attackDamage;
                    hitBoxScript.splashRadius = data.splashRadius;
                    hitBoxScript.attackDuration = splashDuration;
                }
                else
                {
                    CurrentTarget.TakeDamage(data);
                }
            }
        }

    }

    private void AcquireNewTarget()
    {
        if (enemiesInRange.Count > 0)
        {
            currentTarget = enemiesInRange[0];
        }
        else {
            currentTarget = null;
        }
    }

    private bool CheckIfTargetIsStillValid()
    {
        return !(currentTarget == null || (currentTarget.gameObject.transform.position - transform.position).magnitude > attackRange);
    }

}
