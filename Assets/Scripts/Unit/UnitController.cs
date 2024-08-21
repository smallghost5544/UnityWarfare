
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnitStats;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private IDamageable target { get => unitStats.Target; }
    //private UnitController Enemy;
    private Vector2 _targetPosition;
    /// <summary>
    /// 移動目的地
    /// </summary>
    public Vector2 targetPosition
    {
        get { return _targetPosition; }
        set
        {
            _targetPosition = unitStats.RoundToOneDecimal(value);
            float direction = _targetPosition.x - transform.position.x;
            if (Mathf.Abs(direction) > 0.1f)
                unitView.ChangeToward(direction);
        }
    }
    private Vector3 lastPosition;
    public string teamString;
    //use for archer
    public bool isArcher = false;
    public bool stayidle = false;
    private UnitModel unitModel;
    private UnitStats unitStats;
    private UnitView unitView;

    private void Awake()
    {
        unitStats = GetComponent<UnitStats>();
        unitView = GetComponent<UnitView>();
        unitModel = unitStats.unitModel;
        lastPosition = transform.position;
        unitStats.OnDeathAction = HandleDeathAction;
        unitStats.OnHitAction = unitView.OnHit;
    }
    private void OnEnable()
    {
        unitStats.FindMovementArea();
        unitStats.SetCurentHP();
        unitView.GetAnimator();
        unitView.GetCollider();
        StopAllCoroutines();
        CancelInvoke();
        gameObject.layer = unitStats.TeamNumer;
        InvokeRepeating("UnitStatsSearch", 0f, unitModel.searchTime);
        float randomMoveTime = Random.Range(unitModel.moveTime * 0.95f, unitModel.moveTime);
        float randomStartTime = Random.Range(0, 1);
        //是否改成 做完一個動作接續下一個behavior
        InvokeRepeating("UnitBehavior", randomStartTime, randomMoveTime);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    //IEnumerator UnitBehavior()
    //{
    //    if (unitStats.CurrentState != UnitState.Idle)
    //        yield break;
    //    if (target == null)
    //    {
    //        print("notarget");
    //        UnCombatActions();
    //    }
    //    if (target != null)
    //        WalkOrAttack();
    //    yield return new WaitForSeconds(1);
    //    StartCoroutine(UnitBehavior()); 
    //}
    void UnitBehavior()
    {
        if (unitStats.CurrentState != UnitState.Idle)
            return;
        if (target == null)
        {
            print("notarget");
            UnCombatActions();
        }
        if (target != null)
            WalkOrAttack();
    }
    /// <summary>
    /// 角色行為
    /// </summary>
    void UnCombatActions()
    {
        if (stayidle)
            return;
        unitStats.SetUnitState(UnitState.Patrol);
        switch (Random.Range(0, 4))
        {
            case 0:
                //選擇新目標並前進
                GetTargetAndWalk();
                break;
            case 1:
                //發呆
                StartCoroutine(unitStats.Idle());
                break;
            case 2:
                unitStats.DoExpert();
                break;
            case 3:
                GetResource();
                break;
        }
    }
    /// <summary>
    /// 範圍內隨機選擇目的地
    /// </summary>
    public void GetTargetAndWalk()
    {
        targetPosition = unitStats.RandomSelectTarget();
        StartCoroutine(WalkToTarget());
    }

    public IEnumerator WalkToTarget(UnitState nextState = UnitState.Idle)
    {
        unitStats.SetUnitState(UnitState.Walk);
        unitView.StartWalkAnimation(true);
        while (unitStats.CurrentState == UnitState.Walk)
        {
            yield return null;
            //WalkAnimationCheck();
            if (isArcher)
            {
                if (target != null && Vector3.Magnitude((target as MonoBehaviour).transform.position - transform.position) < unitModel.AttackRange)
                    unitStats.SetUnitState(UnitState.Idle);
            }
            Vector2 currentPosition = transform.position;
            // 計算方向向量
            Vector2 direction = (targetPosition - currentPosition).normalized;
            // 計算新位置
            Vector2 newPosition = currentPosition + direction * unitModel.moveSpeed * Time.deltaTime;
            // 更新位置
            transform.position = newPosition;
            // 檢查是否到達目標位置
            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                unitStats.SetUnitState(nextState);
                unitView.StartWalkAnimation(false);
                // 停止移動，並將位置設為精確的目標位置
                transform.position = targetPosition;
            }
        }
    }

    /// <summary>
    /// 針對角色的指令,輸入命令字串,目的地,面向角度1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        if (command == "LineUp")
        {
            StopAllCoroutines();
            //關閉collider避免碰撞改變陣行
            unitView.SetColldier(true);
            targetPosition = target;
            yield return StartCoroutine(WalkToTarget(UnitState.Commannd));
            unitView.SetColldier(false);
            unitView.ChangeToward(faceAngle);
        }
        yield return null;
        if (command == "TakeABreak")
        {
            unitStats.SetUnitState(UnitState.Idle);
            unitView.SetColldier(false);
        }
    }

    //防守的一方可以用,fix目前表現不佳 走路會卡
    void WalkAnimationCheck()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        if (distance > unitModel.moveThreshold)
        {
            unitView.StartWalkAnimation(true);
            lastPosition = transform.position;
        }
        else
            unitView.StartWalkAnimation(false);
    }

    //InvokeRepeat
    void UnitStatsSearch()
    {
        //還沒走到目標前面 目標已消失要換目標
        if (target != null && (target as MonoBehaviour).gameObject.activeSelf == false )
        {
            unitStats.Target = null;
            unitStats.SetUnitState(UnitState.Idle);
            unitView.StartWalkAnimation(false);
        }
        unitStats.SearchEnemy();
        if (unitStats.Target != null)
        {
            MonoBehaviour targetMonoBehaviour = target as MonoBehaviour;
            unitStats.SetUnitState(UnitState.Idle);
            unitView.StartWalkAnimation(false);
        }
    }

    void WalkOrAttack()
    {
        if (!(target as MonoBehaviour).gameObject.activeInHierarchy || (target.CurrentHp <= 0))
        {
            unitStats.SetUnitState(UnitState.Idle);
            unitStats.Target = null;
            return;
        }
        if (Vector3.Magnitude((target as MonoBehaviour).gameObject.transform.position - transform.position) > unitModel.AttackRange)
        {
            targetPosition = (target as MonoBehaviour).transform.position;
            StartCoroutine(WalkToTarget());
        }
        else if (target != null)
            DirectAttack();
    }
    void DirectAttack()
    {
        float time = unitView.AttackAnimation(unitModel.attackType, target);
        StartCoroutine(unitStats.Attack(time)); 
        StartCoroutine(nextMoveTime());
    }
    /// <summary>
    /// 戰鬥中下次行動時間
    /// </summary>
    IEnumerator nextMoveTime()
    {
        while (unitStats.CurrentState == UnitState.Attack)
        {
            unitStats.CurrentTime -= Time.deltaTime;
            if (unitStats.CurrentTime <= 0.01f)
            {
                unitStats.SetUnitState(UnitState.Idle);
                yield break;
            }
            yield return null;
        }
    }
    public void HandleDeathAction()
    {
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        unitView.DieAnimation();
        var test = GameManager.Instance.GetComponentInChildren<TestButtonFunctions>();
        test.unitActions.Remove(this);
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder -= 1;
        gameObject.layer = 0;
        unitStats.SetUnitState(UnitState.Dead);
        CancelInvoke();
        yield return new WaitForSeconds(1f);
        ResetDied();
    }

    void ResetDied()
    {
        unitStats.CurrentHP = unitStats.unitModel.MaxHP;
        unitStats.SetUnitState(UnitState.Idle);
        gameObject.transform.position = new Vector3(100, 100, 100);
        unitView.SetColldier(false);
        unitStats.Target = null;
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder -= 5;
        //objectpool回收
        ObjectPool.Instance.Return(teamString, gameObject);
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
                targetPosition = transform.position;
        }
        if (collision.gameObject.CompareTag("Neutral"))
        {
            targetPosition = transform.position;
        }
    }

    private void GetResource()
    {
        print("getresource");
        var obj = unitStats.FindNeutralObject();
        if (obj == null || !obj.gameObject.activeInHierarchy)
        {
            unitStats.SetUnitState(UnitState.Idle);
            return;
        }

        if (Vector3.Magnitude(obj.gameObject.transform.position - transform.position) > unitModel.AttackRange)
        {
            targetPosition = obj.transform.position;
            StartCoroutine(WalkToTarget());
        }
        else if (target != null)
            DirectAttack();
    }

}
