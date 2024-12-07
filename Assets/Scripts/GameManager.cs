
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static BuildingTowerSpecialty;

public class GameManager : MonoBehaviour
{
    public Transform TeamOneRespawnPoint;
    public Transform TeamTwoRespawnPoint;
    public List<UnitStats> unitLists = new List<UnitStats>();
    private static GameManager _instance;
    ObjectPool objectPool;
    public TestButtonFunctions testButtonFunctions;
    List<ISpecialty> specialties = new List<ISpecialty>();
    public ObjectPoolModel objectPoolData => scriptableManager.AllObjectPoolModel;
    ScriptableManager scriptableManager;
    public int unitNumber = 0;
    public UnitColor PlayerChooseColor = UnitColor.Blue;
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
    public List<PlayerGameData> AllPlayerData = new List<PlayerGameData>();
    public PlayerGameData MainPlayerData;
    public List<GameObject> NationStartPlace = new List<GameObject>();

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
        GameDataSetting();
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

    void GameDataSetting()
    {
        DistrubuteColor();
        DistrubuteNationPlace();
        PlayerDataInitialization();
        PlayerLoadUnit();
        PlayerInitObject();
        InvokeRepeating(nameof(PlayerAutoInitUnit), 0f , 2f);
        Movecamera();
    }

    /// <summary>
    /// 玩家選取顏色後,分配其他電腦的顏色
    /// </summary>
    void DistrubuteColor()
    {
        if (StartSceneController.Instance == null)
            return;
        //讀取玩家選取的顏色
        PlayerChooseColor = StartSceneController.Instance.color;
        List<int> colors = new List<int>() { (int)UnitColor.Red, (int)UnitColor.Blue, (int)UnitColor.Green };
        //依序生成三個顏色
        var mainPlayerData = new PlayerGameData();
        mainPlayerData.playerColor = PlayerChooseColor;
        //找出被移除的編號序號
        var removeColorInt = colors.IndexOf((int)PlayerChooseColor);
        print($"Player顏色: {(UnitColor)colors[removeColorInt]}");
        AllPlayerData.Add(mainPlayerData);
        mainPlayerData.isPlayer = true;
        MainPlayerData = mainPlayerData;
        for (int i = 0; i < colors.Count; i++)
        {
            if (i == removeColorInt)
                continue;
            var CPUData = new PlayerGameData();
            CPUData.playerColor = (UnitColor)colors[i];
            AllPlayerData.Add(CPUData);
            print($"新增 CPU 顏色: {(UnitColor)colors[i]}");
        }
    }
    /// <summary>
    /// 分配各勢力起始位置
    /// </summary>
    void DistrubuteNationPlace()
    {
        foreach (var data in AllPlayerData)
        {
            var num = GetRandomNation();
            data.NationPlace = NationStartPlace[num];
            occupyPlace.Add(num);
            print($"分配區域: {num} ");
        }
    }
    List<int> occupyPlace = new List<int>();
    int GetRandomNation()
    {
        int num;

        // 持續生成隨機數，直到找到未被佔用的位置
        do
        {
            num = Random.Range(0, NationStartPlace.Count);
        } while (occupyPlace.Contains(num));

        return num;
    }

    /// <summary>
    /// 分配玩家資料
    /// </summary>
    void PlayerDataInitialization()
    {
        //各自data尋找movementArea
        foreach (var data in AllPlayerData)
        {
            data.UnitMovementArea = data.NationPlace.transform.Find("UnitMoveSpace").GetComponent<Collider2D>();
            if (data.UnitMovementArea == null)
                print($"{data.playerColor}沒找到");
        }
        //各自data尋找castleplace
        foreach (var data in AllPlayerData)
        {
            foreach (Transform child in data.NationPlace.transform)
            {
                if (child.name == "CastlePlace") // 檢查名稱
                {
                    data.CastlePlaces.Add(child.gameObject); // 加入到列表
                }
            }
            print($"{data.playerColor}有{data.CastlePlaces.Count}座城堡");

            if (data.CastlePlaces.Count == 0)
                print($"{data.playerColor}沒找到城堡");
        }
        //分配建築位置
        foreach (var data in AllPlayerData)
        {

            var TowerGroup = data.NationPlace.transform.Find("BuildingTowerPlace");
            foreach (Transform tower in TowerGroup.transform)
            {
                if (tower.name == "buildingLocation") // 檢查名稱
                {
                    PositionStatus pos = new PositionStatus(new Vector3(tower.transform.position.x, tower.transform.position.y, transform.position.z), true);
                    data.BuildingPlaceLsit.Add(pos);
                }
            }

            print($"{data.playerColor}有{data.BuildingPlaceLsit.Count}座建築地");
            if (data.BuildingPlaceLsit.Count == 0)
                print($"{data.playerColor}沒找到建築地");
        }
    }

    /// <summary>
    /// 讀取各自顏色的建築與單位
    /// </summary>
    void PlayerLoadUnit()
    {
        foreach (var data in AllPlayerData)
        {
            data.LoadAllUnit();
        }
    }

    void PlayerInitObject()
    {
        foreach (var data in AllPlayerData)
        {
            data.InitCastle();
        }
    }

    void PlayerAutoInitUnit()
    {
        foreach (var data in AllPlayerData)
        {
            data.InitUnit();
        }
        print("Invoke");
    }
    void Movecamera()
    {
        foreach (var data in AllPlayerData)
        {
            if (data.isPlayer)
            {
                Vector3 Pos = new Vector3(data.CastlePlaces[0].transform.position.x, data.CastlePlaces[0].transform.position.y , Camera.main.transform.position.z);
                Camera.main.transform.position = Pos;
            }
        }
    }
}
