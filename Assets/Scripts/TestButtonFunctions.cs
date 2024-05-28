using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonFunctions : MonoBehaviour
{
    [Header("���⪫��")]
    public GameObject LittleUnit;
    [Header("�Ͳ������m")]
    public Transform SpawnPoint;
    [Header("���W����ƶq��r")]
    public TextMeshProUGUI onStageCountText;
    [Header("�s�W�������")]
    public KeyCode AddButton;
    [Header("�R���������")]
    public KeyCode DeleteButton;
    [Header("����ƶ�")]
    public KeyCode Line_upButton;
    [Header("�ƶ���t��")]
    public float RowValue;
    [Header("�ƶ��C�t��")]
    public float ColumnValue;
    [Header("�ƶ��̤j���")]
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
    /// �s�W���
    /// </summary>
    public void AddUnit()
    {
        var obj = Instantiate(LittleUnit, SpawnPoint);
        unitsOnStage.Add(obj);
        unitActions.Add(obj.GetComponent<UnitAction>());
        onStageCountText.text = "Units on stage: " + unitsOnStage.Count;
    }
    /// <summary>
    /// �R�h�̫�@�ӳЫت����
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
    /// �w��C�@�Ӧblist����gameobject ���O������m�����L�̦��,�ñ���L�̪����V�Τ@����
    /// </summary>
    public void LineUpUnits()
    {
        var current = 0;
        float curRow = StartPoint.transform.position.x;
        float curColummn = StartPoint.transform.position.y;
        var maxRow = StartPoint.transform.position.x + rowMaxCount;
        while (current < unitActions.Count)
        {
            var targetVector = new Vector2(curRow * RowValue, curColummn * ColumnValue);
            StartCoroutine(unitActions[current].Command("LineUp", targetVector, 1));
            curRow++;
            //�W�X���w�̤j��ƫh����
            if (curRow >= maxRow)
            {
                curColummn++;
                curRow = StartPoint.transform.position.x;
            }
            current++;
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

}
