using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UGUIExButtonToggle), true)]
    [CanEditMultipleObjects]
    public class UGUIExButtonToggleEditor : SelectableEditor
    {
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_TransitionProperty;
        SerializedProperty m_GraphicProperty;
        SerializedProperty m_GroupProperty;
        SerializedProperty m_IsOnProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_TransitionProperty = serializedObject.FindProperty("m_ToggleTransition");
            m_GraphicProperty = serializedObject.FindProperty("m_SelectImage");
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick"); ;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            UGUIExButtonToggle _Button = serializedObject.targetObject as UGUIExButtonToggle;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_IsOnProperty);
            _Button.ReSize();
            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(_Button.gameObject.scene);
                UGUIExButtonToggleGroup group = m_GroupProperty.objectReferenceValue as UGUIExButtonToggleGroup;

                _Button.IsOn = m_IsOnProperty.boolValue;

                if (group != null && _Button.IsActive())
                {
                    if (_Button.IsOn || (!group.AnyTogglesOn() && !group.AllowSwitchOff))
                    {
                        _Button.IsOn = true;
                        group.NotifyToggleOn(_Button);
                    }
                }
            }
            EditorGUILayout.PropertyField(m_TransitionProperty);
            EditorGUILayout.PropertyField(m_GraphicProperty);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_GroupProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(_Button.gameObject.scene);
                UGUIExButtonToggleGroup group = m_GroupProperty.objectReferenceValue as UGUIExButtonToggleGroup;
                _Button.Group = group;
            }

            EditorGUILayout.Space();

            // Draw the event notification options
            EditorGUILayout.PropertyField(m_OnClickProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}