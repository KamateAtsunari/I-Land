using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortCutManager : CreateSlot
{
    [SerializeField] private MyStatus myStatus = null;
    [SerializeField] private Transform equipmentList = null;
    [SerializeField] private Sprite normalImage = null;
    [SerializeField] private Sprite cursorImage = null;

    private List<GameObject> slotList = new List<GameObject>();
    InventorySlot[] inventorySlots;
    //現在のカーソルの位置
    private int serectCount;
    //ひとつ前のカーソルの位置
    private int oldSerectCount;

    private float wheel;

    public void Start()
    {
        inventorySlots = new InventorySlot[slotNum];
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            CreateSlotList(slotList, i);
            inventorySlots[i] = slotList[i].GetComponent<InventorySlot>();
        }
        oldSerectCount = 1;
    }

    private void Update()
    {
        wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0.1&&serectCount<4) serectCount++;
        if (wheel < -0.1&&serectCount>0) serectCount--;

        if (serectCount != oldSerectCount)
        {
            ChangeEquipment();
            transform.GetChild(serectCount).GetComponent<Image>().sprite = cursorImage;
            transform.GetChild(oldSerectCount).GetComponent<Image>().sprite = normalImage;
        }
        oldSerectCount = serectCount;
    }
    public void SetDisplayData(ItemData itemData, int count, int elemNum)
    {
        base.SetDisplayData(itemData, elemNum);
        //スタックが可能な場合アイテムの個数を表示
        if (itemData.GetIsStack())
        {
            transform.GetChild(elemNum).GetChild(1).GetComponent<Text>().text = count.ToString();
        }

    }
    public void ChangeEquipment()
    {
        ItemData handItem = inventorySlots[serectCount].GetItem();
        myStatus.SetHandItem(handItem);
        //選択されたアイテムが武器・道具だった場合は対応したオブジェクトを表示する
        if (handItem != null)
        {
            ToolData toolData = handItem as ToolData;
            if(toolData)
            {
                equipmentList.GetChild(toolData.GetIId()).gameObject.SetActive(true);
                //ステータスに武器・道具の攻撃力を加算
                myStatus.ChangeAttack(toolData.GetItemAttack());
            }
            //道具・武器以外の場合は攻撃力を初期化する
            else
            {
                myStatus.ChangeAttack(0);
            }
        }//スロットが空の場合は攻撃力を初期化する
        else
        {
            myStatus.ChangeAttack(0);
        }

        //ひとつ前のアイテムが武器・道具だった場合はオブジェクトを非表示にする
        if (inventorySlots[oldSerectCount].GetItem() != null)
        {
            ToolData toolData = inventorySlots[oldSerectCount].GetItem() as ToolData;
            if (toolData != null)
            {
                equipmentList.GetChild(toolData.GetIId()).gameObject.SetActive(false);
            }
        }
    }
    //アイテムの個数減少
    public bool ReduceInventory(int count)
    {
        inventorySlots[serectCount].CalcItemCount(-count);

        SetDisplayData(inventorySlots[serectCount].GetItem(), inventorySlots[serectCount].GetItemCount(), serectCount);

        //アイテムの個数が0になったらスロットを初期化する
        if (inventorySlots[serectCount].GetItemCount() == 0)
        {
            inventorySlots[serectCount].SlotClear();
            ResetDisplayData(serectCount);
            return true;
        }
        return false;
    }
    public void ChangeCursor()
    {
        equipmentList.GetChild(serectCount).gameObject.SetActive(true);
        //for(int i = 0; i < 5; i++)
        //{
        //    if(i == serectCount)
        //        transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(true);
        //    else
        //        transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
        //}

    }
}
