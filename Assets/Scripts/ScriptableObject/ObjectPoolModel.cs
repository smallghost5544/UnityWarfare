using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolData", menuName = "ScriptableObjects/ObjectPoolData", order = 1)]
public class ObjectPoolModel : ScriptableObject
{
    [Header("���@�w���ƶq")]
    public int UnitOnePoolCount;
    [Header("���@�r��")]
    public string UnitOnePoolString;
    [Header("���G�w���ƶq")]
    public int UnitTwoPoolCount;
    [Header("���G�r��")]
    public string UnitTwoPoolString;
    [Header("Unit�}�b�w���ƶq")]
    public int UnitArrowCount;
    [Header("Unit�}�b�r��")]
    public string UnitArrowPoolString;
    [Header("Tower�}�b�w���ƶq")]
    public int TowerArrowCount;
    [Header("Tower�}�b�w���r��")]
    public string TowerArrowPoolString;


}
