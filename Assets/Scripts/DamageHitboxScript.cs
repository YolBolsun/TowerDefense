using System.Runtime.CompilerServices;
using UnityEngine;

public class DamageHitboxScript : MonoBehaviour
{
    public float attackDamage;
    public float splashRadius;
    public float splashDuration;
    public float attackDuration;

    public class InheritedAttackData : OffensiveTower.AttackData
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var collider = GetComponent<CircleCollider2D>();
        var spriteRender = GetComponent<Transform>();

        collider.radius = splashRadius;
        transform.localScale = new Vector3((splashRadius * 2), (splashRadius * 2), (splashRadius * 2));

        Destroy(gameObject, attackDuration);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        InheritedAttackData data = new InheritedAttackData();
        data.attackDamage = attackDamage;

        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(data);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
