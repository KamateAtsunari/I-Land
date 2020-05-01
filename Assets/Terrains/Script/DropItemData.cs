using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create DropItem")]
public class DropItemData : ScriptableObject
{
    public List<DropItem> DropItemList = new List<DropItem>();
    public List<DropItem> GetDropItemList(){return DropItemList;}
}


[System.Serializable]
public class DropItem
{
    //設定したいデータの変数
    [SerializeField]
    private string itemName = default;
    [SerializeField]
    private int itemCount = default;

    //public int ItemCount { get => itemCount; set => itemCount = value; }

    public string GetItemName() { return itemName; }
    public int GetItemCount() { return itemCount; }
}
