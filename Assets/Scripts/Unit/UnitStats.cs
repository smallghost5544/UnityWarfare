
using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public enum UnitState
{
    Idle,
    Patrol,
    Walk,
    Attack,
    Commannd,
    Dead
}
public enum SpecialtyType
{
    BuildTower,
    MineResource,
    Patrol,
    Nothing
}
public class UnitStats : MonoBehaviour, IDamageable
{
    public UnitState CurrentState = UnitState.Idle;
    [Header("����ثe��q")]
    public int CurrentHP = 100;
    [Header("����}��Ǹ�")]
    //fix �۰ʶ�J�}��
    public int TeamNumer = 0;
    public float CurrentTime = 0;
    public UnitModel unitModel;
    [Header("�ثe��w�Ĥ�")]
    public IDamageable Target;
    public UnitStats ShowTarget;
    [Header("�Ĥ�h��")]
    //fixx �۰ʶ�J���}��
    public LayerMask enemyLayer;
    public LayerMask neutralObjectLayer;
    Collider2D movementArea;
    public Action OnDeathAction;
    public Action OnHitAction;
    public SpecialtyType UnitSpecialty;
    ISpecialty chooseSpecialty;
    public int CurrentHp { get => CurrentHP; set => throw new NotImplementedException(); }


    public void SetSpecialty()
    {
        string loadSpecialtyString = UnitSpecialty.ToString();
        if (loadSpecialtyString == "Nothing")
            return;
        chooseSpecialty = Resources.Load<GameObject>("LoadSpecialtyPrefab/BuildTower").GetComponent<ISpecialty>();
        //unitSpecialty = GetComponent<ISpecialty>();
    }
    public void SetUnitState(UnitState nextState = UnitState.Idle)
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
        if (Target != null)
        {
            MonoBehaviour targetMonoBehaviour = Target as MonoBehaviour;
            if (Vector2.Distance(transform.position, targetMonoBehaviour.transform.position) <= unitModel.AttackRange)
                // �p�G��e�ؼФ��b�d�򤺡A�h���i��s���j�M
                return;
        }
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            //float distance = Vector2.Distance(transform.position, collider.transform.position);
            float distance =(transform.position - collider.transform.position).sqrMagnitude;
            //�n�`�ٮį�ɦA���s�}��
            //if (distance < unitModel.AttackRange)
            //{
            //    Enemy = collider.GetComponent<UnitController>();
            //    break;
            //}
            if (distance < closestDistance)
            {
                closestDistance = distance;
                current = collider;
            }
        }
        if (current != null)
        {
            Target = current.GetComponent<IDamageable>();
            ShowTarget = Target as UnitStats;
        }
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
            x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
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

    public void DoExpert()
    {
        if (chooseSpecialty != null)
        {
            chooseSpecialty.DoSpecialize();
        }
        //fix ������ʦA��_idle
        //fix�]�w�ɶ�
        //fix �קK����Ĳ�o
        //Invoke("SetUnitSatate", 4f);
        SetUnitState(UnitState.Idle);
    }
    public IEnumerator Attack(float hitAnimationStartTime = 0)
    {
        SetUnitState(UnitState.Attack);
        //CurrentTime = unitModel.moveTime;
        yield return new WaitForSeconds(hitAnimationStartTime);
        if (Target != null )
            Target.GetHurt(UnityEngine.Random.Range(0, unitModel.AttackDamage));
    }
    //fix(�S���Ӫ���)
    public Collider2D FindNeutralObject()
    {
        neutralObjectLayer = LayerMask.GetMask("NeutralObject");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, neutralObjectLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                current = collider;
            }
        }
        //SetUnitState(UnitState.Idle);
        if (current != null)
        {
            Target = current.GetComponent<IDamageable>();
            return current;
        }
        return null;
    }

    public void GetHurt(int damage)
    {
        if (CurrentState == UnitState.Dead)
            return;
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentState = UnitState.Dead;
            //CurrentHP = 0;
            OnDeathAction?.Invoke(); // �� currentHP �p�� 0 �ɡAĲ�o Action
            return;
        }
        OnHitAction?.Invoke();
    }

    public IEnumerator Idle()
    {
        var waitTime = UnityEngine.Random.Range(0, unitModel.randomIdleTime);
        yield return new WaitForSeconds(waitTime);
        CurrentState = UnitState.Idle;
    }

}
