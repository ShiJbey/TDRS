using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class OpinionActorEditorWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private void OnEnable()
    {
        titleContent = new GUIContent("Opinion System Actor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }

    [MenuItem("Window/Opinion Agent Window")]
    public static void ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(OpinionActorEditorWindow));
    }
}
