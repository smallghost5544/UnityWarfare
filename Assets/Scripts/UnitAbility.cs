using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitAbility : MonoBehaviour
{
    [Header("角色目前血量")]
    public int HP = 100;
    [Header("角色最大血量")]
    public int MaxHp = 100;
    [Header("攻擊距離")]
    public float AttackRange = 1.5f;
    [Header("角色傷害")]
    public int AttackDamage = 10;
    [Header("搜索範圍")]
    public float SearchRange = 10f;
    [Header("角色移動速度")]
    public float moveSpeed = 1;
    [Header("角色最大發呆時間")]
    public float randomIdleTime = 5;
    public float moveThreshold = 0.2f;
    public float moveTime = 1.5f;
    public float currentTime = 0;
    public int TeamNumer = 0;
    public int attackType = 0;

}
