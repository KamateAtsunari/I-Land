using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemData/Item")]
public class ItemData : ScriptableObject
{
    //アイテムの名前
    [SerializeField] private string itemName = null;
    //アイテムのアイコン
    [SerializeField] private Sprite icon = null;
    //説明文
    [SerializeField] private List<string> Description = null;
    //スタック可能かどうか
    [SerializeField] private bool isStack = false;

    public Sprite GetIcon() { return icon; }
    public string GetItemName() { return itemName; }
    public List<string> GetDescription() { return Description; }
    public bool GetIsStack() { return isStack; }
}