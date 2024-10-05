
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialtyStaticData", menuName = "ScriptableObjects/SpecialtyStaticData", order = 2)]
public class SpecialtyModel : ScriptableObject
{
    [Header("蓋建塔時間")]
    public float BuildingArcherTowerTime = 5f;
    [Header("蓋建塔動作時間")]
    public float BuildingArcherTowerIntervalTime = 0.5f;
    [Header("開寶箱加成")]
    public float OpenChestMultiply = 1.5f;
    [Header("巡邏時間")]
    public float PatrolTime = 6f;
    [Header("採礦專長")]
    public bool EnableMine = true;
    [Header("採礦加成")]
    public float MineMultiply = 1.5f;
    [Header("最後一滴血")]
    public bool LastBloodStanding = true;
    [Header("強健體魄")]
    public float BetterStaminaMultiply = 1.2f;
}
