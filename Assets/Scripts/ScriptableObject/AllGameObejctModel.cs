using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllGameObejctModel", menuName = "ScriptableObjects/AllGameObejctModel", order = 1)]
public class AllGameObejctModel : ScriptableObject
{
    public List<NestedList> TeamUnitData = new List<NestedList>(); // �̥~�h List
}


[System.Serializable]
public class StringListPair
{
    public string key;                      // ���� Dictionary �� Key
    public GameObject value;
}

[System.Serializable]
public class NestedList
{
    public string key;                                     // �����~�h Key
    public List<StringListPair> subPairs = new List<StringListPair>(); // �������h Dictionary
}
