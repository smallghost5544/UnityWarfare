
using System;
using UnityEngine;
using System.Collections;
using static TreeEditor.TreeEditorHelper;
using UnityEditor;

public enum UnitModelType
{
    BasicMeleeUnitModel,
    BasicRangeUnitModel,
    ArcherTowerModel,
    CastleModel
}

public enum UnitState
{
    Idle,
    Patrol,
    Walk,
    DoSpecialty,
    Building,
    Mining,
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
    [Header("角色陣營序號(請選填)")]
    public UnitColor TeamColor;
    [Header("角色數值表(請選填)")]
    public UnitModelType ChooseUnitModel = 0;
    /// <summary>
    /// 單位專長的實際功能
    /// </summary>
    [Header("選取單位的專長(請單位選填)")]
    public SpecialtyType UnitSpecialty;
    [Header("角色動作(起始可選填)")]
    public UnitState CurrentState = UnitState.Idle;
    [Header("角色目前血量(自動輸入)")]
    public int CurrentHP = 100;
    public int CurrentHp { get => CurrentHP; set => throw new NotImplementedException(); }
    public float CurrentTime = 0;
    public UnitModel unitModel;
    [Header("目前鎖定敵方")]
    public IDamageable Target;
    [Header("敵方層級(自動輸入)")]
    public LayerMask enemyLayer;
    public ISpecialty chooseSpecialty;
    [Header("物資用 單位建築不適用")]
    public ObjectType ObjType;


    //for testing
    public UnitStats ShowTarget;
    public Action OnDeathAction;
    public Action OnHitAction;
    //預設移動區域
    public Collider2D movementArea;
    LayerMask neutralObjectLayer = 8;

    public UnitModel SetUnitModel()
    {
        switch (ChooseUnitModel)
        {
            case UnitModelType.BasicMeleeUnitModel:
                unitModel = ScriptableManager.Instance.BasicMeleeModel;
                return ScriptableManager.Instance.BasicMeleeModel;
            case UnitModelType.BasicRangeUnitModel:
                unitModel = ScriptableManager.Instance.BasicRangeModel;
                return ScriptableManager.Instance.BasicRangeModel; ;
            case UnitModelType.ArcherTowerModel:
                unitModel = ScriptableManager.Instance.ArcherTowerModel;
                return ScriptableManager.Instance.ArcherTowerModel;
            case UnitModelType.CastleModel:
                unitModel = ScriptableManager.Instance.CastleModel;
                return ScriptableManager.Instance.CastleModel;
            default:
                return null;
        }
    }
    public IEnumerator DelaySetState(float time)
    {
        yield return new WaitForSeconds(time);
        SetUnitState();
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

    public void SetAllLayer()
    {
        enemyLayer = LayerMask.GetMask("TeamRed", "TeamBlue" , "TeamGreen");
        enemyLayer &= ~(1 << (int)TeamColor);
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
    //public void SearchEnemy()
    //{
    //    if (Target != null)
    //    {
    //        MonoBehaviour targetMonoBehaviour = Target as MonoBehaviour;
    //        var distance = Vector2.Distance(transform.position, targetMonoBehaviour.transform.position);
    //        if (distance <= unitModel.AttackRange)
    //            // 如果當前目標仍在範圍內，則不進行新的搜尋
    //            return;
    //        else
    //            currentSearchRange = distance;
    //    }
    //    //會依據敵方標籤搜尋,不會隨便找中立物件
    //    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, currentSearchRange, enemyLayer);
    //    float closestDistance = Mathf.Infinity;
    //    Collider2D current = null;
    //    foreach (Collider2D collider in hitColliders)
    //    {
    //        float distance = (transform.position - collider.transform.position).sqrMagnitude;
    //        //要節省效能時再重新開啟
    //        //if (distance < unitModel.AttackRange)
    //        //{
    //        //    Enemy = collider.GetComponent<UnitController>();
    //        //    break;
    //        //}
    //        if (distance < closestDistance)
    //        {
    //            closestDistance = distance;
    //            current = collider;
    //        }
    //    }
    //    if (current != null)
    //    {
    //        Target = current.GetComponent<IDamageable>();
    //        ShowTarget = Target as UnitStats;
    //    }
    //    else
    //    {
    //        currentSearchRange = unitModel.SearchRange;
    //    }
    //}

    //限定單次可搜尋目標上限
    private Collider2D[] results = new Collider2D[10];
    public void  SearchEnemy()
    {
        if (Target!= null && Target.CurrentHp <= 0)
        {
            Target = null;
        }
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, unitModel.SearchRange, results, enemyLayer);

        Collider2D nearestEnemy = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D enemy = results[i];
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null)
        {
            Target = nearestEnemy.GetComponent<IDamageable>();
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

    public IEnumerator Attack(float hitAnimationStartTime = 0)
    {
        SetUnitState(UnitState.Attack);
        //CurrentTime = unitModel.moveTime;
        yield return new WaitForSeconds(hitAnimationStartTime);
        if (Target != null)
            Target.GetHurt(UnityEngine.Random.Range(0, unitModel.AttackDamage));
        //if (Target.CurrentHp <= 0)
        //{
        //    Target = null;
        //}
    }

    public Collider2D FindNeutralObject()
    {
        neutralObjectLayer = LayerMask.GetMask("NeutralObject");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchNetutralRange, neutralObjectLayer);
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

    public void DelayIdle()
    {
        var waitTime = UnityEngine.Random.Range(0, unitModel.randomIdleTime);
        Invoke("setIdle", waitTime);
    }
    void setIdle()
    {
        CurrentState = UnitState.Idle;
    }

    public void SetSpecialty()
    {
        string loadSpecialtyString = UnitSpecialty.ToString();
        if (loadSpecialtyString == "Nothing")
            return;
        //字串需要與讀取的物件完全相同
        chooseSpecialty = Resources.Load<GameObject>("LoadSpecialtyPrefab/" + loadSpecialtyString).GetComponent<ISpecialty>();
    }

    public ObjectType GetObjectType()
    {
        return ObjType;
    }
}
