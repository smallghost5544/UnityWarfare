using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    public List<GameObject> unitsOnStage = new List<GameObject>();
    public List<UnitController> unitActions = new List<UnitController>();
    public bool activeButtonOne = false;
    public bool activeButtonTwo = false;
    public Camera mainCamera;
    public GameObject unitOne;
    public GameObject unitTwo;
    Vector3 worldPosition;

    public float triggerRate = 10f; // �C��̦hĲ�o������
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
                Vector3 mousePosition = Input.mousePosition; // ����ƹ��b�ù��W���y��
                worldPosition = mainCamera.ScreenToWorldPoint(mousePosition); // �ഫ���@�ɮy��
                worldPosition.z = 0; // �T�O�ͦ���m�b2D�����W
                CreateUnitOnScreen(worldPosition, unitOne);
                nextTriggerTime = Time.time + 1f / triggerRate;
            }
        }
        if (activeButtonTwo && (Input.GetMouseButton(0)))
        {
            if (Time.time >= nextTriggerTime)
            {
                Vector3 mousePosition = Input.mousePosition; // ����ƹ��b�ù��W���y��
                worldPosition = mainCamera.ScreenToWorldPoint(mousePosition); // �ഫ���@�ɮy��
                worldPosition.z = 0; // �T�O�ͦ���m�b2D�����W
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
    /// �s�W���
    /// </summary>
    public void AddUnit()
    {
        var obj = Instantiate(LittleUnit, SpawnPoint);
        unitsOnStage.Add(obj);
        unitActions.Add(obj.GetComponent<UnitController>());
        onStageCountText.text = "Units on stage: " + unitsOnStage.Count;
    }
    /// <summary>
    /// ���U�ӳ����s��,��ù��W�I����m�i�ͦ����
    /// </summary>
    public void CreateUnitOnScreen(Vector2 initPlace, GameObject unit)
    {
        var obj = Instantiate(unit, initPlace, Quaternion.identity);
        unitsOnStage.Add(obj);
        unitActions.Add(obj.GetComponent<UnitController>());
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
            unitActions.Remove(deleteUnit.GetComponent<UnitController>());
            onStageCountText.text = "Units on stage: " + unitsOnStage.Count;
        }
    }

    /// <summary>
    /// �w��C�@�Ӧblist����gameobject ���O������m�����L�̦��,�ñ���L�̪����V�Τ@����
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
        //�Y���b�ؿv����� �⤤�Z����_�쥻�Z��
        //fix �`�N�į�
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
        //�ˬd�ؿv��
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
    //fix �S�̷ӥ����߶}�l��
    //void Square()
    //{
    //    //�H�_�l�I�����I�}�l�Ƨ�
    //    var currentUnit = 0;
    //    //�]������},�ҥH���`�ƪ��}�ڸ����@�C���X�H
    //    int maxColumn = (int)Mathf.Round(Mathf.Sqrt(unitsOnStage.Count));
    //    float curRow = Mathf.Round(StartPoint.transform.position.x * 100f) / 100f;
    //    float curColumn = Mathf.Round(StartPoint.transform.position.y * 100f) / 100f;
    //    float saveFirstPlace = curColumn;
    //    float columnCount = 0;
    //    //���Ʈɤ����I�b��Ƥ���,��Ʈɤ����I���̤�����Ʊ�
    //    var columnCenter = unitActions.Count / maxColumn;
    //    while (currentUnit < unitActions.Count)
    //    {
    //        //�q�����I���W�Ƥ@�b,���U�Ƥ@�b
    //        var targetVector = new Vector2(curRow * RowValue, (curColumn + columnCenter / 2) * ColumnValue);
    //        StartCoroutine(unitActions[currentUnit].Command("LineUp", targetVector, 1));
    //        curColumn--;
    //        columnCount++;
    //        //�W�X���w�̤j��ƫh����
    //        if (columnCount >= maxColumn)
    //        {
    //            //�ѥk����,�ҥH--
    //            curRow--;
    //            //�^�k�Ĥ@�Ӧ�m�}�l��
    //            curColumn = saveFirstPlace;
    //            columnCount = 0;
    //        }
    //        currentUnit++;
    //    }
    //}
    /// <summary>
    /// �T���}�� 
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
            //�q�����I���W�Ƥ@�b,���U�Ƥ@�b
            var targetVector = new Vector2(curRow * RowValue, (curColumn + maxColumn / 2) * ColumnValue);
            StartCoroutine(unitActions[currentUnit].Command("LineUp", targetVector, 1));
            curColumn--;
            columnCount++;
            //�W�X���w�̤j��ƫh����
            if (columnCount >= maxColumn)
            {
                //�ѥk����,�ҥH--
                curRow--;
                //�^�k�Ĥ@�Ӧ�m�}�l��
                curColumn = firstColumnPlace;
                columnCount = 0;
                maxColumn += 2;
            }
            currentUnit++;
        }
    }

    public void Rectangle()
    {
        //�ۭq�̤j��ƪ�����,�ثe�Q�Ω����ΰ}�C
        var current = 0;
        var rowCount = 0;
        //�ƥ����o�`�C��,������ߥH�����I�ƶ�
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
        //��b�|
        float radius = 1;
        //�ثe�ĴX�ӳ��
        var current = 0;
        //�ثe��P�ĴX�ӳ��
        var radiusCount = 0;
        //���
        var center = StartPoint.transform.position;
        //���ꨤ��
        var unitAngle = (int)(360 / Mathf.Floor(2 * radius * Mathf.PI));
        while (current < unitActions.Count)
        {
            //�W�L360�׫h�ƥh�U�@��
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
            mousePos.z = Camera.main.WorldToScreenPoint(StartPoint.transform.position).z; // �T�O�ڭ̪��I���I���`�׭�
            // �N�ù��y���ഫ���@�ɮy��
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

        // �ϥγ����W�٭��s�[������
        SceneManager.LoadScene(currentScene.name);
    }
}

