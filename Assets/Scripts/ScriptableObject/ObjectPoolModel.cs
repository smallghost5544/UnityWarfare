using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolData", menuName = "ScriptableObjects/ObjectPoolData", order = 1)]
public class ObjectPoolModel : ScriptableObject
{
    public List<GameObject> PreloadGameObjects= new List<GameObject>(); 
    [Header("���w���ƶq")]
    public int PreloadCount;

}
