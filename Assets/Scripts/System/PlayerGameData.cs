using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildingTowerSpecialty;

public class PlayerGameData : MonoBehaviour
{
    public string identity;
    public int UnitCount;
    public int CastleCount;
    public float MoneyCount;
    //該勢力顏色
    public UnitColor playerColor;
    //是否為玩家操作
    public bool isPlayer = false;
    public GameObject NationPlace;
    public List<GameObject> CastlePlaces = new List<GameObject>();
    public Collider2D UnitMovementArea;
    public List<PositionStatus> BuildingPlaceLsit = new List<PositionStatus>();
    public GameObject BasicUnit;
    public GameObject CastleObject;
    public GameObject ArcherTowerObject;
    ScriptableManager scriptableManager => GameManager.Instance.scriptableManager;
    public void LoadAllUnit()
    {
        var loadString = GetDataByColor();
        var colorString = playerColor.ToString();
        string unitKey = "Melee";       // 子分類
        //BasicUnit = Resources.Load<GameObject>(loadString + colorString + "TeamMelee");
        //BasicUnit = FindGameObject(colorString, unitKey);
        BasicUnit = scriptableManager.FindGameObject(colorString, unitKey);
        if (BasicUnit != null)
            print(BasicUnit);

        unitKey = "Castle";
        //CastleObject = Resources.Load<GameObject>(loadString + colorString + "Castle");
        //CastleObject = FindGameObject(colorString, unitKey);
        CastleObject = scriptableManager.FindGameObject(colorString, unitKey);
        if (CastleObject != null)
            print(CastleObject);

        unitKey = "Tower";
        //ArcherTowerObject = Resources.Load<GameObject>(loadString + colorString + "Tower");
        //ArcherTowerObject = FindGameObject(colorString, unitKey);
        ArcherTowerObject = scriptableManager.FindGameObject(colorString, unitKey);
        if (ArcherTowerObject != null)
            print(ArcherTowerObject);

    }

    string GetDataByColor()
    {
        if (playerColor == UnitColor.Blue)
        {
            return "1.BlueTeam/";
        }
        if (playerColor == UnitColor.Red)
        {
            return "2.RedTeam/";
        }
        if (playerColor == UnitColor.Green)
        {
            return "3.GreenTeam/";
        }
        return null;
    }

    public void InitCastle()
    {
        foreach (var obj in CastlePlaces)
        {
            Instantiate(CastleObject, obj.transform);
        }

    }

    public void InitUnit()
    {
        if (UnitCount >= 50)
            return;
        var unit = Instantiate(BasicUnit, NationPlace.transform);
        var unitStat = unit.GetComponent<UnitStats>();
        unitStat.movementArea = UnitMovementArea;
        UnitCount++;
    }

}
