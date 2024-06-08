using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class UnitAbility : MonoBehaviour
{
    public int HP = 100;
    public int TeamNumber = 0;
    public float AttackRange = 1.5f;
    public int AttackDamage = 10;
    public float SearchRange = 10f;
    public UnitAbility Enemy;
    public LayerMask enemyLayer;
    bool inCombatAction = false;
    float moveTime = 1.5f;
    float currentTime = 0;
    UnitAction unitAction;

    // Start is called before the first frame update
    void Start()
    {
        unitAction = GetComponent<UnitAction>();
        InvokeRepeating("SearchEnemy", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!inCombatAction)
        {
            inCombatAction = true;
            WalkToEnemy();
        }
        if (Enemy != null)
            unitAction.inAction = true;
        nextMoveTime();
    }

    void SearchEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                Enemy = collider.GetComponent<UnitAbility>();
            }
        }
    }
    void WalkToEnemy()
    {
        currentTime = Random.Range(0, moveTime * 2);
        if (Enemy != null)
        {
            if (Vector3.Magnitude(Enemy.gameObject.transform.position - transform.position) > AttackRange)
            {
                //fix ­«½ÆÄ²µo
                unitAction.targetPosition = Enemy.transform.position;
                //StartCoroutine(unitAction.WalkToTarget());
            }
            if (Vector3.Magnitude(transform.position - Enemy.transform.position) <= AttackRange)
            {
                //unitAction.isArriveTarget = true;
                unitAction.StopWalking();
                Attack();
            }
        }
    }
    void nextMoveTime()
    {
        if (inCombatAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0.01f)
                inCombatAction = false;
        }
    }
    void Attack()
    {
        Enemy.GetHurt(Random.Range(0, AttackDamage));
        unitAction.AttackAnimation();
    }
    public void GetHurt(int damage)
    {
        HP -= damage;
        if (HP < 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
        //animation
        //delete script and gameobjectt
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            LayerMask layer = collision.gameObject.layer;
            if (layer != gameObject.layer)
                // Stop the unit when it collides with another unit
                unitAction.targetPosition = transform.position;
        }
    }
}
