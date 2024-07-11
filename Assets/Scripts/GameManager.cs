
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
    public List<UnitAbility> unitLists = new List<UnitAbility>();
    int enemyCount = 0;
    private static GameManager _instance;
    ObjectPool objectPool;
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
    public Quadtree quadtree;
    //public List<GameObject> units = new List<GameObject>();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        objectPool = GetComponent<ObjectPool>();
        Bounds worldBounds = new Bounds(Vector3.zero, new Vector3(100, 100, 0));
        quadtree = new Quadtree(worldBounds);
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

    public GameObject SearchObjectsInArea(Vector2 center, float radius)
    {
        // 定義搜索範圍
        Bounds searchBounds = new Bounds(center, new Vector3(radius * 2, radius * 2, 1f));

        // 在Quadtree中搜索位於指定範圍內的遊戲對象
        List<GameObject> objectsInArea = quadtree.Search(searchBounds);
        print(objectsInArea.Count);
        var closest = Mathf.Infinity;
        GameObject answer = null;
        // 處理搜索結果
        foreach (GameObject obj in objectsInArea)
        {
            var distance = Vector3.Distance(center, obj.transform.position);
            if (distance < closest)
            {
                closest = distance;
                answer = obj;
            }
        }
        return answer;
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
            var obj = objectPool.Get(TeamOneString, TeamOneRespawnPoint.transform.position);
            //quadtree.Insert(obj);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            //Instantiate(TeamTwoUnit, TeamTwoRespawnPoint);
           var obj = objectPool.Get(TeamTwoString, TeamTwoRespawnPoint.transform.position);
            //quadtree.Insert(obj);
        }
        if (Input.GetKey(KeyCode.B))
        {
            //Instantiate(TeamOneUnit, TeamOneRespawnPoint);
           var obj = objectPool.Get(TeamOneString, TeamOneRespawnPoint.transform.position);
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
    public void SetUnitAbility()
    {

    }
}
