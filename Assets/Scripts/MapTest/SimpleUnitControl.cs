using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SimpleUnitControl : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rigid;
    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
        rigid=  GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (rigid.velocity.magnitude > 1.5f)
        {
            StartWalking(true);
        }
        else
            StartWalking(false);

        if (rigid.velocity.x < 0f)
        {
            ChangeFaceDirection(1);
        }
        if (rigid.velocity.x >0)
        {
            ChangeFaceDirection(-1);
        }
    }
    void StartWalking(bool isWalk)
    {
        animator.SetBool("Run", isWalk);
    }

    void ChangeFaceDirection(int direction)
    {
        UnityEngine.Vector3 direc = new UnityEngine.Vector3(direction, 1, 1);
        transform.localScale = direc;
    }
}
