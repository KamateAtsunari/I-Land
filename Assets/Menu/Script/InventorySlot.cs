using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,IBeginDragHandler, IDragHandler,IDropHandler,IEndDragHandler
{
    //アイテムの画像
    [SerializeField] private GameObject itemImageObj = null;

    private InspectorManager inspectorManager;

    public Hand hand;

    protected ItemData itemData;
    protected int itemCount;
    //private int slotOrder;
    private GameObject draggingObj;
    private Transform canvasTransform;

    
    

    public void Start()
    {
        canvasTransform = transform.parent.parent.parent;
        hand = FindObjectOfType<Hand>();
        
    }

    public void SetSlotData(ItemData item,int count)
    {
        itemData = item;
        itemCount = count;
        
    }
    
    public void SlotClear()
    {
        itemData = null;
        itemCount = 0;
    }
    public bool IsEmpty()
    {
        if (itemData == null)
            return true;

        return false;
    }

    public ItemData GetItem()
    {
        return itemData;
    }

    public int CalcItemCount(int count)
    {
        //Debug.Log(itemCount);
        itemCount += count;

        return itemCount;
    }
    public int GetItemCount()
    {
        return itemCount;
    }
    public void SetInventoryImage(ItemData item, int count)
    {
        if (item != null)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = item.GetIcon();
            //if(transform.childCount == 2)
            if (item.GetIsStack())
            {
                transform.GetChild(1).GetComponent<Text>().text = count.ToString();
            }
            else
            {
                transform.GetChild(1).GetComponent<Text>().text = null;
            }
            
        }
        else 
        {
            transform.GetChild(0).GetComponent<Image>().sprite = null;
            //if (transform.childCount == 2)
            transform.GetChild(1).GetComponent<Text>().text = null;
        }
            
    }
    public void onClickAct()
    {
        inspectorManager = GameObject.Find("Inspector").GetComponent<InspectorManager>();
        inspectorManager.SetDisplayItemData(itemData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemData == null)  return; 
        //アイテムのイメージを複製
        draggingObj = Instantiate(itemImageObj, canvasTransform);
        //複製を最前面に配置
        draggingObj.transform.SetAsLastSibling();
        //複製元の色を暗くする
        transform.GetChild(0).GetComponent<Image>().color = Color.gray;
        //Handにアイテムを渡す
        hand.SetGrabbingItem(itemData,itemCount);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //nullチェック
        if(hand == null) { return; }
        if(draggingObj == null) { return; }
        //画像がマウスに追従するようにする
        draggingObj.transform.position = hand.transform.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        //nullcheck
        if (!hand.isHavingItem()) return;

        //アイテムの受け取り
        ItemData item = hand.GetGrabbingItem();
        int count = hand.GetGrabbingItemCount();

        //アイテムをhandに渡す
        hand.SetGrabbingItem(itemData, itemCount);

        SetSlotData(item, count);
        SetInventoryImage(item, count);

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
       
        Destroy(draggingObj);

        ItemData item = hand.GetGrabbingItem();
        int count = hand.GetGrabbingItemCount();

        //ArmorData armorData = item as ArmorData;
        //if (armorData != null) return;

        SetSlotData(item, count);
        SetInventoryImage(item, count);
        transform.GetChild(0).GetComponent<Image>().color = Color.white;
    }
}
