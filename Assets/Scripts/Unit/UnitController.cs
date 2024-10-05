
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


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
    public bool isArcher = false;
    public bool stayidle = false;
    /// <summary>
    /// unit Scriptable
    /// </summary>
    private UnitModel unitModel;
    /// <summary>
    /// unit 邏輯
    /// </summary>
    public UnitStats unitStats;
    /// <summary>
    /// unit視圖
    /// </summary>
    public UnitView unitView;

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
        unitStats.SetSpecialty();
        unitView.GetAnimator();
        unitView.GetCollider();
        unitView.SaveOriginalWeapon();
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

    void UnitBehavior()
    {
        if (unitStats.CurrentState != UnitState.Idle)
            return;
        if (target == null)
        {
            UnCombatActions();
        }
        else if (target != null)
            WalkOrAttack();
    }
    /// <summary>
    /// 角色行為
    /// </summary>
    void UnCombatActions()
    {
        //fix 調整整體行動機率
        if (stayidle)
            return;
        //避免過快出去,重複觸發
        unitStats.SetUnitState(UnitState.Patrol);
        int randomValue = Random.Range(1, 12);
        switch (randomValue)
        {
            case int n when (n <= 8):
                //選擇新目標並前進
                GetTargetAndWalk();
                break;
            case 9:
                //發呆
                unitStats.DelayIdle();
                break;
            case 10:
                //執行專長
                DoSpecialize();
                break;
            case 11:
                //尋找資源
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
                {
                    unitStats.SetUnitState(UnitState.Idle);
                    unitView.StartWalkAnimation(false);
                }
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
                if (target != null && unitStats.CurrentTime <= 0.1f)
                    DirectAttack();
            }
        }
    }


    //InvokeRepeat
    void UnitStatsSearch()
    {
        if (unitStats.CurrentState == UnitState.Building)
            return;
        //還沒走到目標前面 目標已消失要換目標
        if (unitStats.Target != null && (target as MonoBehaviour).gameObject.activeSelf == false)
        {
            unitStats.Target = null;
            unitStats.SetUnitState(UnitState.Idle);
            unitView.StartWalkAnimation(false);
        }
        unitStats.SearchEnemy();
        if (unitStats.Target != null)
        {
            targetPosition = (target as MonoBehaviour).transform.position;
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
        bool attackable = Vector3.Magnitude((target as MonoBehaviour).gameObject.transform.position - transform.position) > unitModel.AttackRange;
        if (attackable)
        {
            targetPosition = (target as MonoBehaviour).transform.position;
            StartCoroutine(WalkToTarget());
        }
        else if (!attackable && target != null && (target as MonoBehaviour).gameObject.activeInHierarchy)
            DirectAttack();
    }
    void DirectAttack()
    {
        unitView.AttackAnimation(unitModel.attackType, target);
        unitStats.CurrentTime = unitModel.attackCD;
        StartCoroutine(unitStats.Attack(unitModel.attackAnimationHitTime));
        StartCoroutine(nextAttackTime());
    }
    /// <summary>
    /// 戰鬥中下次行動時間
    /// </summary>
    IEnumerator nextAttackTime()
    {
        while (unitStats.CurrentState == UnitState.Attack)
        {
            unitStats.CurrentTime -= 0.05f;
            if (unitStats.CurrentTime <= 0.01f)
            {
                unitStats.CurrentTime = 0;
                unitStats.SetUnitState(UnitState.Idle);
                yield break;
            }
            yield return new WaitForSeconds(0.05f);
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

    public void HandleDeathAction()
    {
        if (gameObject.activeSelf == true)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //預設為object pool 可以更加優化
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
        unitView.RecoverAlpha();
        unitStats.Target = null;
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder = 5;
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
    public void DoSpecialize()
    {
        if (unitStats.chooseSpecialty != null)
        {
            //fix 要能隨著技能改為特定目標
            StartCoroutine(unitStats.chooseSpecialty.DoSpecialize(gameObject.transform.position , 
                                                                                                                   this.gameObject ,
                                                                                                                    unitView , 
                                                                                                                    unitStats));
        }
        else
            unitStats.SetUnitState();
    }

}
