using System.Collections;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;

public enum ArrowType
{
    unitArrow,
    towerArrow
}
public class UnitView : MonoBehaviour
{
    Animator animator;
    Collider2D characterCollider = null;
    [SerializeField]
    GameObject hitFX;
    //public GameObject arrowPrefab;
    public GameObject KnockDownAni;
    public SpriteRenderer[] spriteRenderers; // 存儲角色上所有的 SpriteRenderer
    public Material flashMaterial; // 閃爍時使用的紅色材質
    public float flashDuration = 0.1f; // 閃爍持續時間
    private Material[] originalMaterials; // 存儲原始材質
    bool onHitRecover = false;
    ObjectPool objPool;
    public ArrowType arrowtype;
    int arrowTypeNum = 0;       
    void Start()
    {
        // 收集所有 SpriteRenderer 的原始材質
        if (arrowtype == ArrowType.unitArrow)
        {
            arrowTypeNum = 3;
        }
        else
        {
            arrowTypeNum = 4;
        }
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalMaterials = new Material[spriteRenderers.Length];
        objPool = ObjectPool.Instance;
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
    public void RecoverAlpha()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
        }
    }
    private float originalAlpha = 1f;
    float fadeDuration = 0.6f;
    float changeAlpha = 0.5f;
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
    public float knockDownFadeTime = 2f;
    private IEnumerator DestroyFlashEffect()
    {
        float elapsedTime = 0f; // 計時器
        float currentAlpha = 1; // 當前透明度
        // 逐步透明
        while (elapsedTime < knockDownFadeTime)
        {
            // 計算每幀新的透明度值
            elapsedTime += Time.deltaTime;
            foreach (var spriteRenderer in spriteRenderers)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(currentAlpha, 0, elapsedTime / knockDownFadeTime);
                spriteRenderer.color = color;
            }
            // 等待下一幀
            yield return null;
        }
        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
        }
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
        if (animator == null)
            animator = GetComponent<Animator>();
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
        StartCoroutine(DestroyFlashEffect());
        // Invoke("InitKnockDownAnimation", 0.5f);
    }
    void InitKnockDownAnimation()
    {
        Instantiate(KnockDownAni, transform.position, Quaternion.identity);
    }
    public void AttackAnimation(int attackType = 0, IDamageable target = null , int enemyLayer = 0)
    {
        //可改成switch
        //for melee unit
        if (attackType == 0)
        {
            animator.Play("2_Attack_Normal");
        }
        //for archer
        if (attackType == 1)
        {
            if (target != null)
                animator.Play("2_Attack_Bow");
            ChangeToward((target as MonoBehaviour).gameObject.transform.position.x - transform.position.x);
            StartCoroutine(ShootArrow(target, 0.55f , enemyLayer = enemyLayer));
        }
        //for tower
        if (attackType == 2)
        {
            if (target != null)
                StartCoroutine(ShootArrow(target, 0 , enemyLayer = enemyLayer));
        }
        //for mine
        if (attackType == 4)
        {
            animator.Play("2_Attack_Bow");
        }
    }
    IEnumerator ShootArrow(IDamageable target, float delayTime , int enemyLayer = 0)
    {
        yield return new WaitForSeconds(delayTime);
        //var arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        var arrow = objPool.Get(GameManager.Instance.objectPoolData.PreloadGameObjects[arrowTypeNum].gameObject.name, transform.position);
        var arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.target = (target as MonoBehaviour).transform;
        arrowScript.shootPoint = transform;
        arrowScript.enemyLayer = enemyLayer;
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

    SpriteRenderer leftHand = null;
    SpriteRenderer rightHand = null;
    Sprite originalWeaponLeft = null;
    Sprite originalWeaponRight = null;
    Sprite BuildingWeapon;
    Sprite MineWeapon;
    public void SaveOriginalWeapon()
    {
        // 確保 leftHand 和 rightHand 不為 null
        leftHand ??= FindChildByName(gameObject.transform, "L_Weapon")?.GetComponent<SpriteRenderer>();
        rightHand ??= FindChildByName(gameObject.transform, "R_Weapon")?.GetComponent<SpriteRenderer>();

        // 確保 originalWeaponLeft 和 originalWeaponRight 被正確保存
        originalWeaponLeft ??= leftHand?.sprite;
        originalWeaponRight ??= rightHand?.sprite;
    }
    public void MineAction()
    {
        if (MineWeapon == null)
        {
            MineWeapon = Resources.Load<Sprite>("Mine/MineTool");
        }
        // 更新武器顯示
        leftHand.sprite = null;
        rightHand.sprite = MineWeapon;
        AttackAnimation(4); // 攻擊動畫
    }
    public void BuildingSpecialtyAction()
    {
        // 加載 BuildingWeapon 資源
        characterCollider.isTrigger = true;
        if (BuildingWeapon == null)
        {
            BuildingWeapon = Resources.Load<Sprite>("Building/BuildingWeapon");
        }
        // 更新武器顯示

        leftHand.sprite = BuildingWeapon;
        rightHand.sprite = null; // 直接賦值為 null

        AttackAnimation(); // 攻擊動畫
    }
    public void BuildingSpecialtyFinishAction(bool needTrigger = false)
    {
        leftHand.sprite = originalWeaponLeft;
        rightHand.sprite = originalWeaponRight;
        characterCollider.isTrigger = needTrigger;
    }

    Transform FindChildByName(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            Transform found = FindChildByName(child, childName);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }


    #region 名稱相關功能(測試用)
    public bool InitName = true;
    GameObject textObject;
    TextMeshPro textMeshPro;
    public void ShowNameOnStart()
    {
        if (InitName)
        {
            //solution
            textObject = transform.Find("UnitText")?.gameObject;
            if (textObject != null)
                textMeshPro = textObject.GetComponent<TextMeshPro>();
            if (textObject == null)
            {
                textObject = new GameObject("UnitText");
                textObject.transform.SetParent(this.transform);
                textMeshPro = textObject.AddComponent<TextMeshPro>();
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.fontSize = 24;              // 字體大小
                textMeshPro.color = UnityEngine.Color.white;              // 文字顏色
            }

            string formattedName = gameObject.name;
            textMeshPro.text = formattedName;
            textObject.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
            textMeshPro.GetComponent<Renderer>().sortingOrder = 10;
            ChangeNameTagDirection(-1);
            EnterObjName();
        }
    }

    private void EnterObjName()
    {
        if (textObject == null)
            return;
        //gameObject.name = GameManager.Instance.unitNumber.ToString();
        textMeshPro.text = gameObject.name;
    }

    public void ChangeNameTagDirection(float towardDiretion)
    {
        if (textObject == null)
            return;
        if (towardDiretion < 0)
        {
            Vector3 scale = textObject.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(1);
            textObject.transform.localScale = scale;
            textObject.transform.localPosition = new Vector3(-0.1f, 1, 0);
        }
        else if (towardDiretion > 0)
        {
            Vector3 scale = textObject.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(-1);
            textObject.transform.localScale = scale;
            textObject.transform.localPosition = new Vector3(-0.1f, 1, 0);
        }
    }
    #endregion
}


