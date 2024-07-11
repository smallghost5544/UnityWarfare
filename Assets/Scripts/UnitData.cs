
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStaticData", menuName = "ScriptableObjects/UnitStaticData", order = 1)]
public class UnitData : ScriptableObject
{
    [Header("����}��")]
    public int Team;
    [Header("�����q")]
    public int HP = 100;
    [Header("�����Z��")]
    public float AttackRange = 1.5f;
    [Header("����ˮ`")]
    public int AttackDamage = 10;
    [Header("�j���d��")]
    public float SearchRange = 10f;
    [Header("���Ⲿ�ʳt��")]
    public float moveSpeed = 1;
    [Header("����̤j�o�b�ɶ�")]
    public float randomIdleTime = 5;
    [Header("���ʶZ���˴�")]
    public float moveThreshold = 0.2f;
    [Header("�U���ʧ@�ɶ�")]
    public float moveTime = 1.5f;
    [Header("�ثe�ʧ@�ɶ�")]
    public float currentTime = 0;
}
