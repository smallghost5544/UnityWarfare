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
    public GameObject KnockDownAni;
    public SpriteRenderer[] spriteRenderers; // 存儲角色上所有的 SpriteRenderer
    public Material flashMaterial; // 閃爍時使用的紅色材質
    public float flashDuration = 0.1f; // 閃爍持續時間
    private Material[] originalMaterials; // 存儲原始材質
    bool onHitRecover = false;

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
        if (gameObject.activeInHierarchy == false)
            return; 
        StartCoroutine(FlashEffect());
    }
    private float originalAlpha = 1f;
    float fadeDuration = 0.6f;
    float changeAlpha = 0.8f;
    private IEnumerator FlashEffect()
    {
        if (onHitRecover == true)
            yield break;
        onHitRecover = true;
        // 1.設置透明度為目標值
        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = changeAlpha;
            spriteRenderer.color = color;
        }
        // 2. 等待指定的時間
        //yield return new WaitForSeconds(0.1f);

        // 3. 緩慢恢復透明度
        float elapsedTime = 0f; // 計時器
        float currentAlpha = changeAlpha; // 當前透明度

        // 逐步恢復透明度
        while (elapsedTime < fadeDuration)
        {
            // 計算每幀新的透明度值
            elapsedTime += Time.deltaTime;
            foreach (var spriteRenderer in spriteRenderers)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(currentAlpha, originalAlpha, elapsedTime / fadeDuration);
                spriteRenderer.color = color;
            }
            // 等待下一幀
            yield return null;
        }
        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = originalAlpha;
            spriteRenderer.color = color;
        }
        onHitRecover = false;
    }
    /// <summary>
    /// 更改角色面向
    /// </summary>
    /// <param name="towardDiretion"></param>
    public void ChangeToward(float towardDiretion)
    {
        if (towardDiretion < 0)
        {
            Vector3 scale = gameObject.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(1);
            gameObject.transform.localScale = scale;
        }
        else if (towardDiretion > 0)
        {
            Vector3 scale = gameObject.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(-1);
            gameObject.transform.localScale = scale;
        }
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
        // Invoke("InitKnockDownAnimation", 0.5f);
    }
    void InitKnockDownAnimation()
    {
        Instantiate(KnockDownAni, transform.position, Quaternion.identity);
    }
    public void AttackAnimation(int attackType, IDamageable target = null)
    {
        //animator.SetTrigger("Attack");

        if (attackType == 0)
        {
            animator.Play("2_Attack_Normal");
            //time = GetAnimatoinTime("2_Attack_Normal");
            Vector3 effectPosition = (target as MonoBehaviour).transform.position + new Vector3(0, 0.25f, 0);
            //StartCoroutine(DelayFX(0.1f, effectPosition));
        }
        if (attackType == 1)
        {
            if (target != null)
                animator.Play("2_Attack_Bow");
            ChangeToward((target as MonoBehaviour).gameObject.transform.position.x - transform.position.x);
            ShootArrow(target);
        }
        //return time / 2; 
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
    IEnumerator DelayFX(float time, Vector3 effectPosition)
    {
        yield return new WaitForSeconds(time);
        Instantiate(hitFX, effectPosition, Quaternion.identity);
    }


}


