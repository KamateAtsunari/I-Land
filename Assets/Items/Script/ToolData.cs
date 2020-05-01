using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemData/Tool")]
public class ToolData : ItemData
{
    private enum ToolType
    {
        SWORD,
        SPEAR,
        CLBU,
        PICKAXS,
        AXS,
        BOTTLE
    }

    [SerializeField]
    private int id = 0;
    [SerializeField]
    private int itemAttack = 0;
    [SerializeField]
    private float endurance = 0;
    [SerializeField]
    private ToolType toolType = default;

    public int GetIId() { return id; }
    public int GetItemAttack() { return itemAttack; }
    public float GetEndurance() { return endurance; }
    public int GetToolType() { return (int)toolType; }
}
