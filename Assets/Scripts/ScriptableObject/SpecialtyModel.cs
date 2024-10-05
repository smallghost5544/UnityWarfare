
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialtyStaticData", menuName = "ScriptableObjects/SpecialtyStaticData", order = 2)]
public class SpecialtyModel : ScriptableObject
{
    [Header("�\�ض�ɶ�")]
    public float BuildingArcherTowerTime = 5f;
    [Header("�\�ض�ʧ@�ɶ�")]
    public float BuildingArcherTowerIntervalTime = 0.5f;
    [Header("�}�_�c�[��")]
    public float OpenChestMultiply = 1.5f;
    [Header("���ޮɶ�")]
    public float PatrolTime = 6f;
    [Header("���q�M��")]
    public bool EnableMine = true;
    [Header("���q�[��")]
    public float MineMultiply = 1.5f;
    [Header("�̫�@�w��")]
    public bool LastBloodStanding = true;
    [Header("�j����z")]
    public float BetterStaminaMultiply = 1.2f;
}
