using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(CustomMap))]
public class CustomInspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CustomMap yourScript = (CustomMap)target;

        if (GUILayout.Button("SetLayer"))
        {
            yourScript.SetSpriteLayer();
        }
        if (GUILayout.Button("SetBlockRange"))
        {
            yourScript.SetBlockRange();
        }
    }

}
#endif
