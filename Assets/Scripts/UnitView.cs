using UnityEngine;

public class UnitView : MonoBehaviour
{
    Animator animator;
    Collider2D characterCollider = null;
    public GameObject arrowPrefab;
    /// <summary>
    /// 更改角色面向
    /// </summary>
    /// <param name="towardDiretion"></param>
    public void ChangeToward(float towardDiretion)
    {
        if (towardDiretion < 0)
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        else if (towardDiretion > 0)
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
    }
    public void GetAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void GetCollider()
    {
        characterCollider = gameObject.GetComponent<CircleCollider2D>();
    }
    public void SetColldier(bool setting)
    {
        characterCollider.isTrigger = setting;
    }
    public void StartWalkAnimation(bool start)
    {
        animator.SetBool("Run", start);
    }

    public void DieAnimation()
    {
        animator.SetTrigger("Death");
        characterCollider.isTrigger = true;
    }

    public void AttackAnimation(int attackType, UnitController Enemy = null)
    {
        //animator.SetTrigger("Attack");
        if (attackType == 0)
            animator.Play("2_Attack_Normal");
        if (attackType == 1)
        {
            animator.Play("2_Attack_Bow");
            ChangeToward(Enemy.gameObject.transform.position.x - transform.position.x);
            ShootArrow(Enemy);
        }
    }

    void ShootArrow(UnitController Enemy)
    {
        var arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        var arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.target = Enemy.transform;
        arrowScript.shootPoint = transform;
        arrowScript.ShootArrow();
    }
}
