using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacter : MonoBehaviour
{
    public Color color = Color.white;
    public List<SpriteRenderer> changeList = new List<SpriteRenderer>();
    public void GetSpriteList()
    {
        changeList.Clear();
        changeList.Add(GameObject.Find("P_RFoot")?.GetComponentInChildren<SpriteRenderer>());
        changeList.Add(GameObject.Find("P_LFoot")?.GetComponentInChildren<SpriteRenderer>());
        var bodys = GameObject.Find("Body")?.GetComponentsInChildren<SpriteRenderer>();
        foreach (var body in bodys)
            changeList.Add(body);
        changeList.Add(GameObject.Find("P_Helmet")?.GetComponentInChildren<SpriteRenderer>());
        var Larms = GameObject.Find("P_Arm")?.GetComponentsInChildren<SpriteRenderer>();
        foreach (var arm in Larms)
            changeList.Add(arm);

        var Rarms = GameObject.Find("P_RArm")?.GetComponentsInChildren<SpriteRenderer>();
        changeList.Add(GameObject.Find("P_RArm")?.GetComponentInChildren<SpriteRenderer>());
        foreach (var arm in Rarms)
            changeList.Add(arm);
        changeList.Remove(GameObject.Find("P_Weapon")?.GetComponentInChildren<SpriteRenderer>());
        changeList.Remove(GameObject.Find("ArmL")?.GetComponentInChildren<SpriteRenderer>());
        changeList.Remove(GameObject.Find("ArmR")?.GetComponentInChildren<SpriteRenderer>());
    }
    public void ChangeColor()
    {
        //changeList.Clear();
        //changeList.Add(GameObject.Find("P_RFoot")?.GetComponentInChildren<SpriteRenderer>());
        //changeList.Add(GameObject.Find("P_LFoot")?.GetComponentInChildren<SpriteRenderer>());
        //var bodys = GameObject.Find("Body")?.GetComponentsInChildren<SpriteRenderer>();
        //foreach (var body in bodys)
        //    changeList.Add(body);
        //changeList.Add(GameObject.Find("P_Helmet")?.GetComponentInChildren<SpriteRenderer>());
        //var Larms = GameObject.Find("P_Arm")?.GetComponentsInChildren<SpriteRenderer>();
        //foreach (var arm in Larms)
        //    changeList.Add(arm);

        //var Rarms = GameObject.Find("P_RArm")?.GetComponentsInChildren<SpriteRenderer>();
        //changeList.Add(GameObject.Find("P_RArm")?.GetComponentInChildren<SpriteRenderer>());
        //foreach (var arm in Rarms)
        //    changeList.Add(arm);
        //changeList.Remove(GameObject.Find("P_Weapon")?.GetComponentInChildren<SpriteRenderer>());
        //changeList.Remove(GameObject.Find("ArmL")?.GetComponentInChildren<SpriteRenderer>());
        //changeList.Remove(GameObject.Find("ArmR")?.GetComponentInChildren<SpriteRenderer>());

        ////changeList.Add(GameObject.Find("BodyArmor")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("ClothBody")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("11_Helmet1")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("-19_RCArm")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("21_LCArm")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("25_L_Shoulder")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("-15_R_Shoulder")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("_11R_Cloth")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("_2L_Cloth")?.GetComponent<SpriteRenderer>());
        ////changeList.Add(GameObject.Find("Back")?.GetComponent<SpriteRenderer>());
        foreach (SpriteRenderer sprite in changeList)
        {
            sprite.color = color;
        }
    }

}
