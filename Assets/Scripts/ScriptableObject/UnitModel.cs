
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStaticData", menuName = "ScriptableObjects/UnitStaticData", order = 1)]
public class UnitModel : ScriptableObject
{
    [Header("角色陣營")]
    public int Team;
    [Header("角色最大血量")]
    public int MaxHP = 100;
    [Header("攻擊距離")]
    public float AttackRange = 1.5f;
    [Header("角色傷害")]
    public int AttackDamage = 10;
    [Header("搜索敵方範圍")]
    public float SearchRange = 10f; 
    [Header("搜索中立物件範圍")]
    public float SearchNetutralRange = 10f;
    [Header("角色移動速度")]
    public float moveSpeed = 1;
    [Header("角色最大發呆時間")]
    public float randomIdleTime = 5;
    [Header("移動距離檢測")]
    public float moveThreshold = 0.2f;
    [Header("下次動作時間")]
    public float moveTime = 1.5f;
    [Header("攻擊頻率")]
    public float attackCD = 0.9f;
    [Header("目前動作時間")]
    public float currentTime = 0;
    [Header("搜尋敵方頻率")]
    public float  searchTime = 0;
    [Header("攻擊動畫序號")]
    public int attackType = 0;
    [Header("攻擊動畫擊中時間")]
    public float attackAnimationHitTime = 0.3f;
    [Header("通常行為動作間隔時間")]
    public float usualActionIntervalTime = 0.3f;
}
