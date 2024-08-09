using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnitStats;

public class UnitController : MonoBehaviour
{
    [Header("敵方層級")]
    public LayerMask enemyLayer;
    [Header("目前鎖定敵方")]
    public UnitController Enemy;
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
    //public bool inAction = false;
    bool isArriveTarget = true;
    public string teamString;
    //use for archer
    public bool isArcher = false;
    public bool stayidle = false;
    private UnitModel unitModel;
    private UnitStats unitStats;
    private UnitView unitView;

    private void Awake()
    {
        lastPosition = transform.position;
        unitStats = GetComponent<UnitStats>();
        unitModel = unitStats.unitModel;
        unitView = GetComponent<UnitView>();
    }
    private void OnEnable()
    {
        unitStats.FindMovementArea();
        unitStats.SetCurentHP();
        unitView.GetAnimator();
        unitView.GetCollider();
        StopAllCoroutines();
        CancelInvoke("UnitStatsSearch");
        InvokeRepeating("UnitStatsSearch", 0f, 1.5f);
        gameObject.layer = unitStats.TeamNumer;
    }


    void Update()
    {

        //if (unitStats.CurrentState == UnitState.Idle && Enemy == null)
        //    UnCombatActions();
        //else if (unitStats.CurrentState == UnitState.Idle && Enemy != null)
        //    WalkOrAttack();
        if (unitStats.CurrentState == UnitState.Idle)
        {
            if (Enemy == null)
                UnCombatActions();
            else
                WalkOrAttack();
        }
        if (unitStats.CurrentState != UnitState.Idle)
        {
            WalkAnimationCheck();
        }
        nextMoveTime();
    }
    /// <summary>
    /// 角色行為
    /// </summary>
    void UnCombatActions()
    {
        if (stayidle)
            return;
        unitStats.CurrentState = UnitState.Patrol;
        switch (Random.Range(0, 2))
        {
            case 0:
                //選擇新目標並前進
                GetTargetAndWalk();
                break;
            case 1:
                //發呆
                StartCoroutine(Idle());
                break;
                //case 2:
                //    WalkAnimationCheck ();
                //    break;
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

    public IEnumerator WalkToTarget()
    {
        isArriveTarget = false;
        unitView.StartWalkAnimation(true);
        while (!isArriveTarget)
        {
            yield return null;
            if (isArcher)
            {
                if (Enemy != null && Vector3.Magnitude(Enemy.gameObject.transform.position - transform.position) < unitModel.AttackRange)
                    isArriveTarget = true;
            }
            Vector2 currentPosition = transform.position;
            // 計算方向向量
            Vector2 direction = (targetPosition - currentPosition).normalized;
            // 計算新位置
            Vector2 newPosition = currentPosition + direction * unitModel.moveSpeed * Time.deltaTime;
            // 更新位置
            transform.position = newPosition;
            // 檢查是否到達目標位置
            if (Vector2.Distance(currentPosition, targetPosition) < 0.01f)
            {
                // 停止移動，並將位置設為精確的目標位置
                if (unitStats.CurrentState != UnitState.Commannd)
                    unitStats.SetUnitState(UnitState.Idle);
                isArriveTarget = true;
                unitView.StartWalkAnimation(false);
                transform.position = targetPosition;
            }
        }
    }

    IEnumerator Idle()
    {
        var waitTime = Random.Range(0, unitModel.randomIdleTime);
        yield return new WaitForSeconds(waitTime);
        unitStats.CurrentState = UnitState.Idle;
        //inAction = false;
    }

    /// <summary>
    /// 針對角色的指令,輸入命令字串,目的地,面向角度1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        if (command == "LineUp")
        {
            unitStats.SetUnitState(UnitState.Commannd);
            //關閉collider避免碰撞改變陣行
            unitView.SetColldier(true);
            targetPosition = target;
            yield return StartCoroutine(WalkToTarget());
            unitView.SetColldier(false);
            unitView.ChangeToward(faceAngle);
        }
        yield return null;
        if (command == "TakeABreak")
        {
            unitStats.SetUnitState(UnitState.Idle);
            Enemy = null;
            unitView.SetColldier(false);
        }
    }

    //防守的一方可以用
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
        unitStats.SearchEnemy();
        if (unitStats.Enemy != null)
            Enemy = unitStats.Enemy;
    }

    void WalkOrAttack()
    {
        unitStats.CurrentTime = Random.Range(1, unitModel.moveTime * 2);
        if (Enemy.gameObject.activeInHierarchy)
        {
            if (Vector3.Magnitude(Enemy.gameObject.transform.position - transform.position) > unitModel.AttackRange)
            {
                targetPosition = Enemy.transform.position;
                if (isArriveTarget)
                    StartCoroutine(WalkToTarget());
            }
            else
            {
                Attack();
            }
        }
    }
    void Attack()
    {
        unitStats.SetUnitState(UnitState.Attack);
        Enemy.GetHurt(Random.Range(0, unitModel.AttackDamage));
        unitView.AttackAnimation(unitModel.attackType, Enemy);
    }
    /// <summary>
    /// 戰鬥中下次行動時間
    /// </summary>
    void nextMoveTime()
    {
        if (unitStats.CurrentState == UnitState.Attack)
        {
            unitStats.CurrentTime -= Time.deltaTime;
            if (unitStats.CurrentTime <= 0.01f)
            {
                unitStats.SetUnitState(UnitState.Idle);
            }
        }
    }
    public void GetHurt(int damage)
    {
        unitStats.CurrentHP -= damage;
        if (unitStats.CurrentHP < 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        unitView.DieAnimation();
        var test = GameManager.Instance.GetComponentInChildren<TestButtonFunctions>();
        test.unitActions.Remove(this);
        isArriveTarget = true;
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder -= 1;
        gameObject.layer = 0;
        unitStats.SetUnitState(UnitState.Dead);
        yield return new WaitForSeconds(1f);
        //Destroy(unitAbility);
        //Destroy(this);
        ResetDied();
    }

    void ResetDied()
    {
        unitStats.CurrentHP = unitStats.unitModel.MaxHP;
        unitStats.SetUnitState(UnitState.Idle);
        gameObject.transform.position = new Vector3(100, 100, 100);
        unitView.SetColldier(false);
        Enemy = null;
        isArriveTarget = true;
        //CancelInvoke("SearchEnemy");
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
    }

}
