
using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public enum UnitState
{
    Idle,
    Patrol,
    Walk,
    Attack,
    Commannd,
    Dead
}
public enum SpecialtyType
{
    BuildTower,
    MineResource,
    Patrol,
    Nothing
}
public class UnitStats : MonoBehaviour, IDamageable
{
    public UnitState CurrentState = UnitState.Idle;
    [Header("角色目前血量")]
    public int CurrentHP = 100;
    [Header("角色陣營序號")]
    //fix 自動填入陣營
    public int TeamNumer = 0;
    public float CurrentTime = 0;
    public UnitModel unitModel;
    [Header("目前鎖定敵方")]
    public IDamageable Target;
    public UnitStats ShowTarget;
    [Header("敵方層級")]
    //fixx 自動填入對方陣營
    public LayerMask enemyLayer;
    public LayerMask neutralObjectLayer;
    Collider2D movementArea;
    public Action OnDeathAction;
    public Action OnHitAction;
    public SpecialtyType UnitSpecialty;
    ISpecialty chooseSpecialty;
    public int CurrentHp { get => CurrentHP; set => throw new NotImplementedException(); }


    public void SetSpecialty()
    {
        string loadSpecialtyString = UnitSpecialty.ToString();
        if (loadSpecialtyString == "Nothing")
            return;
        chooseSpecialty = Resources.Load<GameObject>("LoadSpecialtyPrefab/BuildTower").GetComponent<ISpecialty>();
        //unitSpecialty = GetComponent<ISpecialty>();
    }
    public void SetUnitState(UnitState nextState = UnitState.Idle)
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
        if (Target != null)
        {
            MonoBehaviour targetMonoBehaviour = Target as MonoBehaviour;
            if (Vector2.Distance(transform.position, targetMonoBehaviour.transform.position) <= unitModel.AttackRange)
                // 如果當前目標仍在範圍內，則不進行新的搜尋
                return;
        }
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            //float distance = Vector2.Distance(transform.position, collider.transform.position);
            float distance =(transform.position - collider.transform.position).sqrMagnitude;
            //要節省效能時再重新開啟
            //if (distance < unitModel.AttackRange)
            //{
            //    Enemy = collider.GetComponent<UnitController>();
            //    break;
            //}
            if (distance < closestDistance)
            {
                closestDistance = distance;
                current = collider;
            }
        }
        if (current != null)
        {
            Target = current.GetComponent<IDamageable>();
            ShowTarget = Target as UnitStats;
        }
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
            x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
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

    public void DoExpert()
    {
        if (chooseSpecialty != null)
        {
            chooseSpecialty.DoSpecialize();
        }
        //fix 做完行動再恢復idle
        //fix設定時間
        //fix 避免重複觸發
        //Invoke("SetUnitSatate", 4f);
        SetUnitState(UnitState.Idle);
    }
    public IEnumerator Attack(float hitAnimationStartTime = 0)
    {
        SetUnitState(UnitState.Attack);
        //CurrentTime = unitModel.moveTime;
        yield return new WaitForSeconds(hitAnimationStartTime);
        if (Target != null )
            Target.GetHurt(UnityEngine.Random.Range(0, unitModel.AttackDamage));
    }
    //fix(沒找到該物件)
    public Collider2D FindNeutralObject()
    {
        neutralObjectLayer = LayerMask.GetMask("NeutralObject");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, neutralObjectLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                current = collider;
            }
        }
        //SetUnitState(UnitState.Idle);
        if (current != null)
        {
            Target = current.GetComponent<IDamageable>();
            return current;
        }
        return null;
    }

    public void GetHurt(int damage)
    {
        if (CurrentState == UnitState.Dead)
            return;
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentState = UnitState.Dead;
            //CurrentHP = 0;
            OnDeathAction?.Invoke(); // 當 currentHP 小於 0 時，觸發 Action
            return;
        }
        OnHitAction?.Invoke();
    }

    public IEnumerator Idle()
    {
        var waitTime = UnityEngine.Random.Range(0, unitModel.randomIdleTime);
        yield return new WaitForSeconds(waitTime);
        CurrentState = UnitState.Idle;
    }

}
