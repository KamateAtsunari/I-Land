using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemData/Armor")]
public class ArmorData : ItemData
{
    private enum ArmorType
    {
        Armor,
        Glove,
        Leggings
    }

    [SerializeField]
    private int itemDefense = 0;
    [SerializeField]
    private float endurance = 0;
    [SerializeField]
    private ArmorType armorType = default;
    
    public int GetItemDefense() { return itemDefense; }
    public float GetEndurance() { return endurance; }
    public int GetArmorType() { return (int)armorType; }
}
