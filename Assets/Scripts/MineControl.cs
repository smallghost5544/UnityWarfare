using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class MineControl : MonoBehaviour, IDamageable
{
    public List<Sprite> MinePics = new List<Sprite>();
    public SpriteRenderer spriteRenderer;
    private Sprite _currentSprite;
    CircleCollider2D collider;
    [SerializeField]
    //private List<(float, float)> colliderFloatPairs = new List<(float, float)>();
    List<float> colliderList = new List<float>();
    private int _currentHp;

    #region public
    public int CurrentHp 
                { get => _currentHp; set => _currentHp = value; }
    public int MaxHp = 100;
    public int MaxMinerCount = 8;
    public int currentMinerCount ;
    public ObjectType ObjType;
    public List<UnitController> MinerList = new List<UnitController>();
    #endregion

    private void OnEnable()
    {
        _currentHp = MaxHp;
        collider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _currentSprite = spriteRenderer.sprite;
        colliderList.Add(0.22f);
        colliderList.Add(0.16f);
        colliderList.Add(0.11f);
        colliderList.Add(0.06f);

        //colliderFloatPairs.Add((0.5f, 0.55f));
        //colliderFloatPairs.Add((0.35f, 0.4f));
        //colliderFloatPairs.Add((0.25f, 0.3f));
        //colliderFloatPairs.Add((0.15f, 0.15f));
    }

    void CheckHpAndChangeSprite()
    {
        if (CurrentHp > MaxHp * 0.75f)
        {
            ChangeMinePhase(0, 8);
        }
        else if (CurrentHp > MaxHp * 0.5f)
        {
            ChangeMinePhase(1, 6);
        }
        else if (CurrentHp > MaxHp * 0.25f)
        {
            ChangeMinePhase(2, 4);
        }
        else if (CurrentHp > MaxHp * 0.1f)
        {
            ChangeMinePhase(3, 3);
        }
        else if( CurrentHp <=0)
        {
            MinerList.Clear();
            gameObject.SetActive(false);
        }

        if (spriteRenderer.sprite != _currentSprite)
        {
            _currentSprite = spriteRenderer.sprite;
            StartCoroutine(FlashEffect());
        }
    }

    void ChangeMinePhase(int phase, int maxMinerCount)
    {
        spriteRenderer.sprite = MinePics[phase];
        //var (width, height) = colliderFloatPairs[phase];
        collider.radius = colliderList[phase];
        //fix ���@��
        MaxMinerCount = maxMinerCount;
        CheckMinerCount();
    }

    void CheckMinerCount()
    {
        if (currentMinerCount > MaxMinerCount)
        {
            print("check");
            currentMinerCount--;
            var lastOne = MinerList.Count - 1;
            MinerList[lastOne].cancelMine();
            MinerList.RemoveAt(lastOne);
        }
    }

    //void CheckMinerAlive()
    //{
    //    for (int i = 0; i < MinerList.Count; i++)
    //    {
    //        if (MinerList[i].gameObject.activeSelf == false)
    //        {
    //              MinerList.RemoveAt(i);    
    //        }
    //    }
    //}

    public void AddMinerInSpace(UnitController unit)
    {
        if (MaxMinerCount > currentMinerCount)
        {
            currentMinerCount++;
            MinerList.Add(unit);
        }
        else
            unit.cancelMine();
    }


    public void GetHurt(int damage)
    {
        CurrentHp -= damage;
        GiveResource();
        if (!inFx)
        {
            inFx = true;
            StartCoroutine(MiningScaleFX());
        }
        CheckHpAndChangeSprite();
    }

    public void GiveResource()
    {
        //�ƥX�����Ϥ����W��
        //�o�쥿�b���q����H
        //�̾ڶˮ`�q�ഫ������
        //�i�H�ǥ�gamemanager�����}�窺�ؼ�
    }

    private float originalAlpha = 1f;
    float fadeDuration = 0.6f;
    float changeAlpha = 0.5f;
    bool onHitRecover = false;
    //�����p�ݭn���n�}�_
    private IEnumerator FlashEffect()
    {
        if (onHitRecover == true)
            yield break;
        onHitRecover = true;
        // 1.�]�m�z���׬��ؼЭ�
        Color color = spriteRenderer.color;
        color.a = changeAlpha;
        spriteRenderer.color = color;
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
            color = spriteRenderer.color;
            color.a = Mathf.Lerp(currentAlpha, originalAlpha, elapsedTime / fadeDuration);
            spriteRenderer.color = color;
            // ���ݤU�@�V
            yield return null;
        }

        color = spriteRenderer.color;
        color.a = originalAlpha;
        spriteRenderer.color = color;

        onHitRecover = false;
    }

    public ObjectType GetObjectType()
    {
        return ObjType;
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
