using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

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
    private Vector2 _targetPosition;
    /// <summary>
    /// 移動目的地
    /// </summary>
    public Vector2 targetPosition
    {
        get { return _targetPosition; }
        set
        {
            _targetPosition = RoundToOneDecimal(value);
            float direction = _targetPosition.x - transform.position.x;
            if (Mathf.Abs(direction) > 0.1f)
                ChangeToward(direction);
        }
    }
    bool isArriveTarget = true;
    bool inCommand = false;
    public bool inAction = false;
    Collider2D characterCollider = null;

    public float moveThreshold = 0.5f;
    private Vector3 lastPosition;
    private void Awake()
    {
        movementArea = GameObject.Find("MoveSpaceCollider").gameObject.GetComponent<PolygonCollider2D>();
        characterCollider = gameObject.GetComponent<CircleCollider2D>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (inCommand)
            WalkAnimationCheck();
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
            //case 2:
            //    WalkAnimationCheck ();
            //    break;
        }
    }
    /// <summary>
    /// 範圍內隨機選擇目的地
    /// </summary>
    public void RandomSelectTarget()
    {
        //Bounds bounds = movementArea.bounds;
        //float x = 1000;
        //float y = 1000;
        ////若產生的點不在指定範圍內,重新隨機一次
        //while (!movementArea.OverlapPoint(new Vector2(x, y)))
        //{
        //    x = Random.Range(bounds.min.x, bounds.max.x);
        //    y = Random.Range(bounds.min.y, bounds.max.y);
        //}
        //targetPosition = new Vector2(x, y);
        StartCoroutine(WalkToTarget());
    }

    public IEnumerator WalkToTarget()
    {
        isArriveTarget = false;
        StartWalkAnimation(true);
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
    /// <param name="towardDiretion"></param>
    public void ChangeToward(float towardDiretion)
    {
        if (towardDiretion < 0)
            unit.gameObject.transform.localScale = new Vector3(1, 1, 1);
        else if (towardDiretion > 0)
            unit.gameObject.transform.localScale = new Vector3(-1, 1, 1);
    }
    /// <summary>
    /// 設立行走動畫
    /// </summary>
    /// <param name="set"></param>
    public void StartWalkAnimation(bool start)
    {
        animator.SetBool("Run", start);
    }
    /// <summary>
    /// 針對角色的指令,輸入命令字串,目的地,面向角度1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        if (command == "LineUp")
        {
            inCommand = true;
            //關閉collider避免碰撞改變陣行
            characterCollider.isTrigger = true;
            targetPosition = target;
            yield return StartCoroutine(WalkToTarget());
            characterCollider.isTrigger = false;
            ChangeToward(faceAngle);
        }
        yield return null;
        if (command == "TakeABreak")
        {
            inCommand = false;
            characterCollider.isTrigger = false;
        }
    }

    public void AttackAnimation()
    {
        animator.SetTrigger("Attack");
    }
    public void DieAnimation()
    {
        animator.SetTrigger("Death");
        characterCollider.isTrigger = true;
    }
    //防守的一方可以用
    void WalkAnimationCheck()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        if (distance > moveThreshold)
        {
            StartWalkAnimation(true);
            lastPosition = transform.position;
        }
        else
            StartWalkAnimation(false);
    }

    private Vector2 RoundToOneDecimal(Vector2 vector)
    {
        return new Vector2(
            Mathf.Round(vector.x * 10f) / 10f,
            Mathf.Round(vector.y * 10f) / 10f
        );
    }
}
