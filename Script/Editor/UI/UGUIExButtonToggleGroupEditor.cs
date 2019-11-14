using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
namespace UnityEditor.UI
{
    [CustomEditor(typeof(UGUIExButtonToggleGroup), true)]
    [CanEditMultipleObjects]
    public class UGUIExButtonToggleGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            UGUIExButtonToggleGroup _Group = serializedObject.targetObject as UGUIExButtonToggleGroup;
         
            _Group.GroupResize();

            serializedObject.ApplyModifiedProperties();
        }
    }
}