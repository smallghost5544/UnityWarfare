using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class UnitAction : MonoBehaviour
{
    [Header("���⪫��")]
    public GameObject unit;
    [Header("����animator")]
    public Animator animator;
    [Header("���Ⲿ�ʳt��")]
    public float moveSpeed = 1;
    [Header("�i���ʽd��")]
    public Collider2D movementArea;
    [Header("����̤j�o�b�ɶ�")]
    public float randomIdleTime = 5;
    private Vector2 _targetPosition;
    /// <summary>
    /// ���ʥت��a
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
    /// ����欰
    /// </summary>
    void UnCombatActions()
    {
        inAction = true;
        switch (Random.Range(0, 2))
        {
            case 0:
                //��ܷs�ؼШëe�i
                RandomSelectTarget();
                break;
            case 1:
                //�o�b
                StartCoroutine(Idle());
                break;
            //case 2:
            //    WalkAnimationCheck ();
            //    break;
        }
    }
    /// <summary>
    /// �d���H����ܥت��a
    /// </summary>
    public void RandomSelectTarget()
    {
        //Bounds bounds = movementArea.bounds;
        //float x = 1000;
        //float y = 1000;
        ////�Y���ͪ��I���b���w�d��,���s�H���@��
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
            // �p���V�V�q
            Vector2 direction = (targetPosition - currentPosition).normalized;
            // �p��s��m
            Vector2 newPosition = currentPosition + direction * moveSpeed * Time.deltaTime;
            // ��s��m
            transform.position = newPosition;
            // �ˬd�O�_��F�ؼЦ�m
            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                // ����ʡA�ñN��m�]����T���ؼЦ�m
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
    /// ��﨤�⭱�V
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
    /// �]�ߦ樫�ʵe
    /// </summary>
    /// <param name="set"></param>
    public void StartWalkAnimation(bool start)
    {
        animator.SetBool("Run", start);
    }
    /// <summary>
    /// �w�﨤�⪺���O,��J�R�O�r��,�ت��a,���V����1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        if (command == "LineUp")
        {
            inCommand = true;
            //����collider�קK�I�����ܰ}��
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
    //���u���@��i�H��
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
