using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum ChestState
{
    Close,
    Open,
}
public class ChestController : MonoBehaviour, IDamageable
{
    private Animator animator;
    public ChestState state;
    public int MaxChestHp = 100;
    public int ChestHp = 100;
    /// <summary>
    /// 查找用
    /// </summary>
    public int CurrentHp { get => ChestHp; set => ChestHp = MaxChestHp; }
    public ObjectType ObjType;
    Collider2D objCollider = null;
    List<GameObject> hpBarGroup;
    //public SpriteRenderer foregroundBar;  // 前景血條
    public Transform healthBarTransform; // 血條Transform，用於縮放
    public LayerMask ChangeLayer;
    Action firstTimeGetHit;
    void OnEnable()
    {
        CloseHealthBar();
        firstTimeGetHit = OpenHealthBar;
        ChestHp = MaxChestHp;
        animator = GetComponent<Animator>();
        Transform childTransform = transform.Find("HealthBar");
        healthBarTransform = childTransform.GetComponent<Transform>();
        animator.SetBool("Open", false);
        state = ChestState.Close;
        objCollider = GetComponent<BoxCollider2D>();
        UpdateHealthBar();
    }

    public void GetHurt(int damage)
    {
        firstTimeGetHit?.Invoke();
        firstTimeGetHit = null;

        var update = ChestHp -= damage;
        UpdateHealthBar();
        if (!inFx)
        {
            inFx = true;
            StartCoroutine(MiningScaleFX());
        }
        if (ChestHp <= 0)
        {
            state = ChestState.Open;
            animator.SetBool("Open", true);
            CloseHealthBar();
            Destroy(objCollider);
            gameObject.layer = ChangeLayer;
        }
    }

    public ObjectType GetObjectType()
    {
        return ObjType;
    }

    private void UpdateHealthBar()
    {
        try
        {
            float healthPercent = (float)ChestHp / (float)MaxChestHp;
            if (healthPercent <= 0)
                healthPercent = 0;
            healthBarTransform.localScale = new Vector3(healthPercent, 0.5f, 1); // 更新 X 軸縮放
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError($"Null reference caught: {ex.Message}");
        }
    }

    void CloseHealthBar()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    void OpenHealthBar()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
        }
    }


    bool inFx = false;
    IEnumerator MiningScaleFX()
    {
        var originalScale = gameObject.transform.localScale;
        var newScale = originalScale;
        var originTransform = gameObject.transform.position;
        var changeTransform = originTransform;
        newScale *= 0.95f;
        changeTransform.x += 0.02f;
        float recoverTime = 0.1f;
        gameObject.transform.localScale = newScale;
        gameObject.transform.position = changeTransform;
        yield return new WaitForSeconds(recoverTime);
        gameObject.transform.localScale = originalScale;
        gameObject.transform.position = originTransform;
        inFx = false;
    }
}
