using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TraitBasedOpinionSystem.Unity;

namespace TraitBasedOpinionSystem.UnityEditor
{
    public enum ModifierType
    {
        Automatic,
        HasTag
    };

    [CustomEditor(typeof(TraitTypeScriptableObject)), CanEditMultipleObjects]
    public class TraitTypeScriptableObjectEditor : Editor
    {
        public SerializedProperty
            traitName,
            description;

        public ModifierType _modifierType = ModifierType.HasTag;

        void OnEnable()
        {
            traitName = serializedObject.FindProperty("traitName");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            switch(_modifierType)
            {
                case ModifierType.HasTag:
                    EditorGUILayout.PropertyField(traitName, new GUIContent("HasTag"));
                    break;
                case ModifierType.Automatic:
                    EditorGUILayout.PropertyField(traitName, new GUIContent("Automatic"));
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
