using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllGameObejctModel", menuName = "ScriptableObjects/AllGameObejctModel", order = 1)]
public class AllGameObejctModel : ScriptableObject
{
    public List<NestedList> TeamUnitData = new List<NestedList>(); // 程糷 List
}


[System.Serializable]
public class StringListPair
{
    public string key;                      // 家览 Dictionary  Key
    public GameObject value;
}

[System.Serializable]
public class NestedList
{
    public string key;                                     // 家览糷 Key
    public List<StringListPair> subPairs = new List<StringListPair>(); // 家览ず糷 Dictionary
}
