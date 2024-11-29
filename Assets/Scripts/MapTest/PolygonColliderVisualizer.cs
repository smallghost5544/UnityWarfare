using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonColliderVisualizer : MonoBehaviour
{
    public PolygonCollider2D polygonCollider;
    private LineRenderer lineRenderer;

    void Awake()
    {
        // 獲取組件
        polygonCollider = GetComponent<PolygonCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        // 設置 LineRenderer 屬性
        lineRenderer.positionCount = 0; // 初始化頂點數量
        lineRenderer.loop = true; // 繪製封閉形狀
        lineRenderer.startWidth = 0.1f; // 線條的寬度
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 使用默認材質
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        UpdateLineRendererPath();
    }

    void UpdateLineRendererPath()
    {
        // 取得多邊形的頂點
        Vector2[] pathPoints = polygonCollider.GetPath(0);
        lineRenderer.positionCount = pathPoints.Length;

        // 設置頂點到 LineRenderer
        for (int i = 0; i < pathPoints.Length; i++)
        {
            // 將本地座標轉換為世界座標
            Vector3 worldPoint = transform.TransformPoint(pathPoints[i]);
            lineRenderer.SetPosition(i, worldPoint);
        }
    }

    void OnValidate()
    {
        // 當參數變動時，更新 LineRenderer 的路徑
        if (polygonCollider != null && lineRenderer != null)
        {
            UpdateLineRendererPath();
        }
    }

    void OnDrawGizmos()
    {
        if (polygonCollider == null) return;

        Gizmos.color = Color.white; // 設定顏色

        // 繪製 PolygonCollider2D 的每個邊
        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            Vector2[] pathPoints = polygonCollider.GetPath(i);
            for (int j = 0; j < pathPoints.Length; j++)
            {
                Vector3 start = transform.TransformPoint(pathPoints[j]);
                Vector3 end = transform.TransformPoint(pathPoints[(j + 1) % pathPoints.Length]);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
