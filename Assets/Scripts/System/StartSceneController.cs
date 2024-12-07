using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum UnitColor
{
    Red = 6,
    Blue = 7,
    Green = 11,
}

public class StartSceneController : MonoBehaviour
{
    public static StartSceneController Instance;
    public UnitColor color = UnitColor.Blue;
    private void Awake()
    {
        // 確保物件唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持該物件在場景切換時不銷毀
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ChooseColor(string colorString)
    {
        if (Enum.TryParse(colorString, out UnitColor playerChooseColor))
        {
           // GameManager.Instance.PlayerChooseColor = color;
            print(color);
            color = playerChooseColor;
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            print("fail");
        }
    }

}
