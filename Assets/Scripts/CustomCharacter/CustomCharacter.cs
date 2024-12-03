using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomCharacter : MonoBehaviour
{
    public Color color = Color.white;
    public List<SpriteRenderer> changeList = new List<SpriteRenderer>();
    public bool ChangeWeaponColor = false;
    public bool ChangeBackColor = false;
    Transform unitRoot;
    public void GetSpriteList()
    {
        unitRoot = transform.Find("UnitRoot");
        changeList.Clear();
        //feet
        changeList.Add(FindChildByName(gameObject.transform, "_12R_Foot", 6, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "_11R_Cloth", 6, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "_3L_Foot", 6, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "_2L_Cloth", 6, 0)?.gameObject.GetComponent<SpriteRenderer>());

        //left arm
        if (ChangeWeaponColor)
        {
            changeList.Add(FindChildByName(gameObject.transform, "L_Shield", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
            changeList.Add(FindChildByName(gameObject.transform, "L_Weapon", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        }
        changeList.Add(FindChildByName(gameObject.transform, "25_L_Shoulder", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "21_LCArm", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "20_L_Arm", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());

        //right arm
        if (ChangeWeaponColor)
        {
            changeList.Add(FindChildByName(gameObject.transform, "R_Shield", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
            changeList.Add(FindChildByName(gameObject.transform, "R_Weapon", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        }
        changeList.Add(FindChildByName(gameObject.transform, "-15_R_Shoulder", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "-19_RCArm", 11 ,0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "-20_R_Arm", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());

        //head
        changeList.Add(FindChildByName(gameObject.transform, "12_Helmet2 ", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "11_Helmet1", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "BodyArmor", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());
        changeList.Add(FindChildByName(gameObject.transform, "ClothBody", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());

        if (ChangeBackColor)
            changeList.Add(FindChildByName(gameObject.transform, "BackBack", 11, 0)?.gameObject.GetComponent<SpriteRenderer>());

        //changeList.Add(transform.Find("P_RFoot")?.GetComponentInChildren<SpriteRenderer>());
        //changeList.Add(transform.Find("P_LFoot")?.GetComponentInChildren<SpriteRenderer>());
        //var bodys = transform.Find("Body")?.GetComponentsInChildren<SpriteRenderer>();
        //foreach (var body in bodys)
        //    changeList.Add(body);
        //changeList.Add(transform.Find("P_Helmet")?.GetComponentInChildren<SpriteRenderer>());
        //var Larms = transform.Find("P_Arm")?.GetComponentsInChildren<SpriteRenderer>();
        //foreach (var arm in Larms)
        //    changeList.Add(arm);

        //var Rarms = transform.Find("P_RArm")?.GetComponentsInChildren<SpriteRenderer>();
        //changeList.Add(transform.Find("P_RArm")?.GetComponentInChildren<SpriteRenderer>());
        //foreach (var arm in Rarms)
        //    changeList.Add(arm);
        //changeList.Remove(transform.Find("P_Weapon")?.GetComponentInChildren<SpriteRenderer>());
        //changeList.Remove(transform.Find("ArmL")?.GetComponentInChildren<SpriteRenderer>());
        //changeList.Remove(transform.Find("ArmR")?.GetComponentInChildren<SpriteRenderer>());
    }
    public void ChangeColor()
    {

        foreach (SpriteRenderer sprite in changeList)
        {
            sprite.color = color;
        }
    }

    Transform FindChildByName(Transform parent, string name, int maxDepth, int currentDepth)
    {
        if (currentDepth > maxDepth) return null;
        print(currentDepth); ;
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                print(child);
                return child;
            }

            // 遞迴查找子物件
            Transform result = FindChildByName(child, name, maxDepth, currentDepth + 1);
            if (result != null) return result;
        }

        return null;
    }

}
