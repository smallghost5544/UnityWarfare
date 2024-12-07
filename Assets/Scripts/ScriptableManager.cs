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

    public ObjectPoolModel AllObjectPoolModel;
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

}
