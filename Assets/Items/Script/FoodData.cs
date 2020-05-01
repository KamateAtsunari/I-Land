using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemData/Food")]
public class FoodData : ItemData
{
    [SerializeField]
    private int satiety = 0;

    public int GetSatiety() { return satiety; }
}
