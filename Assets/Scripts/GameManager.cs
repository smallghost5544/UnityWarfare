
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform TeamOneRespawnPoint;
    public Transform TeamTwoRespawnPoint;
    public GameObject TeamOneUnit;
    public GameObject TeamTwoUnit;
    public int TeamOneCount = 10;
    public int TeamTwoCount = 10;
    public string TeamOneString = "Team1";
    public string TeamTwoString = "Team2";
    public TextMeshProUGUI onStageCountText;
    public List<UnitStats> unitLists = new List<UnitStats>();
    int enemyCount = 0;
    private static GameManager _instance;
    ObjectPool objectPool;
    public TestButtonFunctions testButtonFunctions;
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
        objectPool = GetComponent<ObjectPool>();
        testButtonFunctions = GetComponent<TestButtonFunctions>();
    }
    private void Start()
    {
        objectPool.Preload(TeamOneString, TeamOneUnit, TeamOneCount);
        objectPool.Preload(TeamTwoString, TeamTwoUnit, TeamTwoCount);
        //Bounds worldBounds = new Bounds(Vector3.zero, new Vector3(100, 100, 0)); // Example world bounds
        //quadtree = new Quadtree(worldBounds);
    }

    void Update()
    {
        onStageCountText.text = "Units on stage: " + enemyCount;
        TestFunctionButtons();
    }

    void SetGameSpeed(float newSpeed)
    {
        Time.timeScale += newSpeed;
    }
    void TestFunctionButtons()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //Instantiate(TeamOneUnit, TeamOneRespawnPoint);
            objectPool.Get(TeamOneString, TeamOneRespawnPoint.transform.position);
            //quadtree.Insert(obj);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            //Instantiate(TeamTwoUnit, TeamTwoRespawnPoint);
           objectPool.Get(TeamTwoString, TeamTwoRespawnPoint.transform.position);
            //quadtree.Insert(obj);
        }
        if (Input.GetKey(KeyCode.B))
        {
            //Instantiate(TeamOneUnit, TeamOneRespawnPoint);
           var obj =objectPool.Get(TeamOneString, TeamOneRespawnPoint.transform.position);
            testButtonFunctions.unitsOnStage.Add(obj);
            //quadtree.Insert(obj);
            enemyCount += 1;
        }
        if (Input.GetKey(KeyCode.N))
        {
            //Instantiate(TeamTwoUnit, TeamTwoRespawnPoint);
            var obj =objectPool.Get(TeamTwoString, TeamTwoRespawnPoint.transform.position);
            //quadtree.Insert(obj);
            enemyCount += 1;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetGameSpeed(-1.0f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SetGameSpeed(1.0f);
        }
    }
}
