using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    GUN,
    LIGHT
}

public abstract class EquipmentSO : ScriptableObject
{
    public EquipmentType equipType;
    public Sprite icon;
}
