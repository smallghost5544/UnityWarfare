using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform shootPoint;    // 射出點
    public Transform target;        // 目標點
    public float gravity = 9.8f;    // 重力加速度
    public float shootTime = 2f;    // 投射時間
    bool arriveTarget = false;
    public float maxHeight = 5f;
    public float exitTime = 1.5f;
    public bool CustomizedHeight = false;
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
            // 使箭矢沿著運動方向旋轉
            float angle = Mathf.Atan2(initialVelocity.y - gravity * timeElapsed, initialVelocity.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (timeElapsed >= shootTime)
            {
                arriveTarget = true;
                yield return new WaitForSeconds(exitTime);
                Destroy(arrow);
            }
            yield return null;
        }
    }
}
