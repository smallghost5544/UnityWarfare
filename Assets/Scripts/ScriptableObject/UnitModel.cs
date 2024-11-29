
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStaticData", menuName = "ScriptableObjects/UnitStaticData", order = 1)]
public class UnitModel : ScriptableObject
{
    [Header("����}��")]
    public int Team;
    [Header("����̤j��q")]
    public int MaxHP = 100;
    [Header("�����Z��")]
    public float AttackRange = 1.5f;
    [Header("����ˮ`")]
    public int AttackDamage = 10;
    [Header("�j���Ĥ�d��")]
    public float SearchRange = 10f; 
    [Header("�j�����ߪ���d��")]
    public float SearchNetutralRange = 10f;
    [Header("���Ⲿ�ʳt��")]
    public float moveSpeed = 1;
    [Header("����̤j�o�b�ɶ�")]
    public float randomIdleTime = 5;
    [Header("���ʶZ���˴�")]
    public float moveThreshold = 0.2f;
    [Header("�U���ʧ@�ɶ�")]
    public float moveTime = 1.5f;
    [Header("�����W�v")]
    public float attackCD = 0.9f;
    [Header("�ثe�ʧ@�ɶ�")]
    public float currentTime = 0;
    [Header("�j�M�Ĥ��W�v")]
    public float  searchTime = 0;
    [Header("�����ʵe�Ǹ�")]
    public int attackType = 0;
    [Header("�����ʵe�����ɶ�")]
    public float attackAnimationHitTime = 0.3f;
    [Header("�q�`�欰�ʧ@���j�ɶ�")]
    public float usualActionIntervalTime = 0.3f;
}
