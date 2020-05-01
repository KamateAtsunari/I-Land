using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : CreateSlot
{
    //[SerializeField]
    //private MyStatus myStatus = default;

    [SerializeField] private ItemDataBase itemDataBase = null;

    private int freeNum;
    private List<ItemData> itemList;
    private List<GameObject> slotList = new List<GameObject>();
    InventorySlot[] inventorySlots;

    public void Awake()
    {
        inventorySlots = new InventorySlot[slotNum];
        itemList = itemDataBase.GetItemDataList();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            CreateSlotList(slotList, i);
            inventorySlots[i] = slotList[i].GetComponent<InventorySlot>();
        }
        //AddInventory("粗悪な斧", 1);
        //AddInventory("鉄剣", 1);
        //AddInventory("枝", 4);
        //AddInventory("鉄鉱石", 60);
        //AddInventory("銅の斧", 1);
        //AddInventory("粗悪なつるはし", 1);
        //AddInventory("鉄の足具", 1);
        //AddInventory("薬草", 20);
        //AddInventory("りんご", 5);
        //AddInventory("革の鎧", 1);

    }
    public void SetDisplayData(ItemData itemData,int count,int elemNum)
    {
        base.SetDisplayData(itemData,elemNum);
        //スタックが可能な場合アイテムの個数を表示
        if (itemData.GetIsStack())
        {
            transform.GetChild(elemNum).GetChild(1).GetComponent<Text>().text = count.ToString();
        }
        
    }
    //アイテムをインベントリに追加する
    public void AddInventory(string itemName, int count)
    {
        //アイテムのデータベースの中から対象のアイテムを検索する
        foreach (var item in itemList)
        {
            //Debug.Log("cheak");
            if (itemName == item.GetItemName())
            {
                //インベントリスロットが空の位置を検索する
                while (!inventorySlots[freeNum].IsEmpty())
                {
                    freeNum++;
                }
                //インベントリ内に同じアイテムがある場合その位置にアイテムを追加する
                if (SearchInventoryItem(item))
                {
                    //アイテムの個数の加算
                    inventorySlots[freeNum].CalcItemCount(count);
                }
                //同じアイテムがない場合は空のスロットに追加する
                else
                {
                    inventorySlots[freeNum].SetSlotData(item, count);
                }

                SetDisplayData(item, inventorySlots[freeNum].GetItemCount(), freeNum);
            }
        }
    }
    //インベントリからアイテム個数を減らす
    public void ReduceInventory(int elemNum, int count)
    {
        inventorySlots[elemNum].CalcItemCount(-count);

        SetDisplayData(inventorySlots[elemNum].GetItem(),inventorySlots[elemNum].GetItemCount(), elemNum);

        if (inventorySlots[elemNum].GetItemCount() == 0)
        {
            inventorySlots[elemNum].SlotClear();
            ResetDisplayData(elemNum);

            if (elemNum < freeNum)
            {
                freeNum = elemNum;
            }
        }
    }
    //アイテムの検索
    public bool SearchInventoryItem(ItemData item)
    {
        //インベントリ内の検索
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //インベントリ内にアイテムがあってかつ、そのアイテムがスタック可能の場合trueを返す
            if (inventorySlots[i].GetItem() == item && inventorySlots[i].GetItem().GetIsStack())
            {
                freeNum = i;
                return true;
            }
        }
        return false;
    }
    //アイテムの削除
    public void RemoveItem(ItemData item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].GetItem() == item)
            {
                inventorySlots[i].SlotClear();
                ResetDisplayData(i);
                return;
            }
        }
    }

    public int GetInventorySize()
    {
        return inventorySlots.Length;
    }

    public ItemData GetInventoryItem(int elemNum)
    {
        return inventorySlots[elemNum].GetItem();
    }
    public int GetInventoryItemQte(int elemNum)
    {
        return inventorySlots[elemNum].GetItemCount();
    }


    public int GetfreeNum()
    {
        return freeNum;
    }
}
