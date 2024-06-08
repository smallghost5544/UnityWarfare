using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacter : MonoBehaviour
{
    public Color color = Color.white;
    public List<SpriteRenderer> changeList = new List<SpriteRenderer>();
    public void ChangeColor()
    {
        changeList.Clear();
        changeList.Add(GameObject.Find("BodyArmor")?.GetComponent<SpriteRenderer>());
        changeList.Add(GameObject.Find("11_Helmet1")?.GetComponent<SpriteRenderer>());
        changeList.Add(GameObject.Find("-19_RCArm")?.GetComponent<SpriteRenderer>());
        changeList.Add(GameObject.Find("21_LCArm")?.GetComponent<SpriteRenderer>());
        changeList.Add(GameObject.Find("25_L_Shoulder")?.GetComponent<SpriteRenderer>());
        changeList.Add(GameObject.Find("-15_R_Shoulder")?.GetComponent<SpriteRenderer>());
        changeList.Add(GameObject.Find("_11R_Cloth")?.GetComponent<SpriteRenderer>());
        changeList.Add(GameObject.Find("_2L_Cloth")?.GetComponent<SpriteRenderer>());

        foreach (SpriteRenderer sprite in changeList)
        {
            sprite.color = color;
        }
        //var armor = GameObject.Find("BodyArmor")?.GetComponent<SpriteRenderer>();
        //var helmet = GameObject.Find("11_Helmet1")?.GetComponent<SpriteRenderer>();
        //var rightHand = GameObject.Find("-19_RCArm")?.GetComponent<SpriteRenderer>();
        //var leftHand = GameObject.Find("21_LCArm")?.GetComponent<SpriteRenderer>();
        //var leftShoulder = GameObject.Find("25_L_Shoulder")?.GetComponent<SpriteRenderer>();
        //var rightShoulder = GameObject.Find("-15_R_Shoulder")?.GetComponent<SpriteRenderer>();
        //var leftShoe = GameObject.Find("-15_R_Shoulder")?.GetComponent<SpriteRenderer>();
        //var rightShoe = GameObject.Find("_2L_Cloth")?.GetComponent<SpriteRenderer>();


    }

}
