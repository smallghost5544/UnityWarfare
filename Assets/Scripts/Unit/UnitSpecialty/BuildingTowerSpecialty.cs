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
    //true�N����m
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

    //fix��g�ܧ�ɰ����j�M
    public void CheckTowerBuildingPlace()
    {
        var places = GameObject.FindGameObjectsWithTag("ArcherTowerPlace");
        //fix�̤j�ƶq�n�b�C��������
        foreach (var place in places)
        {
            PositionStatus pos = new PositionStatus(new Vector3(place.transform.position.x, place.transform.position.y , transform.position.z), true);
            buildingPlaceLsit.Add(pos);
        }
    }

    //�ѳ��h��Ĳ�o,�`�N�į�
    public IEnumerator DoSpecialize(Vector3 startPosition, GameObject gameObject, UnitView unitView, UnitStats unitStats)
    {
        //1.�T�w���Ŧ�ηǳƤH���ʵe
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
        //�]��targetPosition�קK���V���~
        var unit = gameObject.GetComponent<UnitController>();
        unit.targetPosition = place;
        //2.�樫�ܫؿv�ؼЦ�m
        while (unitStats.CurrentState == UnitState.DoSpecialty)
        {
            yield return null;
            MoveTowardsTarget(gameObject, unitStats, place, unitView);
        }

        //3.�ͦ��ؿv���A�u�ͦ��@��
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

        // 4.����M���ʧ@�A�æb�ɶ������ᰱ��
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
        //��������Z�����l�l
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
        //�^�Ƿ��j�Ʀr,�N��S���ؼ�
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
