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
        //����m
        //�\�ؿv
        //fix �į�ﵽ
        SetReSource();
        Instantiate(smallArrowTower,gameObject.transform.position , Quaternion.identity );
    }

}
