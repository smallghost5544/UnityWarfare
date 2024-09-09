using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildingController : MonoBehaviour
{
    [SerializeField]
    private IDamageable target { get => unitStats.Target; }
    public string teamString;
    private UnitModel unitModel;
    private UnitStats unitStats;
    private UnitView unitView;

    private void Awake()
    {
        unitStats = GetComponent<UnitStats>();
        unitView = GetComponent<UnitView>();
        unitModel = unitStats.unitModel;
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
        //是否改成 做完一個動作接續下一個behavior
        InvokeRepeating("UnitBehavior", 0, unitModel.attackCD);
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
            return;
        if (target != null)
            CheckAttack();
    }

    //InvokeRepeat
    void UnitStatsSearch()
    {
        //還沒走到目標前面 目標已消失要換目標
        if (unitStats.Target != null && (target as MonoBehaviour).gameObject.activeSelf == false)
        {
            unitStats.Target = null;
            unitStats.SetUnitState(UnitState.Idle);
            unitView.StartWalkAnimation(false);
        }
        unitStats.SearchEnemy();
    }

    void CheckAttack()
    {
        if (!(target as MonoBehaviour).gameObject.activeInHierarchy || (target.CurrentHp <= 0))
        {
            unitStats.SetUnitState(UnitState.Idle);
            unitStats.Target = null;
            return;
        }
        bool attackable = Vector3.Magnitude((target as MonoBehaviour).gameObject.transform.position - transform.position) < unitModel.AttackRange;

        if (attackable && target != null)
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

    public void HandleDeathAction()
    {
        if (gameObject.activeSelf == true)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //預設為object pool 可以更加優化
        var layer = GetComponentInChildren<SpriteRenderer>();
        layer.sortingOrder -= 1;
        gameObject.layer = 0;
        unitStats.SetUnitState(UnitState.Dead);
        CancelInvoke();
        gameObject.SetActive(false);
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
    }


}
