using System;
using UnityEngine;

public class OffensiveTower : MonoBehaviour
{
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackSpeed;

    private float timeOfLastAttack = 0f;
    private float secondsPerAttack;

    [Serializable]
    public class AttackData
    {
        public float attackDamage;
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
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        timeOfLastAttack = Time.time;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        AttackData data = new AttackData();
        data.attackDamage = attackDamage;

        foreach (Collider2D hit in hits)
        {

            if (hit != null && (hit.CompareTag("Enemy")))
            {
                // Clicked on a collider with the matching tag
                Enemy currEnemy = hit.gameObject.GetComponent<Enemy>();
                currEnemy.TakeDamage(data);
            }
        }

    }

}
