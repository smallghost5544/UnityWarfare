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
        // �T�O����ߤ@
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �O���Ӫ���b���������ɤ��P��
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
