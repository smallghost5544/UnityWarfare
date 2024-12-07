
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

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
            {
                unitView.ChangeToward(direction);
                unitView.ChangeNameTagDirection(direction);
            }
        }
    }
    private Vector3 lastPosition;
    [Header("影響物件池生成回收(不同單位不重複)")]
    public string UnitFormerName;
    public bool isArcher = false;
    public bool stayidle = false;
    bool getResource = false;
    /// <summary>
    /// unit Scriptable
    /// </summary>
    private UnitModel unitModel;
    /// <summary>
    /// unit 邏輯
    /// </summary>
    private UnitStats unitStats;
    /// <summary>
    /// unit視圖
    /// </summary>
    private UnitView unitView;
    [Header("走到目標,發呆,專長,採資源")]
    public List<int> ActionProb = new List<int>() { 1, 1, 1, 1 };
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
        //unitStats.FindMovementArea();
        unitStats.SetCurentHP();
        unitStats.SetSpecialty();
        unitStats.SetAllLayer();
        unitModel = unitStats.SetUnitModel();
        unitView.ShowNameOnStart();
        unitView.GetAnimator();
        unitView.GetCollider();
        unitView.SaveOriginalWeapon();
        StopAllCoroutines();
        CancelInvoke();
        gameObject.layer = (int)unitStats.TeamColor;
        InvokeRepeating(nameof(UnitStatsSearch), 0f, unitModel.searchTime);
        float randomMoveTime = Random.Range(unitModel.moveTime, unitModel.moveTime);
        float randomStartTime = Random.Range(0, 1);
        //是否改成 做完一個動作接續下一個behavior
        InvokeRepeating(nameof(UnitBehavior), randomStartTime, randomMoveTime);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
    public bool notAttack = false;
    void UnitBehavior()
    {
        if (notAttack)
            return;
        if (unitStats.CurrentState != UnitState.Idle)
            return;
        if (target == null)
        {
            UnCombatActions();
        }
        else if (target != null)
            WalkOrAttack();
    }

    int ActionProbability()
    {
        int allProbCount = 0;
        foreach (var prob in ActionProb)
        {
            allProbCount += prob;
        }
        int chooseAction = Random.Range(0, allProbCount + 1);
        for (int i = 0; i < ActionProb.Count; i++)
        {
            if (ActionProb[i] == 0)
                continue;

            chooseAction -= ActionProb[i];
            if (chooseAction <= 0)
            {
                return i;
            }
        }
        //最大值
        print(3);
        return ActionProb.Count - 1;
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

        // int randomValue = Random.Range(1, maxRandom);
        switch (ActionProbability())
        {
            case 0:
                //選擇新目標並前進
                GetTargetAndWalk();
                break;
            case 1:
                //發呆
                unitStats.DelayIdle();
                break;
            case 2:
                //執行專長
                DoSpecialize();
                break;
            case 3:
                //尋找資源
                StartCoroutine(GetResource());
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

    public IEnumerator WalkToTarget(UnitState nextState = UnitState.Idle, bool needAttack = true)
    {
        unitStats.SetUnitState(UnitState.Walk);
        unitView.StartWalkAnimation(true);

        float stopThreshold = 0.5f; // 移動量的閾值
        float checkInterval = 0.5f; // 檢查間隔
        float elapsedTime = 0f; // 計時器
        int stuckCount = 0; // 卡住計數器
        int maxStuckCount = 10; // 卡住的最大次數
        Vector2 lastPosition = transform.position; // 紀錄上一個位置

        while (unitStats.CurrentState == UnitState.Walk)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
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
                if (target != null && unitStats.CurrentTime <= 0.1f && needAttack)
                {
                    DirectAttack();
                }
            }
            if (CheckStuck(ref stuckCount, ref elapsedTime, checkInterval, stopThreshold, maxStuckCount, lastPosition, currentPosition))
            {
                unitStats.SetUnitState(UnitState.Idle);
                unitView.StartWalkAnimation(false);
                yield break; // 停止協程
            }

            lastPosition = currentPosition; // 更新最後位置
        }
    }

    private bool CheckStuck
        (ref int stuckCount, ref float elapsedTime,
        float checkInterval, float stopThreshold,
        int maxStuckCount, Vector2 lastPosition,
        Vector2 currentPosition)
    {
        if (elapsedTime >= checkInterval)
        {
            float movedDistance = Vector2.Distance(lastPosition, currentPosition);

            if (movedDistance < stopThreshold)
            {
                stuckCount++;
                if (stuckCount >= maxStuckCount)
                {
                    Debug.Log("Unit stuck detected. Stopping movement.");
                    return true; // 表示卡住了
                }
            }
            else
            {
                stuckCount = 0; // 重置卡住計數器
            }

            elapsedTime = 0f; // 重置計時器
        }

        return false; // 未卡住
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
            unitStats.Target = null;
            targetPosition = transform.position;
            unitStats.SetUnitState(UnitState.Idle);
            //StartCoroutine(unitStats.DelaySetState(unitModel.moveTime));
            return;
        }
        bool notAttackable = Vector3.Magnitude((target as MonoBehaviour).gameObject.transform.position - transform.position) > unitModel.AttackRange;
        if (notAttackable)
        {
            targetPosition = (target as MonoBehaviour).transform.position;
            StartCoroutine(WalkToTarget());
        }
        else if (target != null && (target as MonoBehaviour).gameObject.activeInHierarchy)
        {
            DirectAttack();
        }
    }
    void DirectAttack()
    {
        unitView.AttackAnimation(unitModel.attackType, target, enemyLayer: unitStats.enemyLayer);
        unitStats.CurrentTime = unitModel.attackCD;
        if (isArcher)
        {
            unitStats.SetUnitState(UnitState.Attack);
        }
        else
        {
            StartCoroutine(unitStats.Attack(unitModel.attackAnimationHitTime));
        }
        StartCoroutine(nextAttackTime());
    }

    /// <summary>
    /// 戰鬥中下次行動時間
    /// </summary>
    IEnumerator nextAttackTime()
    {
        while (unitStats.CurrentState == UnitState.Attack)
        {
            unitStats.CurrentTime -= 0.1f;
            if (unitStats.CurrentTime <= 0.01f)
            {
                unitStats.CurrentTime = 0;
                unitStats.SetUnitState(UnitState.Idle);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    /// <summary>
    /// 針對角色的指令,輸入命令字串,目的地,面向角度1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        if (command == "LineUp")
        {
            unitStats.SetUnitState(UnitState.Commannd);
            unitStats.Target = null;
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

    Rigidbody2D rb;
    IEnumerator Die()
    {
        //預設為object pool 可以更加優化
        unitView.DieAnimation();
        //若正在挖礦則解除動作與排除該對象
        if (currentMine != null)
        {
            currentMine.MinerList.Remove(this);
            currentMine.currentMinerCount--;
        }
        getResource = false;
        //gamemanager相關
        var test = GameManager.Instance.GetComponentInChildren<TestButtonFunctions>();
        test.unitActions.Remove(this);

        CancelInvoke();
        StopCoroutine(WalkToTarget());

        //view 相關
        unitView.SetColldier(true);
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
        unitView.BuildingSpecialtyFinishAction();
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder -= 1;
        gameObject.layer = 0;

        unitStats.SetUnitState(UnitState.Dead);
        yield return new WaitForSeconds(unitView.knockDownFadeTime);
        ResetDied();
    }

    void ResetDied()
    {
        rb.simulated = true;
        unitStats.CurrentHP = unitStats.unitModel.MaxHP;
        unitStats.SetUnitState(UnitState.Idle);
        gameObject.transform.position = new Vector3(100, 100, 100);
        unitView.SetColldier(false);
        unitView.RecoverAlpha();
        unitStats.Target = null;
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder = 5;
        //objectpool回收
        ObjectPool.Instance.Return(UnitFormerName, gameObject);
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
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (getResource == false)
            return;
        //碰到礦物後,會進行挖礦動作
        if (collision.gameObject.CompareTag("Mine"))
        {
            targetPosition = transform.position;
            //unitStats.SetUnitState(UnitState.Mining);
        }
        if (collision.gameObject.CompareTag("Chest"))
        {
            targetPosition = transform.position;
            //unitStats.SetUnitState(UnitState.Attack);
        }
    }

    bool archerSaving;
    MineControl currentMine;
    private IEnumerator GetResource()
    {
        var obj = unitStats.FindNeutralObject();
        if (obj == null || !obj.gameObject.activeInHierarchy)
        {
            unitStats.SetUnitState(UnitState.Idle);
            yield break;
        }
        targetPosition = obj.transform.position;
        var targetType = target.GetObjectType();
        //挖礦執行以下功能
        if (targetType == ObjectType.Mine)
        {
            archerSaving = isArcher;
            isArcher = false;
            currentMine = target as MineControl;
            //單一挖礦的最大人數限制
            //fix正在挖礦的不會撤出
            if (currentMine.currentMinerCount > currentMine.MaxMinerCount)
            {
                cancelMine();
                yield break;
            }
            getResource = true;
            currentMine.AddMinerInSpace(this);
            yield return StartCoroutine(WalkToTarget(UnitState.Mining, needAttack: false));
            yield return StartCoroutine(MineAction());
        }
        else if (targetType == ObjectType.Chest)
        {
            //getresource如何解除為false
            getResource = true;
            WalkOrAttack();
        }
        else
        {
            unitStats.SetUnitState(UnitState.Idle);
        }
    }

    IEnumerator MineAction()
    {
        while (unitStats.Target != null)
        {
            //希望靠近一點去進行挖礦動作
            if (Vector2.Distance(transform.position, (target as MonoBehaviour).transform.position) > 0.2f)
            {
                yield return StartCoroutine(WalkToTarget(UnitState.Mining, needAttack: false));
            }
            unitView.MineAction();
            yield return new WaitForSeconds(0.6f);
            if (target != null)
                target.GetHurt(unitModel.AttackDamage);
            yield return new WaitForSeconds(unitModel.usualActionIntervalTime - 0.3f);
            //fix CALL GAMANGER 給錢
        }
        cancelMine();
        yield return null;
    }


    public void cancelMine()
    {
        if (unitStats.CurrentState == UnitState.Walk)
            return;
        isArcher = archerSaving;
        getResource = false;
        unitView.BuildingSpecialtyFinishAction();
        //unitStats.SetUnitState(UnitState.Idle);
        unitStats.Target = null;
        GetTargetAndWalk();
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
            StartCoroutine(unitStats.chooseSpecialty.DoSpecialize(gameObject.transform.position,
                                                                                                                   this.gameObject,
                                                                                                                    unitView,
                                                                                                                    unitStats));
        }
        else
            unitStats.SetUnitState();
    }



}
