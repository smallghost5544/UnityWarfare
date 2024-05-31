using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    List<GameObject> unitsOnStage = new List<GameObject>();
    List<UnitAction> unitActions = new List<UnitAction>();
    private void Update()
    {
        RowValue = rowValueSlider.value;
        ColumnValue = columnValueSlider.value;
        if (Input.GetKey(AddButton))
            AddUnit();
        if (Input.GetKey(DeleteButton))
            DeleteUnit();
    }
    /// <summary>
    /// 新增單位
    /// </summary>
    public void AddUnit()
    {
        var obj = Instantiate(LittleUnit, SpawnPoint);
        unitsOnStage.Add(obj);
        unitActions.Add(obj.GetComponent<UnitAction>());
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
            unitActions.Remove(deleteUnit.GetComponent<UnitAction>());
            onStageCountText.text = "Units on stage: " + unitsOnStage.Count;
        }
    }

    /// <summary>
    /// 針對每一個在list中的gameobject 分別指派位置並讓他們行動,並控制他們的面向統一為準
    /// </summary>
    public void LineUpUnits()
    {
        Square();
        //var current = 0;
        //float curRow = StartPoint.transform.position.x;
        //float curColummn = StartPoint.transform.position.y;
        //var maxRow = StartPoint.transform.position.x + rowMaxCount;
        //while (current < unitActions.Count)
        //{
        //    var targetVector = new Vector2(curRow * RowValue, curColummn * ColumnValue);
        //    StartCoroutine(unitActions[current].Command("LineUp", targetVector, 1));
        //    curRow++;
        //    //超出限定最大行數則換排
        //    if (curRow >= maxRow)
        //    {
        //        curColummn++;
        //        curRow = StartPoint.transform.position.x;
        //    }
        //    current++;
        //}
    }

    public void TakeABreak()
    {
        var current = 0;
        while (current < unitActions.Count)
        {
            StartCoroutine(unitActions[current].Command("TakeABreak"));
            current++;
        }
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

    /// <summary>
    /// 目前限定 由右排至左 由上至下  未來或許可改成直列數量大於橫排
    /// </summary>
    void Square()
    {
        //想以起始點為中點開始排序
        var currentUnit = 0;
        //因為為方陣,所以取總數的開根號為一行有幾人
        var maxColumn = Mathf.Round(Mathf.Sqrt(unitsOnStage.Count));
        float curRow = Mathf.Round(StartPoint.transform.position.x * 10f) / 10f;
        float curColumnPlace = Mathf.Round(StartPoint.transform.position.y * 10f) / 10f;
        float saveFirstColumn = curColumnPlace;
        float columnCount = 0;
        while (currentUnit < unitActions.Count)
        {
            //從中心點往上排一半,往下排一半
            var targetVector = new Vector2(curRow * RowValue, (curColumnPlace + maxColumn / 2) * ColumnValue);
            StartCoroutine(unitActions[currentUnit].Command("LineUp", targetVector, 1));
            curColumnPlace--;
            columnCount++;
            //超出限定最大行數則換排
            if (columnCount >= maxColumn )
            {
                //由右往左,所以--
                curRow--;
                //回歸第一個位置開始排
                curColumnPlace = saveFirstColumn;
                columnCount =0;
            }
            currentUnit++;
        }
    }

}

