using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolData", menuName = "ScriptableObjects/ObjectPoolData", order = 1)]
public class ObjectPoolModel : ScriptableObject
{
    [Header("單位一預熱數量")]
    public int UnitOnePoolCount;
    [Header("單位一字串")]
    public string UnitOnePoolString;
    [Header("單位二預熱數量")]
    public int UnitTwoPoolCount;
    [Header("單位二字串")]
    public string UnitTwoPoolString;
    [Header("Unit弓箭預熱數量")]
    public int UnitArrowCount;
    [Header("Unit弓箭字串")]
    public string UnitArrowPoolString;
    [Header("Tower弓箭預熱數量")]
    public int TowerArrowCount;
    [Header("Tower弓箭預熱字串")]
    public string TowerArrowPoolString;


}
