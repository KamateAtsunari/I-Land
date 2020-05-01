using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create CraftDataBase")]
public class CraftDataBase : ScriptableObject
{
    [SerializeField]
    private List<CraftData> CraftLists = new List<CraftData>();

    //　アイテムリストを返す
    public List<CraftData> GetCraftDataList()
    {
        return CraftLists;
    }

}