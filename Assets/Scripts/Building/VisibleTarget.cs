using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleTarget : MonoBehaviour
{
    public Camera playerCamera;    // 玩家攝影機
    public float toggleDistance = 10f;  // 設定顯示的最大距離
    public Renderer objectRenderer;   // 物件的渲染器
    public float CurrentDistance;
    void Start()
    {
        // 確保腳本啟動時獲取物件的 Renderer
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("物件缺少 Renderer 組件！");
        }
    }

    void Update()
    {
        //fix改為事件驅動
        // 計算攝影機和物件之間的距離
        CurrentDistance = playerCamera.orthographicSize;
        // 根據距離顯示或隱藏物件
        if (CurrentDistance >= toggleDistance)
        {
            // 距離小於設定值時顯示
            objectRenderer.enabled = true;
        }
        else
        {
            // 距離大於設定值時隱藏
            objectRenderer.enabled = false;
        }
    }
}
