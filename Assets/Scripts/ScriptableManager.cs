using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ScriptableManager : MonoBehaviour
{
    private static ScriptableManager _instance;
    public static ScriptableManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScriptableManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<ScriptableManager>();
                    singletonObject.name = typeof(ScriptableManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    public AllGameObejctModel AllGameObjectModels;
    public ObjectPoolModel PreloadObjectPoolModels;
    public SpecialtyModel AllSpecialtyModel;
    public UnitModel ArcherTowerModel;
    public UnitModel BasicMeleeModel;
    public UnitModel BasicRangeModel;
    public UnitModel CastleModel;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }


    public GameObject FindGameObject(string mainKey, string subKey)
    {
        // �M���̥~�h���
        foreach (var nestedList in AllGameObjectModels.TeamUnitData)
        {
            if (nestedList.key == mainKey) // �ǰt�D���� key
            {
                // �M�����h���
                foreach (var pair in nestedList.subPairs)
                {
                    if (pair.key == subKey) // �ǰt�l���� key
                    {
                        return pair.value; // ��^��쪺 GameObject
                    }
                }
            }
        }
        // �����h��^ null
        return null;
    }
}
