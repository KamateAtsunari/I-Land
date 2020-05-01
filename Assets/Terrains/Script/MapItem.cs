using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : DropItemManager
{
    //マップにあるアイテムの耐久力
    [SerializeField] int durability = 0;
    //対応するツール
    [SerializeField] List<ToolData> supportToolList = null;

    public bool ReduceDurability(ToolData toolData)
    {
        //配列内の検索（-１の場合は配列内に存在しない）
        int index = supportToolList.IndexOf(toolData);
        //配列内にあった場合
        if (index != -1)
        {
            //Debug.Log("cheak");
            //耐久力を減少させる
            durability -= index+1;
            if (durability <= 0)
            {
                return true;
            }
           
        }
        return false;
    }
}
