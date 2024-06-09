using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitAbility : MonoBehaviour
{
    [Header("�����q")]
    public int HP = 100;
    [Header("�����Z��")]
    public float AttackRange = 1.5f;
    [Header("����ˮ`")]
    public int AttackDamage = 10;
    [Header("�j���d��")]
    public float SearchRange = 10f;
    [Header("�ثe��w�Ĥ�")]
    public UnitAbility Enemy;
    [Header("�Ĥ�h��")]
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
    /// �Ĥ�������Z���~�h���V�Ĥ�,�Ϥ��h����
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
    /// �԰����U����ʮɶ�
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
    //�i�����p�d�U�ΧR���˦a���Ҳ�
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
    /// �I��Ĥ��찱���
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
