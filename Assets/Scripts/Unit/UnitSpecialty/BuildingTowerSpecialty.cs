using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTowerSpecialty : MonoBehaviour, ISpecialty
{

    public class PositionStatus
    {
        public Vector2 position;
        public bool hasSpace = true;
        public BuildingController builldController;
        public PositionStatus(Vector3 position, bool status)
        {
            this.position = position;
            this.hasSpace = status;
        }
        private IEnumerator CheckHasSpaceAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            if (builldController == null)
            {
                hasSpace = true;
            }
        }
    }

    [SerializeField]
    GameObject RedArrowTower = null;
    [SerializeField]
    GameObject BlueArrowTower = null;
    [SerializeField]
    GameObject GreenArrowTower = null;
    [SerializeField]
    //true代表有位置
    public List<PositionStatus> buildingPlaceLsit = new List<PositionStatus>();
    public SpecialtyModel specialtyModel;
    public bool IsPassive { get => false; }
    public float specializeTime => specialtyModel.BuildingArcherTowerTime;
    float interval;


    public void SetResource()
    {
        //smallArrowTower = Resources.Load<GameObject>("Building/ArrowTower");
        specialtyModel = ScriptableManager.Instance.AllSpecialtyModel;
        interval = specialtyModel.BuildingArcherTowerIntervalTime;
        CheckTowerBuildingPlace();
    }

    //fix領土變更時做此搜尋
    public void CheckTowerBuildingPlace()
    {
        var places = GameObject.FindGameObjectsWithTag("ArcherTowerPlace");
        //fix最大數量要在遊戲中改變
        foreach (var place in places)
        {
            PositionStatus pos = new PositionStatus(new Vector3(place.transform.position.x, place.transform.position.y , transform.position.z), true);
            buildingPlaceLsit.Add(pos);
        }
    }

    //由單位多次觸發,注意效能
    public IEnumerator DoSpecialize(Vector3 startPosition, GameObject gameObject, UnitView unitView, UnitStats unitStats)
    {
        //1.確定有空位及準備人物動畫
        var placeNumber = CheckBuildingPlace();
        if (placeNumber > buildingPlaceLsit.Count)
        {
            unitStats.SetUnitState(UnitState.Idle);
            yield break;
        }
        var place = buildingPlaceLsit[placeNumber].position;
        buildingPlaceLsit[placeNumber].hasSpace = false;
        float elapsed = 0f;
        unitStats.SetUnitState(UnitState.DoSpecialty);
        unitView.StartWalkAnimation(true);
        //設立targetPosition避免面向錯誤
        var unit = gameObject.GetComponent<UnitController>();
        unit.targetPosition = place;
        //2.行走至建築目標位置
        while (unitStats.CurrentState == UnitState.DoSpecialty)
        {
            yield return null;
            MoveTowardsTarget(gameObject, unitStats, place, unitView);
        }

        //3.生成建築物，只生成一次
        if (unitStats.CurrentState == UnitState.Building)
        {
            GameObject smallArrowTower = BlueArrowTower;
            smallArrowTower =  GameManager.Instance.scriptableManager.
                FindGameObject(unitStats.TeamColor.ToString() ,"ArcherTower" );

            var buildingController = Instantiate(smallArrowTower, place + new Vector2(0, -0.4f), Quaternion.identity).GetComponent<BuildingController>();
            buildingPlaceLsit[placeNumber].builldController = buildingController;
        }
        else
        {
            buildingPlaceLsit[placeNumber].hasSpace = true;
            EndSpecialityAction(unitView, unitStats);
            yield break;
        }

        // 4.執行專長動作，並在時間結束後停止
        while (elapsed <= specializeTime)
        {
            OnSpecializeAction(unitView);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        EndSpecialityAction(unitView, unitStats);
    }

    public void ExecuteAction(Action action)
    {

    }

    public void StartSpecialityAction(UnitView unitView, UnitStats unitStats, Vector2 place)
    {

    }

    public void OnSpecializeAction(UnitView unitView)
    {
        unitView.BuildingSpecialtyAction();
        //雙手替換武器為槌子
    }
    public void EndSpecialityAction(UnitView unitView, UnitStats unitStats)
    {
        unitView.BuildingSpecialtyFinishAction();
        unitStats.SetUnitState(UnitState.Idle);
    }


    int CheckBuildingPlace()
    {
        int current = 0;
        foreach (var place in buildingPlaceLsit)
        {
            if (place.hasSpace == true)
            {
                return current;
            }
            current++;
        }
        //回傳極大數字,代表沒有目標
        return 1000;
    }


    public void CheckAllBuildingPlace()
    {
        for (int i = 0; i < buildingPlaceLsit.Count; i++)
        {
            if (buildingPlaceLsit[i].hasSpace == false && buildingPlaceLsit[i].builldController == null)
            {
                buildingPlaceLsit[i].hasSpace = true;
            }
        }
    }

    private void MoveTowardsTarget(GameObject gameObject, UnitStats unitStats, Vector2 targetPosition, UnitView unitView)
    {
        //CheckDirection(targetPosition, unitView);
        Vector2 currentPosition = gameObject.transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        //unitView.ChangeToward(direction.magnitude);
        Vector2 newPosition = currentPosition + direction * unitStats.unitModel.moveSpeed * Time.deltaTime;
        gameObject.transform.position = newPosition;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            unitView.StartWalkAnimation(false);
            gameObject.transform.position = targetPosition;
            unitStats.CurrentState = UnitState.Building;
        }
    }

}
