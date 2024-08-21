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
    public SpriteRenderer[] spriteRenderers; // �s�x����W�Ҧ��� SpriteRenderer
    public Material flashMaterial; // �{�{�ɨϥΪ��������
    public float flashDuration = 0.1f; // �{�{����ɶ�
    private Material[] originalMaterials; // �s�x��l����


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
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        // �N�Ҧ� SpriteRenderer ������]���{�{����
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material = flashMaterial;
        }

        // ���ݰ{�{����ɶ�
        yield return new WaitForSeconds(flashDuration);

        // �٭�Ҧ� SpriteRenderer ����l����
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = originalMaterials[i];
        }
    }
    /// <summary>
    /// ��﨤�⭱�V
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
    IEnumerator DelayFX(float time , Vector3 effectPosition)
    {
        yield return new WaitForSeconds(time);
        Instantiate(hitFX, effectPosition, Quaternion.identity);
    }


}

