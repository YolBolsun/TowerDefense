using System.Runtime.CompilerServices;
using UnityEngine;

public class DamageHitboxScript : MonoBehaviour
{
    public OffensiveTower.AttackData attackData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3((attackData.splashRadius * 2), (attackData.splashRadius * 2), 1);
        Destroy(gameObject, attackData.timeToDamage);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(attackData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
