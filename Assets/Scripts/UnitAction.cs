
using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


public class UnitAction : MonoBehaviour
{
    [Header("�Ĥ�h��")]
    public LayerMask enemyLayer;
    [Header("���⪫��")]
    public GameObject unit;
    [Header("����animator")]
    public Animator animator;
    [Header("�i���ʽd��")]
    public Collider2D movementArea;
    [Header("�ثe��w�Ĥ�")]
    public UnitAction Enemy;
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
    private Vector3 lastPosition;
    Collider2D characterCollider = null;
    public UnitAbility unitAbility;
    public bool inAction = false;
    bool isArriveTarget = true;
    bool inCommand = false;
    bool inCombatAction = false;
    public string teamString;
    GameManager gameManager;
    //use for archer
    public bool isArcher = false;
    public bool stayidle = false;
    public GameObject arrowPrefab;

    private void Awake()
    {
        lastPosition = transform.position;
        gameManager = GameManager.Instance;
        unitAbility = GetComponent<UnitAbility>();
        characterCollider = gameObject.GetComponent<CircleCollider2D>();
        movementArea = GameObject.Find("MoveSpaceCollider").gameObject.GetComponent<PolygonCollider2D>();
    }
    private void OnEnable()
    {
        StopAllCoroutines();
        CancelInvoke("SearchEnemy");
        InvokeRepeating("SearchEnemy", 0f, 1.5f);
        gameObject.layer = unitAbility.TeamNumer;
        //StartCoroutine(Check());
    }
    //public IEnumerator Check()
    //{
    //    // Measure the execution time of SearchEnemy function
    //    Stopwatch stopwatch1 = Stopwatch.StartNew();
    //    yield return SearchEnemy();
    //    stopwatch1.Stop();
    //    UnityEngine.Debug.Log("SearchEnemy execution time: " + stopwatch1.ElapsedMilliseconds + " milliseconds");

    //    // Measure the execution time of SearchEnemyQuadtree function
    //    Stopwatch stopwatch2 = Stopwatch.StartNew();
    //    yield return SearchEnemyQuadtree();
    //    stopwatch2.Stop();
    //    UnityEngine.Debug.Log("SearchEnemyQuadtree execution time: " + stopwatch2.ElapsedMilliseconds + " milliseconds");
    //}

    void Update()
    {
        //�Y���B��ʧ@��, ���O�� �B�S���Ĥ��H
        if (!inAction && !inCommand && Enemy == null)
            UnCombatActions();
        else if (!inCombatAction)
        {
            inCombatAction = true;
            inAction = true;
            WalkOrAttack();
        }

        if (inAction)
            WalkAnimationCheck();
        nextMoveTime();
    }
    /// <summary>
    /// ����欰
    /// </summary>
    void UnCombatActions()
    {
        inAction = true;
        if (stayidle)
            return;
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
        Bounds bounds = movementArea.bounds;
        float x = 1000;
        float y = 1000;
        //�Y���ͪ��I���b���w�d��,���s�H���@��
        while (!movementArea.OverlapPoint(new Vector2(x, y)))
        {
            x = Random.Range(bounds.min.x, bounds.max.x);
            y = Random.Range(bounds.min.y, bounds.max.y);
        }
        targetPosition = new Vector2(x, y);
        StartCoroutine(WalkToTarget());
    }

    public IEnumerator WalkToTarget()
    {
        isArriveTarget = false;
        StartWalkAnimation(true);
        while (!isArriveTarget)
        {
            yield return null;
            if (isArcher)
            {
                if (Enemy != null && Vector3.Magnitude(Enemy.gameObject.transform.position - transform.position) < unitAbility.AttackRange)
                    isArriveTarget = true;
            }
            Vector2 currentPosition = transform.position;
            // �p���V�V�q
            Vector2 direction = (targetPosition - currentPosition).normalized;
            // �p��s��m
            Vector2 newPosition = currentPosition + direction * unitAbility.moveSpeed * Time.deltaTime;
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
        var waitTime = Random.Range(0, unitAbility.randomIdleTime);
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
            inAction = false;
            Enemy = null;
            characterCollider.isTrigger = false;
        }
    }

