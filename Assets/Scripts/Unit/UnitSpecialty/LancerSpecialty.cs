using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerSpecialty : MonoBehaviour , ISpecialty
{
    GameObject smallArrowTower;
    public void SetReSource()
    {
        if (smallArrowTower == null)
        {
            smallArrowTower = Resources.Load<GameObject>("Building/ArrowTower");
        }
    }
    public void DoSpecialize()
    {
        //找到位置
        //蓋建築
        //fix 效能改善
        SetReSource();
        Instantiate(smallArrowTower,gameObject.transform.position , Quaternion.identity );
    }

}
