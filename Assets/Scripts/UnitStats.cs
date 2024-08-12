
using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public enum UnitState
    {
        Idle,
        Patrol,
        Walk,
        Attack,
        Commannd,
        Dead
    }
    public UnitState CurrentState = UnitState.Idle;
    [Header("角色目前血量")]
    public int CurrentHP = 100;
    [Header("角色陣營序號")]
    //fix 自動填入陣營
    public int TeamNumer = 0;
    public float CurrentTime = 0;
    public UnitModel unitModel;
    [Header("目前鎖定敵方")]
    public UnitController Enemy;
    [Header("敵方層級")]
    //fixx 自動填入對方陣營
    public LayerMask enemyLayer;
    Collider2D movementArea;

    public void SetUnitState(UnitState nextState)
    {
        CurrentState = nextState;
    }
    /// <summary>
    /// 設定當前HP為最大HP
    /// </summary>
    public void SetCurentHP()
    {
        CurrentHP = unitModel.MaxHP;
    }
    /// <summary>
    /// 找出可移動區域物件
    /// </summary>
    public void FindMovementArea()
    {
        movementArea = GameObject.Find("MoveSpaceCollider").gameObject.GetComponent<PolygonCollider2D>();
    }
    /// <summary>
    /// 找尋敵方單位
    /// </summary>
    public void SearchEnemy()
    {
        if (Enemy != null && Vector2.Distance(transform.position, Enemy.transform.position) <= unitModel.AttackRange)
        {
            // 如果當前目標仍在範圍內，則不進行新的搜尋
            return;
        }
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < unitModel.AttackRange)
            {
                Enemy = collider.GetComponent<UnitController>();
                break;
            }
            if (distance < closestDistance)
            {
                closestDistance = distance;
                current = collider;
            }
        }
        if(current != null)
                Enemy = current.GetComponent<UnitController>();
    }
    /// <summary>
    /// 隨機選擇目的地
    /// </summary>
    /// <returns></returns>
    public Vector2 RandomSelectTarget()
    {
        Bounds bounds = movementArea.bounds;
        float x = 1000;
        float y = 1000;
        //若產生的點不在指定範圍內,重新隨機一次
        while (!movementArea.OverlapPoint(new Vector2(x, y)))
        {
            x = Random.Range(bounds.min.x, bounds.max.x);
            y = Random.Range(bounds.min.y, bounds.max.y);
        }
        var targetPosition = new Vector2(x, y);
        return targetPosition; 
    }

    /// <summary>
    /// 取小數至第一位
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public Vector2 RoundToOneDecimal(Vector2 vector)
    {
        return new Vector2(
            Mathf.Round(vector.x * 10f) / 10f,
            Mathf.Round(vector.y * 10f) / 10f
        );
    }

}
