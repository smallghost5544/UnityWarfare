using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleTarget : MonoBehaviour
{
    public Camera playerCamera;    // ���a��v��
    public float toggleDistance = 10f;  // �]�w��ܪ��̤j�Z��
    public Renderer objectRenderer;   // ���󪺴�V��
    public float CurrentDistance;
    void Start()
    {
        // �T�O�}���Ұʮ�������� Renderer
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("����ʤ� Renderer �ե�I");
        }
    }

    void Update()
    {
        //fix�אּ�ƥ��X��
        // �p����v���M���󤧶����Z��
        CurrentDistance = playerCamera.orthographicSize;
        // �ھڶZ����ܩ����ê���
        if (CurrentDistance >= toggleDistance)
        {
            // �Z���p��]�w�Ȯ����
            objectRenderer.enabled = true;
        }
        else
        {
            // �Z���j��]�w�Ȯ�����
            objectRenderer.enabled = false;
        }
    }
}
