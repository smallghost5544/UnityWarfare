#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomCharacter))]
public class CustomInspectorCharacter : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CustomCharacter customCharacter = (CustomCharacter)target;
        if (GUILayout.Button("SetColor"))
        {
            customCharacter.ChangeColor();
        }
        if (GUILayout.Button("GetSpriteList"))
        {
            customCharacter.GetSpriteList();
        }
    }
}
#endif