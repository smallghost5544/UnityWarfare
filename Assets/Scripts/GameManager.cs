
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform TeamOneRespawnPoint;
    public Transform TeamTwoRespawnPoint;
    public GameObject TeamOneUnit;
    public GameObject TeamOneArcher;
    public GameObject TeamTwoUnit;
    public GameObject unitArrow;
    public GameObject towerArrow;
    public int TeamOneCount = 10;
    public int TeamTwoCount = 10;
    public string TeamOneString = "Team1";
    public string TeamTwoString = "Team2";
    public List<UnitStats> unitLists = new List<UnitStats>();
    private static GameManager _instance;
    ObjectPool objectPool;
    public TestButtonFunctions testButtonFunctions;
    List<ISpecialty> specialties = new List<ISpecialty>();
    public ObjectPoolModel objectPoolData => scriptableManager.AllObjectPoolModel;
    ScriptableManager scriptableManager;
    public int unitNumber = 0;
    public bool IsPressingCreateUnitButton
    {
        get { return isUseCreateButtons(); }
    }
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<GameManager>();
                    singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        scriptableManager = ScriptableManager.Instance;
        objectPool = GetComponent<ObjectPool>();
        testButtonFunctions = GetComponent<TestButtonFunctions>();
        //if(objectPoolData == null)
        //{
        //    objectPoolData = Resources.Load<ObjectPoolModel>("ScriptableObjectData/ObjectPoolData") ;
        //}
    }
    private void Start()
    {
        PreloadObjects();
        ResetSpeciality();
        //Bounds worldBounds = new Bounds(Vector3.zero, new Vector3(100, 100, 0)); // Example world bounds
        //quadtree = new Quadtree(worldBounds);
    }

    void PreloadObjects()
    {
        for (int i = 0; i < objectPoolData.PreloadGameObjects.Count; i++)
        {
            objectPool.Preload(objectPoolData.PreloadGameObjects[i].gameObject.name,
                                                objectPoolData.PreloadGameObjects[i],
                                                objectPoolData.PreloadCount);
        }
        //objectPool.Preload(objectPoolData.UnitOnePoolString, TeamOneUnit, objectPoolData.UnitOnePoolCount);
        //objectPool.Preload(objectPoolData.UnitTwoPoolString, TeamTwoUnit, objectPoolData.UnitTwoPoolCount);
        //objectPool.Preload(objectPoolData.UnitArrowPoolString, unitArrow, objectPoolData.UnitArrowCount);
        //objectPool.Preload(objectPoolData.TowerArrowPoolString, towerArrow, objectPoolData.TowerArrowCount);
        //objectPool.Preload(objectPoolData.TeamOneArcherString, TeamOneArcher, objectPoolData.TeamOneArcherCount);
    }

    void Update()
    {
        TestFunctionButtons();
    }

    void SetGameSpeed(float newSpeed)
    {
        Time.timeScale += newSpeed;
    }
    void TestFunctionButtons()
    {
        if (Input.GetKey(KeyCode.B))
        {
            //Instantiate(TeamOneUnit, TeamOneRespawnPoint);
            var obj = objectPool.Get(objectPoolData.PreloadGameObjects[0].gameObject.name, TeamOneRespawnPoint.transform.position);
            testButtonFunctions.unitsOnStage.Add(obj);
            //quadtree.Insert(obj);
        }
        if (Input.GetKey(KeyCode.N))
        {
            //Instantiate(TeamTwoUnit, TeamTwoRespawnPoint);
            var obj = objectPool.Get(objectPoolData.PreloadGameObjects[1].gameObject.name, TeamTwoRespawnPoint.transform.position);
            //quadtree.Insert(obj);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetGameSpeed(-1.0f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SetGameSpeed(1.0f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetGameSpeed(-0.1f);
            print("currentGameSpeed: " + Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetGameSpeed(0.1f);
            print("currentGameSpeed: " + Time.timeScale);
        }
    }

    //fix 要隨製造單位按鍵數量來判定true false
    bool isUseCreateButtons()
    {
        if (testButtonFunctions.activeButtonOne == false && testButtonFunctions.activeButtonTwo == false)
            return false;
        return true;
    }

    void ResetSpeciality()
    {
        var buildingSpeciality = Resources.Load<GameObject>("LoadSpecialtyPrefab/BuildTower").GetComponent<ISpecialty>();
        specialties.Add(buildingSpeciality);
        foreach (var specialty in specialties)
        {
            specialty.SetResource();
        }
    }
}
