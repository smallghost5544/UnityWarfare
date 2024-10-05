using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestButtonFunctions : MonoBehaviour
{
    [Header("角色物件")]
    public GameObject LittleUnit;
    [Header("生產角色位置")]
    public Transform SpawnPoint;
    [Header("場上角色數量文字")]
    public TextMeshProUGUI onStageCountText;
    [Header("新增角色按鍵")]
    public KeyCode AddButton;
    [Header("刪除角色按鍵")]
    public KeyCode DeleteButton;
    [Header("角色排隊")]
    public KeyCode Line_upButton;
    [Header("排隊行系數")]
    public float RowValue;
    [Header("排隊列系數")]
    public float ColumnValue;
    [Header("排隊最大行數")]
    public int rowMaxCount;
    public Transform StartPoint;
    public Slider rowValueSlider;
    public Slider columnValueSlider;
    public TextMeshProUGUI rowCount;
    public List<GameObject> unitsOnStage = new List<GameObject>();
    public List<UnitController> unitActions = new List<UnitController>();
    public bool activeButtonOne = false;
    public bool activeButtonTwo = false;
    public Camera mainCamera;
    public GameObject unitOne;
    public GameObject unitTwo;
    Vector3 worldPosition;

    public float triggerRate = 10f; // 每秒最多觸發的次數
    private float nextTriggerTime = 0f;
    private void Update()
    {
        RowValue = rowValueSlider.value;
        ColumnValue = columnValueSlider.value;
        if (Input.GetKey(AddButton))
            AddUnit();
        if (Input.GetKey(DeleteButton))
            DeleteUnit();
        ChangeStartPoint();
        if (activeButtonOne && (Input.GetMouseButton(0)))
        {
            if (Time.time >= nextTriggerTime)
            {
                Vector3 mousePosition = Input.mousePosition; // 獲取滑鼠在螢幕上的座標
                worldPosition = mainCamera.ScreenToWorldPoint(mousePosition); // 轉換為世界座標
                worldPosition.z = 0; // 確保生成位置在2D平面上
                CreateUnitOnScreen(worldPosition, unitOne);
                nextTriggerTime = Time.time + 1f / triggerRate;
            }
        }
        if (activeButtonTwo && (Input.GetMouseButton(0)))
        {
            if (Time.time >= nextTriggerTime)
            {
                Vector3 mousePosition = Input.mousePosition; // 獲取滑鼠在螢幕上的座標
                worldPosition = mainCamera.ScreenToWorldPoint(mousePosition); // 轉換為世界座標
                worldPosition.z = 0; // 確保生成位置在2D平面上
                CreateUnitOnScreen(worldPosition, unitTwo);
                nextTriggerTime = Time.time + 1f / triggerRate;
            }
        }
    }
    public void ActiveButton(int buttonNumber)
    {
        if (buttonNumber == 1)
        {
            if (activeButtonOne == true)
                activeButtonOne = false;
            else
            {
                activeButtonOne = true;
                activeButtonTwo = false;
            }
        }
        if (buttonNumber == 2)
        {
            if (activeButtonTwo == true)
                activeButtonTwo = false;
            else
            {
                activeButtonOne = false;
                activeButtonTwo = true;
            }
        }
    }

    /// <summary>
    /// 新增單位
    /// </summary>
    public void AddUnit()
    {
        var obj = Instantiate(LittleUnit, SpawnPoint);
        unitsOnStage.Add(obj);
        unitActions.Add(obj.GetComponent<UnitController>());
        onStageCountText.text = "Units on stage: " + unitsOnStage.Count;
    }
    /// <summary>
    /// 按下該單位按鈕後,於螢幕上點擊位置可生成單位
    /// </summary>
    public void CreateUnitOnScreen(Vector2 initPlace, GameObject unit)
    {
        var obj = Instantiate(unit, initPlace, Quaternion.identity);
        unitsOnStage.Add(obj);
        unitActions.Add(obj.GetComponent<UnitController>());
        onStageCountText.text = "Units on stage: " + unitsOnStage.Count;
    }

    /// <summary>
    /// 刪去最後一個創建的單位
    /// </summary>
    public void DeleteUnit()
    {
        if (unitsOnStage.Count > 0)
        {
            GameObject deleteUnit = unitsOnStage[unitsOnStage.Count - 1];
            Destroy(deleteUnit);
            unitsOnStage.RemoveAt(unitsOnStage.Count - 1);
            unitActions.Remove(deleteUnit.GetComponent<UnitController>());
            onStageCountText.text = "Units on stage: " + unitsOnStage.Count;
        }
    }

    /// <summary>
    /// 針對每一個在list中的gameobject 分別指派位置並讓他們行動,並控制他們的面向統一為準
    /// </summary>
    public void LineUpUnits(int number)
    {
        activeButtonOne = false;
        activeButtonTwo = false;
        switch (number)
        {
            case 1:
                Square();
                break;
            case 2:
                Triangle();
                break;
            case 3:
                Rectangle();
                break;
            case 4:
                Circle();
                break;
        }
        //若正在建築的單位 手中武器恢復原本武器
        //fix 注意效能
        foreach (var obj in unitsOnStage)
        {
            
            var view = obj.GetComponent<UnitView>();
            view.BuildingSpecialtyFinishAction(needTrigger: true);
        }
    }

    public void TakeABreak()
    {
        var current = 0;
        while (current < unitActions.Count)
        {
            StartCoroutine(unitActions[current].Command("TakeABreak"));
            current++;
        }
        //檢查建築物
        var ChekBuilding = Resources.Load<GameObject>("LoadSpecialtyPrefab/BuildTower").GetComponent<BuildingTowerSpecialty>();
        ChekBuilding.CheckAllBuildingPlace();
    }

    public void AddRowMax()
    {
        rowMaxCount++;
        rowCount.text = rowMaxCount.ToString();
    }
    public void MinusRowMax()
    {
        rowMaxCount--;
        rowCount.text = rowMaxCount.ToString();
    }
    void Square()
    {
        int currentUnit = 0;
        int maxColumn = Mathf.RoundToInt(Mathf.Sqrt(unitActions.Count));
        float curRow = Mathf.Round(StartPoint.transform.position.x * 100f) / 100f;
        float curColumn = Mathf.Round(StartPoint.transform.position.y * 100f) / 100f;
        float initialColumn = curColumn;
        while (currentUnit < unitActions.Count)
        {
            Vector2 targetVector = new Vector2(curRow * RowValue, (curColumn + maxColumn / 2) * ColumnValue);
            StartCoroutine(unitActions[currentUnit].Command("LineUp", targetVector, 1));
            curColumn--;
            if ((currentUnit + 1) % maxColumn == 0)
            {
                curRow--;
                curColumn = initialColumn;
            }
            currentUnit++;
        }
    }
    //fix 沒依照正中心開始排
    //void Square()
    //{
    //    //以起始點為中點開始排序
    //    var currentUnit = 0;
    //    //因為為方陣,所以取總數的開根號為一列有幾人
    //    int maxColumn = (int)Mathf.Round(Mathf.Sqrt(unitsOnStage.Count));
    //    float curRow = Mathf.Round(StartPoint.transform.position.x * 100f) / 100f;
    //    float curColumn = Mathf.Round(StartPoint.transform.position.y * 100f) / 100f;
    //    float saveFirstPlace = curColumn;
    //    float columnCount = 0;
    //    //雙數時中心點在兩排中間,單數時中心點位於最中間單數排
    //    var columnCenter = unitActions.Count / maxColumn;
    //    while (currentUnit < unitActions.Count)
    //    {
    //        //從中心點往上排一半,往下排一半
    //        var targetVector = new Vector2(curRow * RowValue, (curColumn + columnCenter / 2) * ColumnValue);
    //        StartCoroutine(unitActions[currentUnit].Command("LineUp", targetVector, 1));
    //        curColumn--;
    //        columnCount++;
    //        //超出限定最大行數則換排
    //        if (columnCount >= maxColumn)
    //        {
    //            //由右往左,所以--
    //            curRow--;
    //            //回歸第一個位置開始排
    //            curColumn = saveFirstPlace;
    //            columnCount = 0;
    //        }
    //        currentUnit++;
    //    }
    //}
    /// <summary>
    /// 三角陣型 
    /// </summary>
    void Triangle()
    {
        var currentUnit = 0;
        var maxColumn = 1;
        float curRow = Mathf.Round(StartPoint.transform.position.x * 10f) / 10f;
        float curColumn = Mathf.Round(StartPoint.transform.position.y * 10f) / 10f;
        float firstColumnPlace = curColumn;
        float columnCount = 0;
        while (currentUnit < unitActions.Count)
        {
            //從中心點往上排一半,往下排一半
            var targetVector = new Vector2(curRow * RowValue, (curColumn + maxColumn / 2) * ColumnValue);
            StartCoroutine(unitActions[currentUnit].Command("LineUp", targetVector, 1));
            curColumn--;
            columnCount++;
            //超出限定最大行數則換排
            if (columnCount >= maxColumn)
            {
                //由右往左,所以--
                curRow--;
                //回歸第一個位置開始排
                curColumn = firstColumnPlace;
                columnCount = 0;
                maxColumn += 2;
            }
            currentUnit++;
        }
    }

    public void Rectangle()
    {
        //自訂最大行數的版本,目前想用於長方形陣列
        var current = 0;
        var rowCount = 0;
        //事先取得總列數,讓隊伍中心以中心點排隊
        var maxColumn = unitActions.Count / rowMaxCount;
        float curRow = Mathf.Round(StartPoint.transform.position.x * 100f) / 100f;
        float curColumn = Mathf.Round(StartPoint.transform.position.y * 100f) / 100f;
        while (current < unitActions.Count)
        {
            var targetVector = new Vector3(curRow * RowValue, (curColumn + maxColumn / 2) * ColumnValue);
            StartCoroutine(unitActions[current].Command("LineUp", targetVector, 1));
            curRow--;
            rowCount++;
            if (rowCount >= rowMaxCount)
            {
                curColumn--;
                rowCount = 0;
                curRow = StartPoint.transform.position.x;
            }
            current++;
        }
    }

    public void Circle()
    {
        //圓半徑
        float radius = 1;
        //目前第幾個單位
        var current = 0;
        //目前圓周第幾個單位
        var radiusCount = 0;
        //圓心
        var center = StartPoint.transform.position;
        //單位圓角度
        var unitAngle = (int)(360 / Mathf.Floor(2 * radius * Mathf.PI));
        while (current < unitActions.Count)
        {
            //超過360度則排去下一圈
            if (unitAngle * radiusCount >= 360)
            {
                radius += 1;
                radiusCount = 0;
                unitAngle = (int)(360 / Mathf.Floor(2 * radius * Mathf.PI));
            }
            float angleInRadians = unitAngle * radiusCount * Mathf.Deg2Rad;
            var x = radius * (Mathf.Cos(angleInRadians)) + center.x;
            var y = radius * (Mathf.Sin(angleInRadians)) + center.y;
            var targetVector = new Vector2(x, y);
            StartCoroutine(unitActions[current].Command("LineUp", targetVector, 1));
            current++;
            radiusCount++;
        }
    }
    void ChangeStartPoint()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (IsPointerOverUIElement())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(StartPoint.transform.position).z; // 確保我們的點擊點有深度值
            // 將螢幕座標轉換為世界座標
            StartPoint.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            StartPoint.transform.position = new Vector3(StartPoint.transform.position.x, StartPoint.transform.position.y, 0);
            return;
        }
    }
    private bool IsPointerOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    public void Reset()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // 使用場景名稱重新加載場景
        SceneManager.LoadScene(currentScene.name);
    }
}

