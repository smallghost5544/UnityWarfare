using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PerformanceTest : MonoBehaviour
{
    public IDamageable Target;
    public UnitModel unitModel;
    public LayerMask enemyLayer;
    private void Awake()
    {
        // �����Ҧ� SpriteRenderer ����l����
       // spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

    }
    void Start()
    {
        int iterations = 1000; // �]�w�B�榸�ơA�Ҧp 100 �� 1000 ��

        // �Ыؤ@�� Stopwatch �ӰO���ɶ�
        Stopwatch stopwatch = new Stopwatch();

        // �}�l�p��
        stopwatch.Start();

        // ���ư���t��k�h��
        for (int i = 0; i < iterations; i++)
        {
            RunAlgorithm();  // �C������t��k
        }

        // ����p��
        stopwatch.Stop();

        // ��X�`�Ӯ� (�@��)
        UnityEngine.Debug.Log(iterations + " ���B���`�Ӯ�: " + stopwatch.ElapsedMilliseconds + " �@��");
        UnityEngine.Debug.Log("�C���B�業���Ӯ�: " + (stopwatch.ElapsedMilliseconds / (float)iterations) + " �@��");
    }

    // �A���t��k
    void RunAlgorithm()
    {
        // ���]�o�̬O�@�Ӯ��Ӹ귽���t��k
        SearchEnemy();
    }

    public void SearchEnemy()
    {
        //if (Target != null)
        //{
        //    MonoBehaviour targetMonoBehaviour = Target as MonoBehaviour;
        //    if (Vector2.Distance(transform.position, targetMonoBehaviour.transform.position) <= unitModel.AttackRange)
        //        // �p�G��e�ؼФ��b�d�򤺡A�h���i��s���j�M
        //        return;
        //}
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = (transform.position - collider.transform.position).sqrMagnitude;
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
            //ShowTarget = Target as UnitStats;
        }
    }
}
