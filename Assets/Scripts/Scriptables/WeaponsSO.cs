using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using Unity.VisualScripting;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum BulletKeys
{
    Chain,
    BigBertha,
    MultiZap
}

public enum BulletTypes
{
    Classic,
    Explosive,
    Multiple
}

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Scriptables/WeaponStats", order = 2)]
public class WeaponsSO: EquipmentSO
{
    public int damage;
    public float bulletSpeed;
    public float reloadTime;
    [Range(0, 45f)]
    public float bulletSpread;
    public BulletKeys key;
    public BulletTypes type;
    public FlashLightSO flashLight;
    public GameObject particles;

    [HideInInspector]
    public float AOE_Range;

    [HideInInspector] 
    public int bulletNumber;
}

#if UNITY_EDITOR
[CustomEditor(typeof(WeaponsSO))]
public class RandomScript_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields
 
        WeaponsSO script = (WeaponsSO)target;

        
        if (script.type == BulletTypes.Explosive)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("AOE Range");
            script.AOE_Range = EditorGUILayout.Slider(script.AOE_Range, 0, 100);
            EditorGUILayout.EndHorizontal();
        } else if (script.type == BulletTypes.Multiple)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Number of bullets");
            script.bulletNumber = EditorGUILayout.IntSlider(script.bulletNumber, 0, 8);
            EditorGUILayout.EndHorizontal();
        }
        
    }
}
#endif
