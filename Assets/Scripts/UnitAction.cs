using System.Collections;
using UnityEngine;

public class UnitAction : MonoBehaviour
{
    [Header("角色物件")]
    public GameObject unit;
    [Header("角色animator")]
    public Animator animator;
    [Header("角色移動速度")]
    public float moveSpeed = 1;
    [Header("可移動範圍")]
    public Collider2D movementArea;
    [Header("角色最大發呆時間")]
    public float randomIdleTime = 5;
    /// <summary>
    /// 移動目的地
    /// </summary>
    public Vector2 targetPosition = Vector2.zero;
    bool isArriveTarget = true;
    bool inCommand = false;
    bool inAction = false;

    private void Awake()
    {
        movementArea = GameObject.Find("MoveSpaceCollider").gameObject.GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        if (!inAction && !inCommand)
            UnCombatActions();
    }
    /// <summary>
    /// 角色行為
    /// </summary>
    void UnCombatActions()
    {
        inAction = true;
        switch (Random.Range(0, 2))
        {
            case 0:
                //選擇新目標並前進
                RandomSelectTarget();
                break;
            case 1:
                //發呆
                StartCoroutine(Idle());
                break;
        }
    }
    /// <summary>
    /// 範圍內隨機選擇目的地
    /// </summary>
    private void RandomSelectTarget()
    {
        //以0.0方式選擇範圍內目的地
        Bounds bounds = movementArea.bounds;
        float x = 1000;
        float y = 1000;
        //若產生的點不在指定範圍內,重新隨機一次
        while (!movementArea.OverlapPoint(new Vector2(x, y)))
        {
            x = Random.Range(bounds.min.x, bounds.max.x);
            y = Random.Range(bounds.min.y, bounds.max.y);
            x = Mathf.Round(x * 10f) / 10f;
            y = Mathf.Round(y * 10f) / 10f;
        }
        targetPosition = new Vector2(x, y);
        StartCoroutine(WalkToTarget());
    }

    public IEnumerator WalkToTarget()
    {
        isArriveTarget = false;
        StartWalkAnimation(true);
        ChangeToward(targetPosition.x - transform.position.x);
        while (!isArriveTarget)
        {
            yield return null;
            Vector2 currentPosition = transform.position;
            // 計算方向向量
            Vector2 direction = (targetPosition - currentPosition).normalized;
            // 計算新位置
            Vector2 newPosition = currentPosition + direction * moveSpeed * Time.deltaTime;
            // 更新位置
            transform.position = newPosition;
            // 檢查是否到達目標位置
            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                // 停止移動，並將位置設為精確的目標位置
                inAction = false;
                isArriveTarget = true;
                StartWalkAnimation(false);
                transform.position = targetPosition;
            }
        }
    }

    IEnumerator Idle()
    {
        var waitTime = Random.Range(0, randomIdleTime);
        yield return new WaitForSeconds(waitTime);
        inAction = false;
    }

    /// <summary>
    /// 更改角色面向
    /// </summary>
    /// <param name="towardDiretionnn"></param>
    void ChangeToward(float towardDiretionnn)
    {
        if (towardDiretionnn < 0)
            unit.gameObject.transform.localScale = new Vector3(1, 1, 1);
        else
            unit.gameObject.transform.localScale = new Vector3(-1, 1, 1);
    }
    /// <summary>
    /// 設立行走動畫
    /// </summary>
    /// <param name="set"></param>
    void StartWalkAnimation(bool start)
    {
        animator.SetBool("Run", start);
    }
    /// <summary>
    /// 針對角色的指令,輸入命令字串,目的地,面向角度1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        inCommand = true;
        //關閉collider避免碰撞改變陣行
        CircleCollider2D col = gameObject.GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        if (command == "LineUp")
        {
            targetPosition = target;
            yield return StartCoroutine(WalkToTarget());
            ChangeToward(faceAngle);
        }
        yield return null;
        if (command == "TakeABreak")
        {
            inCommand = false;
            col.isTrigger = false;
        }
    }
}
