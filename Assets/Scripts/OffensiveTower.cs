using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class OffensiveTower : MonoBehaviour
{
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackSpeed;
    [SerializeField] private bool projectile = false;
    [SerializeField] private bool multiHit = false;
    [SerializeField] private bool omniHit = false;

    private float timeOfLastAttack = 0f;
    private float secondsPerAttack;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Enemy currentTarget;
    private Vector3 projectileDestination;

    [Serializable]
    public class AttackData
    {
        public float attackDamage;
        public float attackRange;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondsPerAttack = 1 / attackSpeed;
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
            }
        }

        AquireNewTarget();

        PerformAttack();
    }

    private void PerformAttack()
    {
        timeOfLastAttack = Time.time;
        AttackData data = new AttackData();
        data.attackDamage = attackDamage;

        if (omniHit)
        {
            foreach (Enemy thisEnemy in enemiesInRange)
            {
                thisEnemy.TakeDamage(data);
            }
        }
        else if (!projectile)
        {
            if (currentTarget == null)
            {
                return;
            }
            else
            {
                currentTarget.TakeDamage(data);
            }
        }
        else if (projectile)
        {
            
        }

    }

    private void AquireNewTarget()
    {
        if (enemiesInRange.Count > 0)
        {
            currentTarget = enemiesInRange[0];
            projectileDestination = currentTarget.transform.position;
        }
    }

    private bool CheckIfTargetIsStillValid(Enemy target)
    {
        if (target == null)
        {
            return false;
        }
        else //need logic to determine if the enemy has left range
        {
            
            return true;
        }

    }

}
