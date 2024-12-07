
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
    /// ���ʥت��a
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
    [Header("�v�T������ͦ��^��(���P��줣����)")]
    public string UnitFormerName;
    public bool isArcher = false;
    public bool stayidle = false;
    bool getResource = false;
    /// <summary>
    /// unit Scriptable
    /// </summary>
    private UnitModel unitModel;
    /// <summary>
    /// unit �޿�
    /// </summary>
    private UnitStats unitStats;
    /// <summary>
    /// unit����
    /// </summary>
    private UnitView unitView;
    [Header("����ؼ�,�o�b,�M��,�ĸ귽")]
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
        //�O�_�令 �����@�Ӱʧ@����U�@��behavior
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
        //�̤j��
        print(3);
        return ActionProb.Count - 1;
    }

    /// <summary>
    /// ����欰
    /// </summary>
    void UnCombatActions()
    {
        //fix �վ�����ʾ��v
        if (stayidle)
            return;
        //�קK�L�֥X�h,����Ĳ�o
        unitStats.SetUnitState(UnitState.Patrol);

        // int randomValue = Random.Range(1, maxRandom);
        switch (ActionProbability())
        {
            case 0:
                //��ܷs�ؼШëe�i
                GetTargetAndWalk();
                break;
            case 1:
                //�o�b
                unitStats.DelayIdle();
                break;
            case 2:
                //����M��
                DoSpecialize();
                break;
            case 3:
                //�M��귽
                StartCoroutine(GetResource());
                break;
        }
    }
    /// <summary>
    /// �d���H����ܥت��a
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

        float stopThreshold = 0.5f; // ���ʶq���H��
        float checkInterval = 0.5f; // �ˬd���j
        float elapsedTime = 0f; // �p�ɾ�
        int stuckCount = 0; // �d��p�ƾ�
        int maxStuckCount = 10; // �d���̤j����
        Vector2 lastPosition = transform.position; // �����W�@�Ӧ�m

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
            // �p���V�V�q
            Vector2 direction = (targetPosition - currentPosition).normalized;
            // �p��s��m
            Vector2 newPosition = currentPosition + direction * unitModel.moveSpeed * Time.deltaTime;
            // ��s��m
            transform.position = newPosition;
            // �ˬd�O�_��F�ؼЦ�m
            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                unitStats.SetUnitState(nextState);
                unitView.StartWalkAnimation(false);
                // ����ʡA�ñN��m�]����T���ؼЦ�m
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
                yield break; // �����{
            }

            lastPosition = currentPosition; // ��s�̫��m
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
                    return true; // ��ܥd��F
                }
            }
            else
            {
                stuckCount = 0; // ���m�d��p�ƾ�
            }

            elapsedTime = 0f; // ���m�p�ɾ�
        }

        return false; // ���d��
    }

    //InvokeRepeat
    void UnitStatsSearch()
    {
        if (unitStats.CurrentState == UnitState.Building)
            return;
        //�٨S����ؼЫe�� �ؼФw�����n���ؼ�
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
    /// �԰����U����ʮɶ�
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
    /// �w�﨤�⪺���O,��J�R�O�r��,�ت��a,���V����1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        if (command == "LineUp")
        {
            unitStats.SetUnitState(UnitState.Commannd);
            unitStats.Target = null;
            StopAllCoroutines();
            //����collider�קK�I�����ܰ}��
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
        //�w�]��object pool �i�H��[�u��
        unitView.DieAnimation();
        //�Y���b���q�h�Ѱ��ʧ@�P�ư��ӹ�H
        if (currentMine != null)
        {
            currentMine.MinerList.Remove(this);
            currentMine.currentMinerCount--;
        }
        getResource = false;
        //gamemanager����
        var test = GameManager.Instance.GetComponentInChildren<TestButtonFunctions>();
        test.unitActions.Remove(this);

        CancelInvoke();
        StopCoroutine(WalkToTarget());

        //view ����
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
        //objectpool�^��
        ObjectPool.Instance.Return(UnitFormerName, gameObject);
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
                targetPosition = transform.position;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (getResource == false)
            return;
        //�I���q����,�|�i����q�ʧ@
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
        //���q����H�U�\��
        if (targetType == ObjectType.Mine)
        {
            archerSaving = isArcher;
            isArcher = false;
            currentMine = target as MineControl;
            //��@���q���̤j�H�ƭ���
            //fix���b���q�����|�M�X
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
            //getresource�p��Ѱ���false
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
            //�Ʊ�a��@�I�h�i����q�ʧ@
            if (Vector2.Distance(transform.position, (target as MonoBehaviour).transform.position) > 0.2f)
            {
                yield return StartCoroutine(WalkToTarget(UnitState.Mining, needAttack: false));
            }
            unitView.MineAction();
            yield return new WaitForSeconds(0.6f);
            if (target != null)
                target.GetHurt(unitModel.AttackDamage);
            yield return new WaitForSeconds(unitModel.usualActionIntervalTime - 0.3f);
            //fix CALL GAMANGER ����
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

    //���u���@��i�H��,fix�ثe��{���� �����|�d
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
            //fix �n���H�ۧޯ�אּ�S�w�ؼ�
            StartCoroutine(unitStats.chooseSpecialty.DoSpecialize(gameObject.transform.position,
                                                                                                                   this.gameObject,
                                                                                                                    unitView,
                                                                                                                    unitStats));
        }
        else
            unitStats.SetUnitState();
    }



}
