using System.Collections;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class UnitView : MonoBehaviour
{
    Animator animator;
    Collider2D characterCollider = null;
    [SerializeField]
    GameObject hitFX;
    public GameObject arrowPrefab;
    public SpriteRenderer[] spriteRenderers; // 存儲角色上所有的 SpriteRenderer
    public Material flashMaterial; // 閃爍時使用的紅色材質
    public float flashDuration = 0.1f; // 閃爍持續時間
    private Material[] originalMaterials; // 存儲原始材質


    void Start()
    {
        // 收集所有 SpriteRenderer 的原始材質
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalMaterials = new Material[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalMaterials[i] = spriteRenderers[i].material;
        }
    }
    public void OnHit()
    {
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        // 將所有 SpriteRenderer 的材質設為閃爍材質
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material = flashMaterial;
        }

        // 等待閃爍持續時間
        yield return new WaitForSeconds(flashDuration);

        // 還原所有 SpriteRenderer 的原始材質
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = originalMaterials[i];
        }
    }
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
        //Debug.Log(start);
        //if (isWalking == true && start == true)
        //    return;
        //isWalking = start;
        animator.SetBool("Run", start);
    }

    public void DieAnimation()
    {
        animator.SetTrigger("Death");
        characterCollider.isTrigger = true;
    }

    public float AttackAnimation(int attackType, IDamageable target = null)
    {
        //animator.SetTrigger("Attack");
        float time = 0;
        if (attackType == 0)
        {
            animator.Play("2_Attack_Normal");
            time = GetAnimatoinTime("2_Attack_Normal");
            Vector3 effectPosition = (target as MonoBehaviour).transform.position + new Vector3(0, 0.25f, 0);
            StartCoroutine(DelayFX(time, effectPosition));
        }
        if (attackType == 1)
        {
            if(target!=null)
            animator.Play("2_Attack_Bow");
            ChangeToward((target as MonoBehaviour).gameObject.transform.position.x - transform.position.x);
            ShootArrow(target);
        }
        return time / 2; 
    }

    void ShootArrow(IDamageable target)
    {
        var arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        var arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.target = (target as MonoBehaviour).transform;
        arrowScript.shootPoint = transform;
        arrowScript.ShootArrow();
    }


    float GetAnimatoinTime(string animationName)
    {
        float playbackTime = 0;
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        foreach (var animationClip in animator.runtimeAnimatorController.animationClips)
        {
            print(animationClip.name);
            // 檢查動畫片段的名字
            if (animationClip.name == animationName)
            {
                // 獲取該動畫片段的播放時間
                 playbackTime = animationClip.length;
            }
        }
        print(playbackTime);
            return playbackTime;
    }
    IEnumerator DelayFX(float time , Vector3 effectPosition)
    {
        yield return new WaitForSeconds(time);
        Instantiate(hitFX, effectPosition, Quaternion.identity);
    }


}

