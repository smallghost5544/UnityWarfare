using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitAbility : MonoBehaviour
{
    [Header("����ثe��q")]
    public int HP = 100;
    [Header("����̤j��q")]
    public int MaxHp = 100;
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
    public float moveThreshold = 0.2f;
    public float moveTime = 1.5f;
    public float currentTime = 0;
    public int TeamNumer = 0;
    public int attackType = 0;

}
