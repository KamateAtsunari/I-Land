using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemData/Medicine")]

public class MedicineData : ItemData
{
    [SerializeField]
    private int healHp = 0;

    public int GetHealHp() { return healHp; }
}
