using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonColliderVisualizer : MonoBehaviour
{
    public PolygonCollider2D polygonCollider;
    private LineRenderer lineRenderer;

    void Awake()
    {
        // ����ե�
        polygonCollider = GetComponent<PolygonCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        // �]�m LineRenderer �ݩ�
        lineRenderer.positionCount = 0; // ��l�Ƴ��I�ƶq
        lineRenderer.loop = true; // ø�s�ʳ��Ϊ�
        lineRenderer.startWidth = 0.1f; // �u�����e��
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // �ϥ��q�{����
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        UpdateLineRendererPath();
    }

    void UpdateLineRendererPath()
    {
        // ���o�h��Ϊ����I
        Vector2[] pathPoints = polygonCollider.GetPath(0);
        lineRenderer.positionCount = pathPoints.Length;

        // �]�m���I�� LineRenderer
        for (int i = 0; i < pathPoints.Length; i++)
        {
            // �N���a�y���ഫ���@�ɮy��
            Vector3 worldPoint = transform.TransformPoint(pathPoints[i]);
            lineRenderer.SetPosition(i, worldPoint);
        }
    }

    void OnValidate()
    {
        // ��Ѽ��ܰʮɡA��s LineRenderer �����|
        if (polygonCollider != null && lineRenderer != null)
        {
            UpdateLineRendererPath();
        }
    }

    void OnDrawGizmos()
    {
        if (polygonCollider == null) return;

        Gizmos.color = Color.white; // �]�w�C��

        // ø�s PolygonCollider2D ���C����
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
