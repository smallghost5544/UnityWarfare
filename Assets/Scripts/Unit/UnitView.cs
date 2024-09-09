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
    public SpriteRenderer[] spriteRenderers; // �s�x����W�Ҧ��� SpriteRenderer
    public Material flashMaterial; // �{�{�ɨϥΪ��������
    public float flashDuration = 0.1f; // �{�{����ɶ�
    private Material[] originalMaterials; // �s�x��l����
    bool onHitRecover = false;

    void Start()
    {
        // �����Ҧ� SpriteRenderer ����l����
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
        // 1.�]�m�z���׬��ؼЭ�
        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = changeAlpha;
            spriteRenderer.color = color;
        }
        // 2. ���ݫ��w���ɶ�
        //yield return new WaitForSeconds(0.1f);

        // 3. �w�C��_�z����
        float elapsedTime = 0f; // �p�ɾ�
        float currentAlpha = changeAlpha; // ��e�z����

        // �v�B��_�z����
        while (elapsedTime < fadeDuration)
        {
            // �p��C�V�s���z���׭�
            elapsedTime += Time.deltaTime;
            foreach (var spriteRenderer in spriteRenderers)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(currentAlpha, originalAlpha, elapsedTime / fadeDuration);
                spriteRenderer.color = color;
            }
            // ���ݤU�@�V
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
    /// ��﨤�⭱�V
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
            // �ˬd�ʵe���q���W�r
            if (animationClip.name == animationName)
            {
                // ����Ӱʵe���q������ɶ�
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


