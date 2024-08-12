
using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public enum UnitState
    {
        Idle,
        Patrol,
        Walk,
        Attack,
        Commannd,
        Dead
    }
    public UnitState CurrentState = UnitState.Idle;
    [Header("����ثe��q")]
    public int CurrentHP = 100;
    [Header("����}��Ǹ�")]
    //fix �۰ʶ�J�}��
    public int TeamNumer = 0;
    public float CurrentTime = 0;
    public UnitModel unitModel;
    [Header("�ثe��w�Ĥ�")]
    public UnitController Enemy;
    [Header("�Ĥ�h��")]
    //fixx �۰ʶ�J���}��
    public LayerMask enemyLayer;
    Collider2D movementArea;

    public void SetUnitState(UnitState nextState)
    {
        CurrentState = nextState;
    }
    /// <summary>
    /// �]�w��eHP���̤jHP
    /// </summary>
    public void SetCurentHP()
    {
        CurrentHP = unitModel.MaxHP;
    }
    /// <summary>
    /// ��X�i���ʰϰ쪫��
    /// </summary>
    public void FindMovementArea()
    {
        movementArea = GameObject.Find("MoveSpaceCollider").gameObject.GetComponent<PolygonCollider2D>();
    }
    /// <summary>
    /// ��M�Ĥ���
    /// </summary>
    public void SearchEnemy()
    {
        if (Enemy != null && Vector2.Distance(transform.position, Enemy.transform.position) <= unitModel.AttackRange)
        {
            // �p�G��e�ؼФ��b�d�򤺡A�h���i��s���j�M
            return;
        }
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < unitModel.AttackRange)
            {
                Enemy = collider.GetComponent<UnitController>();
                break;
            }
            if (distance < closestDistance)
            {
                closestDistance = distance;
                current = collider;
            }
        }
        if(current != null)
                Enemy = current.GetComponent<UnitController>();
    }
    /// <summary>
    /// �H����ܥت��a
    /// </summary>
    /// <returns></returns>
    public Vector2 RandomSelectTarget()
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
        var targetPosition = new Vector2(x, y);
        return targetPosition; 
    }

    /// <summary>
    /// ���p�ƦܲĤ@��
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public Vector2 RoundToOneDecimal(Vector2 vector)
    {
        return new Vector2(
            Mathf.Round(vector.x * 10f) / 10f,
            Mathf.Round(vector.y * 10f) / 10f
        );
    }

}
