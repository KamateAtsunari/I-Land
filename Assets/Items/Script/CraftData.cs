using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create CraftData")]
public class CraftData : ScriptableObject
{
    [SerializeField] private string CraftName = null;
    //アイテムの画像
    [SerializeField] private Sprite icon = null;
    //クラフトで生成されるアイテム
    [SerializeField] private ItemData product = default;
    //必要なアイテムの種類
    [SerializeField] private List<ItemData> necessaryItem = new List<ItemData>();
    //必要なアイテムの個数
    [SerializeField] private List<int> necessaryNum = new List<int>();
    //説明欄に表示するテキスト
    [SerializeField] private List<string> necessaryText = new List<string>();

    public Sprite GetIcon() { return icon; }
    public string GetName() { return CraftName;  }
    public List<ItemData> GetItemList() { return necessaryItem; }
    public List<int> GetNecessaryNum(){ return necessaryNum; }
    public ItemData GetProduct() { return product; }
    public List<string> GetTextList() {return necessaryText;}
}