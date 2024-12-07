
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

    //fix �n�H�s�y������ƶq�ӧP�wtrue false
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
    /// ���a����C���,���t��L�q�����C��
    /// </summary>
    void DistrubuteColor()
    {
        if (StartSceneController.Instance == null)
            return;
        //Ū�����a������C��
        PlayerChooseColor = StartSceneController.Instance.color;
        List<int> colors = new List<int>() { (int)UnitColor.Red, (int)UnitColor.Blue, (int)UnitColor.Green };
        //�̧ǥͦ��T���C��
        var mainPlayerData = new PlayerGameData();
        mainPlayerData.playerColor = PlayerChooseColor;
        //��X�Q�������s���Ǹ�
        var removeColorInt = colors.IndexOf((int)PlayerChooseColor);
        print($"Player�C��: {(UnitColor)colors[removeColorInt]}");
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
            print($"�s�W CPU �C��: {(UnitColor)colors[i]}");
        }
    }
    /// <summary>
    /// ���t�U�դO�_�l��m
    /// </summary>
    void DistrubuteNationPlace()
    {
        foreach (var data in AllPlayerData)
        {
            var num = GetRandomNation();
            data.NationPlace = NationStartPlace[num];
            occupyPlace.Add(num);
            print($"���t�ϰ�: {num} ");
        }
    }
    List<int> occupyPlace = new List<int>();
    int GetRandomNation()
    {
        int num;

        // ����ͦ��H���ơA�����쥼�Q���Ϊ���m
        do
        {
            num = Random.Range(0, NationStartPlace.Count);
        } while (occupyPlace.Contains(num));

        return num;
    }

    /// <summary>
    /// ���t���a���
    /// </summary>
    void PlayerDataInitialization()
    {
        //�U��data�M��movementArea
        foreach (var data in AllPlayerData)
        {
            data.UnitMovementArea = data.NationPlace.transform.Find("UnitMoveSpace").GetComponent<Collider2D>();
            if (data.UnitMovementArea == null)
                print($"{data.playerColor}�S���");
        }
        //�U��data�M��castleplace
        foreach (var data in AllPlayerData)
        {
            foreach (Transform child in data.NationPlace.transform)
            {
                if (child.name == "CastlePlace") // �ˬd�W��
                {
                    data.CastlePlaces.Add(child.gameObject); // �[�J��C��
                }
            }
            print($"{data.playerColor}��{data.CastlePlaces.Count}�y����");

            if (data.CastlePlaces.Count == 0)
                print($"{data.playerColor}�S��쫰��");
        }
        //���t�ؿv��m
        foreach (var data in AllPlayerData)
        {

            var TowerGroup = data.NationPlace.transform.Find("BuildingTowerPlace");
            foreach (Transform tower in TowerGroup.transform)
            {
                if (tower.name == "buildingLocation") // �ˬd�W��
                {
                    PositionStatus pos = new PositionStatus(new Vector3(tower.transform.position.x, tower.transform.position.y, transform.position.z), true);
                    data.BuildingPlaceLsit.Add(pos);
                }
            }

            print($"{data.playerColor}��{data.BuildingPlaceLsit.Count}�y�ؿv�a");
            if (data.BuildingPlaceLsit.Count == 0)
                print($"{data.playerColor}�S���ؿv�a");
        }
    }

    /// <summary>
    /// Ū���U���C�⪺�ؿv�P���
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
