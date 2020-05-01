using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraftManager : CreateSlot
{
    //クラフトに関するデータ
    [SerializeField]
    private CraftDataBase craftDataBase = null;
    //自身のアイテムデータを入れておく
    [SerializeField]
    private InventoryManager inventoryMgr = null;
    //インスペクターのマネージャー
    [SerializeField]
    private InspectorManager inspectorMgr = null;
    //SEマネージャー
    [SerializeField] private SEManager seMgr = null;

    private List<CraftData> craftDataList = new List<CraftData>();
    private List<GameObject> slotList = new List<GameObject>();
    private int pageNum; 

    public void Awake()
    {
        //データベースのデータを個別にリスト化
        craftDataList = craftDataBase.GetCraftDataList();
        //スロットの作成
        for (int i = 0; i < slotNum; i++)
        {
            CreateSlotList(slotList,i);
        }
    }

    public override void CreateSlotList(List<GameObject> slotList, int elemNum)
    {
        base.CreateSlotList(slotList, elemNum);
        //アイテムスロットに画像を追加
        SetDisplayData(craftDataList[elemNum].GetProduct(), elemNum);
        transform.GetChild(elemNum).GetComponent<CraftSlot>().SetCraftData(craftDataList[elemNum],inspectorMgr);
    }

    public void CraftItem(CraftData craftData)
    {
        int requiredCount = 0;
        int successCount = 0;
        List<ItemData> craftItemList = craftData.GetItemList();
        List<int> craftQteList = craftData.GetNecessaryNum();
        //Debug.Log(craftData.GetName());

        List<int> inventoryNumList = new List<int>();

        //クラフトの素材がインベントリにあるかの確認
        foreach (var item in craftItemList)
        {
            for (int i = 0; i < inventoryMgr.GetInventorySize(); i++)
            {
                //Debug.Log(item);
                if (inventoryMgr.GetInventoryItem(i) == item)
                {
                    
                    if (inventoryMgr.GetInventoryItemQte(i) >= craftQteList[successCount])
                    {
                        successCount++;
                        inventoryNumList.Add(i);
                    }
                }

            }
            requiredCount++;
        }
        //クラフトの素材分のアイテムインベントリからを減らす
        if (requiredCount == successCount)
        {
            inventoryMgr.AddInventory(craftData.GetName(), 1);
            //slot.ChangeSlot(inventory.GetfreeNum(), craftData.GetProduct());
            int i = 0;
            foreach (var elemNum in inventoryNumList)
            {
                inventoryMgr.ReduceInventory(elemNum, craftQteList[i]);
                i++;
            }
            seMgr.PlayCraftSE();
            //Debug.Log("クラフト成功");

        }
        //else
        //{
        //    debug.log("クラフト失敗");
        //}
       
    }
    //クラフトのページの切り替え
    public void ClickRightButton()
    {
        //ページの移動制限
        if (pageNum < 2)
        {
            pageNum++;
            for (int i = 0; i < slotNum; i++)
            {
                //アイテムスロットに画像を追加
                SetDisplayData(craftDataList[pageNum * slotNum + i].GetProduct(), i);
                transform.GetChild(i).GetComponent<CraftSlot>().SetCraftData(craftDataList[pageNum * slotNum + i], inspectorMgr);
            }
        }
    }
    //クラフトのページの切り替え
    public void ClickLeftButton()
    {
        //ページの移動制限
        if (pageNum > 0)
        {
            pageNum--;
            for (int i = 0; i < slotNum; i++)
            {
                //アイテムスロットに画像を追加
                SetDisplayData(craftDataList[pageNum * slotNum + i].GetProduct(), i);
                transform.GetChild(i).GetComponent<CraftSlot>().SetCraftData(craftDataList[pageNum * slotNum + i], inspectorMgr);
            }
        }
    }
}