using System;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveTower : MonoBehaviour
{
    [SerializeField] private float attackSpeed;
    [SerializeField] private bool projectile = false;
    [SerializeField] private GameObject DamageHitboxPrefab;

    [SerializeField] private AttackData attackData;

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
        public float attackLifetime;
        public float splashRadius = 1f;
        public float timeToDamage;
        public bool followTarget = false;
        public bool singleTarget = false;

        [HideInInspector]
        public Transform targetTransform;
        [HideInInspector]
        public Vector3 targetLocation;

        public AttackData(AttackData other)
        {
            attackDamage = other.attackDamage;
            attackRange = other.attackRange;
            projectileSpeed = other.projectileSpeed;
            attackLifetime = other.attackLifetime;  
            splashRadius = other.splashRadius;  
            timeToDamage = other.timeToDamage;
            followTarget = other.followTarget;
            singleTarget = other.singleTarget;
            targetTransform = other.targetTransform;
            targetLocation = other.targetLocation;
        }
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
            PopulateEnemiesInRange();
            PerformAttack();
         }
        
    }

    private void PopulateEnemiesInRange()
    {
        enemiesInRange.Clear();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackData.attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit != null && (hit.CompareTag("Enemy")))
            {
                enemiesInRange.Add(hit.gameObject.GetComponent<Enemy>());
            }
        }
    }

    private void PerformAttack()
    {
        timeOfLastAttack = Time.time;


        if (!projectile) //hitscan
        {
            if (CurrentTarget == null)
            {
                return;
            }
            else
            {
                GameObject DamageHitBox = GameObject.Instantiate(DamageHitboxPrefab, CurrentTarget.transform.position, UnityEngine.Quaternion.identity);
                DamageHitboxScript hitBoxScript = DamageHitBox.GetComponent<DamageHitboxScript>();
                hitBoxScript.attackData = attackData;
            }
        }
        else
        {
            if (CurrentTarget == null)
            {
                return;
            }
            else
            {
                if (attackData.followTarget)
                {
                    attackData.targetTransform = CurrentTarget.transform;
                }
                else
                {
                    attackData.targetTransform = null;
                    attackData.targetLocation = CurrentTarget.transform.position;
                }
                GameObject DamageHitBox = GameObject.Instantiate(DamageHitboxPrefab, transform.position, UnityEngine.Quaternion.identity);
                var hitBoxScript = DamageHitBox.GetComponent<DamageHitboxScript>();
                hitBoxScript.attackData = attackData;
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
        return !(currentTarget == null || (currentTarget.gameObject.transform.position - transform.position).magnitude > attackData.attackRange);
    }

}
