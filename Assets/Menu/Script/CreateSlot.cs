using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateSlot : MonoBehaviour
{
    //スロット
    [SerializeField] protected GameObject slot = null;
    //スロットの数
    [SerializeField] protected int slotNum = 0;

    //アイテムスロットの作成
    public virtual void CreateSlotList(List<GameObject> slotList, int elemNum)
    {
        slotList.Add(Instantiate<GameObject>(slot, transform));
        slotList[elemNum].name = "ItemSlot" + elemNum;
        slotList[elemNum].transform.localScale = new Vector3(1f, 1f, 1f);
    }
    //アイテムスロットにアイテムの情報を入れる
    public virtual void SetDisplayData(ItemData itemData, int elemNum)
    {
        //アイテムのスプライトを設定
        transform.GetChild(elemNum).GetChild(0).GetComponent<Image>().sprite = itemData.GetIcon();
    }
    public virtual void ResetDisplayData(int elemNum)
    {
        transform.GetChild(elemNum).GetChild(0).GetComponent<Image>().sprite = null;
        transform.GetChild(elemNum).GetChild(1).GetComponent<Text>().text = null;
    }
}