    //���u���@��i�H��
    void WalkAnimationCheck()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        if (distance > unitAbility.moveThreshold)
        {
            StartWalkAnimation(true);
            lastPosition = transform.position;
        }
        else
            StartWalkAnimation(false);
    }

    void SearchEnemy()
    {

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitAbility.SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < unitAbility.AttackRange)
            {
                Enemy = collider.GetComponent<UnitAction>();
                break;
            }
            if (distance < closestDistance)
            {
                closestDistance = distance;
                Enemy = collider.GetComponent<UnitAction>();
            }
        }

    }

    IEnumerator SearchEnemyQuadtree()
    {
        Bounds searchBounds = new Bounds(transform.position, new Vector3(unitAbility.SearchRange, unitAbility.SearchRange, 0));
        var nearbyEnemies = gameManager.quadtree.Search(searchBounds);
        float distance = Mathf.Infinity;
        GameObject enemy = null;
        print("nearbyEnemiesCount   " + nearbyEnemies.Count);
        foreach (var obj in nearbyEnemies)
        {
            if (obj.layer == gameObject.layer)
                continue;
            var dis = Vector3.Distance(transform.position, obj.transform.position);
            if (dis < distance)
            {
                distance = dis;
                enemy = obj;
            }
        }
        if (enemy != null)
        {
            Enemy = enemy.GetComponent<UnitAction>();
        }

        yield return null;
    }
    void WalkOrAttack()
    {
        unitAbility.currentTime = Random.Range(0, unitAbility.moveTime * 2);
        if (Enemy == null)
            return;
        if (Enemy.gameObject.activeInHierarchy)
        {
            if (Vector3.Magnitude(Enemy.gameObject.transform.position - transform.position) > unitAbility.AttackRange)
            {
                targetPosition = Enemy.transform.position;
                if (isArriveTarget)
                    StartCoroutine(WalkToTarget());
            }
            else
            {
                Attack();
            }
        }
    }
    void Attack()
    {
        Enemy.GetHurt(Random.Range(0, unitAbility.AttackDamage));
        AttackAnimation();
    }
    /// <summary>
    /// �԰����U����ʮɶ�
    /// </summary>
    void nextMoveTime()
    {
        if (inCombatAction)
        {
            unitAbility.currentTime -= Time.deltaTime;
            if (unitAbility.currentTime <= 0.01f)
                inCombatAction = false;
        }
    }
    public void GetHurt(int damage)
    {
        unitAbility.HP -= damage;
        if (unitAbility.HP < 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        DieAnimation();
        var test = GameManager.Instance.GetComponentInChildren<TestButtonFunctions>();
        test.unitActions.Remove(this);
        isArriveTarget = true;
        var layer = GetComponentInChildren<SortingGroup>();
        layer.sortingOrder -= 1;
        gameObject.layer = 0;
        inAction = true;
        yield return new WaitForSeconds(1f);
        //Destroy(unitAbility);
        //Destroy(this);
        ResetDied();
    }

    void ResetDied()
    {
        unitAbility.HP = unitAbility.MaxHp;
        gameObject.transform.position = new Vector3(100, 100, 100);
        characterCollider.isTrigger = false;
        Enemy = null;
        inAction = false;
        isArriveTarget = true;
        inCommand = false;
        inCombatAction = false;
        //CancelInvoke("SearchEnemy");
        //objectpool�^��
        ObjectPool.Instance.Return(teamString, gameObject);
        gameManager.quadtree.Remove(gameObject);
    }
    /// <summary>
    /// �I��Ĥ��찱���
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            LayerMask layer = collision.gameObject.layer;
            if (layer != gameObject.layer)
                targetPosition = transform.position;
        }
    }


    private Vector2 RoundToOneDecimal(Vector2 vector)
    {
        return new Vector2(
            Mathf.Round(vector.x * 10f) / 10f,
            Mathf.Round(vector.y * 10f) / 10f
        );
    }
    /// <summary>
    /// �]�ߦ樫�ʵe
    /// </summary>
    /// <param name="set"></param>
    public void StartWalkAnimation(bool start)
    {
        animator.SetBool("Run", start);
    }
    public void AttackAnimation()
    {
        //animator.SetTrigger("Attack");
        if (unitAbility.attackType == 0)
            animator.Play("2_Attack_Normal");
        if (unitAbility.attackType == 1)
        {
            animator.Play("2_Attack_Bow");
            ChangeToward(Enemy.gameObject.transform.position.x - transform.position.x);
            ShootArrow();
        }
    }
    public void DieAnimation()
    {
        animator.SetTrigger("Death");
        characterCollider.isTrigger = true;
    }
    void ShootArrow()
    {
        var arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        var arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.target = Enemy.transform;
        arrowScript.shootPoint = transform;
        arrowScript.ShootArrow();
    }
}
