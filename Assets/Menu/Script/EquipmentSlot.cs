using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : InventorySlot
{
    //プレイヤーのステータス
    private MyStatus myStatus = null;
    //防具の情報
    private ArmorData armorData;
    //防具の種類
    private int armorType = 0;

    public override void OnDrop(PointerEventData eventData)
    {
        //nullcheck
        if (!hand.isHavingItem()) return;

        //アイテムの受け取り
        ItemData item = hand.GetGrabbingItem();
        int count = hand.GetGrabbingItemCount();

        //移動しているアイテムがアイテムスロットに対応したアイテムじゃない場合は元の位置に戻す
        armorData = item as ArmorData;
        if (armorData == null||armorType!=armorData.GetArmorType())
        {
            hand.SetGrabbingItem(item, count);
        }
        //移動しているアイテムがアイテムスロットに対応したアイテムの場合は装備する
        else
        {
            //アイテムをアイテムスロットに渡す
            hand.SetGrabbingItem(itemData, itemCount);
            //アイテムスロットにアイテムを反映
            SetSlotData(item, count);
            SetInventoryImage(item, count);
            //アイテムの防御力分プレイヤーのステータスを増加
            myStatus.ChangeDefense(armorData.GetItemDefense(),armorType);
        }
    }
    //装備を外す
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        //アイテムの防御力をプレイヤーのステータスを減少
        myStatus.ChangeDefense(0,armorType);
    }

    public void SetArmorType(int type,MyStatus mStatus)
    {
        armorType = type;
        myStatus = mStatus;
    }
}
