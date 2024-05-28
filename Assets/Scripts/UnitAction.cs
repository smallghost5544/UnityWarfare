using System.Collections;
using UnityEngine;

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
    /// <summary>
    /// ���ʥت��a
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
        }
    }
    /// <summary>
    /// �d���H����ܥت��a
    /// </summary>
    private void RandomSelectTarget()
    {
        //�H0.0�覡��ܽd�򤺥ت��a
        Bounds bounds = movementArea.bounds;
        float x = 1000;
        float y = 1000;
        //�Y���ͪ��I���b���w�d��,���s�H���@��
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
    /// <param name="towardDiretionnn"></param>
    void ChangeToward(float towardDiretionnn)
    {
        if (towardDiretionnn < 0)
            unit.gameObject.transform.localScale = new Vector3(1, 1, 1);
        else
            unit.gameObject.transform.localScale = new Vector3(-1, 1, 1);
    }
    /// <summary>
    /// �]�ߦ樫�ʵe
    /// </summary>
    /// <param name="set"></param>
    void StartWalkAnimation(bool start)
    {
        animator.SetBool("Run", start);
    }
    /// <summary>
    /// �w�﨤�⪺���O,��J�R�O�r��,�ت��a,���V����1or-1
    /// </summary>
    public IEnumerator Command(string command, Vector2 target = new Vector2(), float faceAngle = 1)
    {
        inCommand = true;
        //����collider�קK�I�����ܰ}��
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
