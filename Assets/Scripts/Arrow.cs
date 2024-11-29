
using System.Collections;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    public Transform shootPoint;    // �g�X�I
    public Transform target;        // �ؼ��I
    public float gravity = 9.8f;    // ���O�[�t��
    public float shootTime = 2f;    // ��g�ɶ�
    bool arriveTarget = false;
    public float maxHeight = 5f;
    public float exitTime = 1.5f;
    public bool CustomizedHeight = false;
    public ArrowType arrowType;
    public int arrorDamage = 5;
    public LayerMask enemyLayer;
    public float arrowWidth = 1f;
    public void ShootArrow()
    {
        Vector2 velocity = CalculateInitialVelocity(shootPoint.position, target.position, shootTime, maxHeight);
        StartCoroutine(UpdateArrowPosition(gameObject, velocity));
    }

    Vector2 CalculateInitialVelocity(Vector2 start, Vector2 end, float time, float maxHeight)
    {
        Vector3 distance = end - start;
        float vx = distance.x / time;
        float vy = 0;
        if (CustomizedHeight)
            vy = Mathf.Sqrt(2 * gravity * maxHeight);
        else
            vy = (distance.y + 0.5f * gravity * Mathf.Pow(time, 2)) / time;
        return new Vector2(vx, vy);
    }

    IEnumerator UpdateArrowPosition(GameObject arrow, Vector2 initialVelocity)
    {
        float timeElapsed = 0;
        Vector2 currentPosition = arrow.transform.position;

        while (arriveTarget == false)
        {
            timeElapsed += Time.deltaTime;
            float x = initialVelocity.x * timeElapsed;
            float y = initialVelocity.y * timeElapsed - 0.5f * gravity * timeElapsed * timeElapsed;

            arrow.transform.position = new Vector2(currentPosition.x + x, currentPosition.y + y);
            // �Ͻb�ڪu�۹B�ʤ�V����
            float angle = Mathf.Atan2(initialVelocity.y - gravity * timeElapsed, initialVelocity.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (timeElapsed >= shootTime)
            {
                //�����O�_�����ؼ�
                arriveTarget = true;
                HitEnemyCheck();
                yield return new WaitForSeconds(exitTime);
            }
            yield return null;
        }
        ResetArrow();
        ObjectPool.Instance.Return(arrowType.ToString(), arrow);
    }

    void ResetArrow()
    {
        arriveTarget = false;
    }

    IDamageable Target;
    private void HitEnemyCheck()
    {
        //�|�̾ڼĤ���ҷj�M,���|�H�K�䤤�ߪ���
        Collider2D hitColliders = Physics2D.OverlapCircle(transform.position, arrowWidth, enemyLayer);
        if (hitColliders != null)
        {
            Target = hitColliders.GetComponent<IDamageable>();
            if (Target != null)
                Target.GetHurt(UnityEngine.Random.Range(arrorDamage/2, arrorDamage));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, arrowWidth);
    }


}
