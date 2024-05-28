using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if EXCLUDE_EXAMPLESCRIPT
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
