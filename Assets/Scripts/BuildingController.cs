using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static BuildingTowerSpecialty;


public enum BuildingType
{
    ArcherTower,
    Castle
}

public class BuildingController : MonoBehaviour
{
    [SerializeField]
    private IDamageable target { get => unitStats.Target; }
    private UnitModel unitModel;
    private UnitStats unitStats;
    private UnitView unitView;
    SpecialtyModel specialtyModel;
    public bool NeedBuildingFX = true;
    [Header("選擇建築物類型")]
    public BuildingType BuildingTypeChoose;
    private void Awake()
    {
        unitStats = GetComponent<UnitStats>();
        unitView = GetComponent<UnitView>();
        unitModel = unitStats.unitModel;
        unitStats.OnDeathAction = HandleDeathAction;
        unitStats.OnHitAction = unitView.OnHit;
        //specialtyModel  = Resources.Load<SpecialtyModel>("ScriptableObjectData/SpecialtyData");
        specialtyModel = ScriptableManager.Instance.AllSpecialtyModel;
    }

    private void OnEnable()
    {
        unitModel = unitStats.SetUnitModel();
        unitStats.SetCurentHP();
        unitStats.SetAllLayer();
        unitStats.SetUnitState(UnitState.Building);
        unitView.GetAnimator();
        unitView.GetCollider();
        StopAllCoroutines();
        CancelInvoke();
        gameObject.layer = (int)unitStats.TeamColor;
        if (NeedBuildingFX)
            StartCoroutine(BuildingScalingFX(specialtyModel.BuildingArcherTowerTime, specialtyModel.BuildingArcherTowerIntervalTime));
        InvokeRepeating("UnitStatsSearch", 0f, unitModel.searchTime);
        //是否改成 做完一個動作接續下一個behavior
        InvokeRepeating("UnitBehavior", 0, unitModel.attackCD);
        if (NeedBuildingFX)
            Invoke(nameof(FinishBuilding), specialtyModel.BuildingArcherTowerTime);
        else
            FinishBuilding();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    void FinishBuilding()
    {
        unitStats.SetUnitState(UnitState.Idle);
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
        if (unitStats.CurrentState == UnitState.Building)
            return;
        //還沒走到目標前面 目標已消失要換目標
        if (unitStats.Target != null && (target as MonoBehaviour).gameObject.activeSelf == false)
        {
            unitStats.Target = null;
            unitStats.SetUnitState(UnitState.Idle);
        }
        unitStats.SearchEnemy();
    }

    void CheckAttack()
    {
        if (unitStats.CurrentState == UnitState.Building)
            return;
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
        unitView.AttackAnimation(unitModel.attackType, target, enemyLayer: unitStats.enemyLayer);
        unitStats.CurrentTime = unitModel.attackCD;
        unitStats.SetUnitState(UnitState.Attack);
        //StartCoroutine(unitStats.Attack(unitModel.attackAnimationHitTime));
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

    public void HandleDeathAction()
    {
        if (gameObject.activeSelf == true)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //預設為object pool 可以更加優化
        unitView.DieAnimation();
        CallBuildingTowerSpecialty();
        yield return new WaitForSeconds(1.5f);
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

    //確保沒有建築物的目標地上不被錯誤判定有建築物
    void CallBuildingTowerSpecialty()
    {
        //呼叫buildinngTowerSpecialty
        var List = Resources.Load<GameObject>("LoadSpecialtyPrefab/BuildTower").GetComponent<BuildingTowerSpecialty>().buildingPlaceLsit;
        for (int i = 0; i < List.Count; i++)
        {
            if (List[i].builldController == this)
            {
                List[i].hasSpace = true;
                List[i].builldController = null;
                break;
            }
        }
    }

    /// <summary>
    ///模擬建築被敲擊時,晃動之特效
    /// </summary>
    /// <param name="fullTime"></param>
    /// <param name="intervalTime"></param>
    /// <returns></returns>
    IEnumerator BuildingScalingFX(float fullTime, float intervalTime)
    {
        var originalScale = gameObject.transform.localScale;
        var newScale = originalScale;
        var originTransform = gameObject.transform.position;
        var changeTransform = originTransform;
        newScale *= 0.95f;
        changeTransform.x += 0.02f;
        float recoverTime = 0.1f;
        yield return new WaitForSeconds(0.15f);
        gameObject.transform.localScale = newScale;
        gameObject.transform.position = changeTransform;
        yield return new WaitForSeconds(recoverTime);
        gameObject.transform.localScale = originalScale;
        gameObject.transform.position = originTransform;
        while (fullTime >= 0)
        {
            yield return new WaitForSeconds(intervalTime - recoverTime);
            gameObject.transform.localScale = newScale;
            gameObject.transform.position = changeTransform;
            yield return new WaitForSeconds(recoverTime);
            gameObject.transform.localScale = originalScale;
            gameObject.transform.position = originTransform;
            fullTime -= intervalTime;
            if (fullTime < intervalTime)
                yield break;
        }
    }
}
