using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using UnityEngine.Serialization;

public class GameData : MonoBehaviour
{
    public float offset;
    public AudioClip musicClip;
    public float clickAccuracy = 0.3f;
    public float clickDetection = 0.5f;
    public int cubeCount = 4;
    public float health = 1;

    public bool autoNote = true;
    public List<float> noteTimings = new List<float>();

    // Custom Editor for GameData
    // [CustomEditor(typeof(GameData))]
    // public class GameDataEditor : Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         // Get the target object
    //         GameData gameData = (GameData)target;
    //
    //         // Update the serialized object
    //         serializedObject.Update();
    //
    //         // Draw each field except noteTimings manually
    //         EditorGUILayout.PropertyField(serializedObject.FindProperty("offset"));
    //         EditorGUILayout.PropertyField(serializedObject.FindProperty("musicClip"));
    //         EditorGUILayout.PropertyField(serializedObject.FindProperty("clickAccuracy"));
    //         EditorGUILayout.PropertyField(serializedObject.FindProperty("clickDetection"));
    //         EditorGUILayout.PropertyField(serializedObject.FindProperty("cubeCount"));
    //         EditorGUILayout.PropertyField(serializedObject.FindProperty("health"));
    //
    //         // Draw the autoNote field
    //         gameData.autoNote = EditorGUILayout.Toggle("Auto Note", gameData.autoNote);
    //
    //         // Conditionally draw the noteTimings field
    //         if (!gameData.autoNote)
    //         {
    //             EditorGUILayout.PropertyField(serializedObject.FindProperty("noteTimings"), true);
    //         }
    //
    //         // Apply changes to the serialized properties
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
}