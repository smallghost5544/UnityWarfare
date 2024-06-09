using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitAbility : MonoBehaviour
{
    [Header("角色血量")]
    public int HP = 100;
    [Header("攻擊距離")]
    public float AttackRange = 1.5f;
    [Header("角色傷害")]
    public int AttackDamage = 10;
    [Header("搜索範圍")]
    public float SearchRange = 10f;
    [Header("目前鎖定敵方")]
    public UnitAbility Enemy;
    [Header("敵方層級")]
    public LayerMask enemyLayer;
    bool inCombatAction = false;
    float moveTime = 1.5f;
    float currentTime = 0;
    UnitAction unitAction;

    void Start()
    {
        unitAction = GetComponent<UnitAction>();
        InvokeRepeating("SearchEnemy", 0f, 1f);
    }

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
    /// <summary>
    /// 敵方位於攻擊距離外則走向敵方,反之則攻擊
    /// </summary>
    void WalkToEnemy()
    {
        currentTime = Random.Range(0, moveTime * 2);
        if (Enemy != null)
        {
            if (Vector3.Magnitude(Enemy.gameObject.transform.position - transform.position) > AttackRange)
                unitAction.targetPosition = Enemy.transform.position;
            else
                Attack();
        }
    }
    /// <summary>
    /// 戰鬥中下次行動時間
    /// </summary>
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
            StartCoroutine(Die());
    }
    //可視情況留下或刪除倒地單位模組
    IEnumerator Die()
    {
        unitAction.DieAnimation();
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder -= 1;
        gameObject.layer = 0;
        yield return new WaitForSeconds(1f);
        Destroy(unitAction);
        Destroy(this);
    }
    /// <summary>
    /// 碰到敵方單位停止走動
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            LayerMask layer = collision.gameObject.layer;
            if (layer != gameObject.layer)
                unitAction.targetPosition = transform.position;
        }
    }
}
