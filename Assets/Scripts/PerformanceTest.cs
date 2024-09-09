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
        // 收集所有 SpriteRenderer 的原始材質
       // spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

    }
    void Start()
    {
        int iterations = 1000; // 設定運行次數，例如 100 或 1000 次

        // 創建一個 Stopwatch 來記錄時間
        Stopwatch stopwatch = new Stopwatch();

        // 開始計時
        stopwatch.Start();

        // 重複執行演算法多次
        for (int i = 0; i < iterations; i++)
        {
            RunAlgorithm();  // 每次執行演算法
        }

        // 停止計時
        stopwatch.Stop();

        // 輸出總耗時 (毫秒)
        UnityEngine.Debug.Log(iterations + " 次運行總耗時: " + stopwatch.ElapsedMilliseconds + " 毫秒");
        UnityEngine.Debug.Log("每次運行平均耗時: " + (stopwatch.ElapsedMilliseconds / (float)iterations) + " 毫秒");
    }

    // 你的演算法
    void RunAlgorithm()
    {
        // 假設這裡是一個消耗資源的演算法
        SearchEnemy();
    }

    public void SearchEnemy()
    {
        //if (Target != null)
        //{
        //    MonoBehaviour targetMonoBehaviour = Target as MonoBehaviour;
        //    if (Vector2.Distance(transform.position, targetMonoBehaviour.transform.position) <= unitModel.AttackRange)
        //        // 如果當前目標仍在範圍內，則不進行新的搜尋
        //        return;
        //}
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, unitModel.SearchRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Collider2D current = null;
        foreach (Collider2D collider in hitColliders)
        {
            float distance = (transform.position - collider.transform.position).sqrMagnitude;
            //要節省效能時再重新開啟
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
