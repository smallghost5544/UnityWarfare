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
    private int ChestHp = 100;

    public int CurrentHp { get => ChestHp; set => ChestHp = value; }

    Collider2D objCollider = null;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Open", false);
        state = ChestState.Close;
        objCollider = GetComponent<CircleCollider2D>();      


    }

    public void GetHurt(int damage)
    {
        ChestHp -= damage;
        if (ChestHp <= 0)
        {
            state = ChestState.Open;
            animator.SetBool("Open", true);
            Destroy(objCollider);
        }
    }
}
