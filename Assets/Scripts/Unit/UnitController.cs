
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
        //�O�_�令 �����@�Ӱʧ@����U�@��behavior
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
    /// ����欰
    /// </summary>
    void UnCombatActions()
    {
        if (stayidle)
            return;
        unitStats.SetUnitState(UnitState.Patrol);
        switch (Random.Range(0, 4))
        {
            case 0:
                //��ܷs�ؼШëe�i
                GetTargetAndWalk();
                break;
            case 1:
                //�o�b
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
    /// �d���H����ܥت��a
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
            }
        }
    }

    /// <summary>
    /// �w�﨤�⪺���O,��J�R�O�r��,�ت��a,���V����1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        if (command == "LineUp")
        {
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

    //InvokeRepeat
    void UnitStatsSearch()
    {
        //�٨S����ؼЫe�� �ؼФw�����n���ؼ�
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
    /// �԰����U����ʮɶ�
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
        //objectpool�^��
        ObjectPool.Instance.Return(teamString, gameObject);
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
