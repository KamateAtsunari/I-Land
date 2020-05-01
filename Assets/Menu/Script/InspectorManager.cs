using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InspectorManager : MonoBehaviour
{
    [SerializeField] private CraftManager craftMgr = null;
    [SerializeField] private InventoryManager inventoryMgr = null;
    //アイテムの名前
    [SerializeField] private Text itemName = null;
    //アイテムの画像
    [SerializeField] private Image icon = null;
    //説明欄
    [SerializeField] private Text discription= null;
    //アイテムクラフトボタン
    [SerializeField] private GameObject craftButton = null;
    //アイテム削除ボタン
    [SerializeField] private GameObject deleteButton = null;

    private CraftData craftData;
    private ItemData itemData;
    //オブジェクトが非表示になったとき
    private void OnDisable()
    {
        //アイテムの名前を表示
        itemName.text = null;
        //画像を初期化
        icon.sprite = null;
        //説明欄を初期化
        discription.text = null;

        //クラフトボタンが表示されていた場合クラフトボタンを非表示にする
        if (craftButton.activeSelf) { craftButton.SetActive(false); }
        //削除ボタンが表示されていた場合削除ボタンを非表示する
        if (deleteButton.activeSelf) { deleteButton.SetActive(false); }

    }
    //クラフトの情報を表示
    public void SetDisplayCraftData(CraftData craft)
    {
        craftData = craft;
        List<string> textList = craft.GetTextList();
        StringBuilder builder = new StringBuilder();

        //アイテムの名前を表示
        itemName.text = craftData.GetName();

        //アイコンの画像を格納
        icon.sprite = craftData.GetIcon();

        //クラフトの必要素材を説明欄に表示する
        for (int i = 0;i<textList.Count;i++)
        {
            builder.Append(textList[i]);
            builder.Append(Environment.NewLine);
        }
        discription.text = builder.ToString();

        //クラフトボタンが非表示の場合クラフトボタンを表示する
        if (!craftButton.activeSelf) { craftButton.SetActive(true); }
        //削除ボタンが表示されていた場合削除ボタンを非表示する
        if (deleteButton.activeSelf) { deleteButton.SetActive(false); }
    }

    //アイテムの情報を表示
    public void SetDisplayItemData(ItemData item)
    {
        List<string> textList = item.GetDescription();
        StringBuilder builder = new StringBuilder();
        itemData = item;

        //アイテムの名前を表示
        itemName.text = item.GetItemName();

        //削除ボタンが非表示の場合削除ボタンを表示する
        if (!deleteButton.activeSelf) { deleteButton.SetActive(true); }
        //クラフトボタンが表示されていた場合クラフトボタンを非表示にする
        if (craftButton.activeSelf) { craftButton.SetActive(false); }

        //アイコンの画像を格納
        icon.sprite = item.GetIcon();
        //説明欄にアイテムの情報を格納
        for (int i = 0; i < textList.Count; i++)
        {
            builder.Append(textList[i]);
            builder.Append(Environment.NewLine);
        }
        discription.text = builder.ToString();
    }
    //クラフトボタンが押された場合
    public void PushCraftButton()
    {
        craftMgr.CraftItem(craftData);
    }
    //削除ボタンが押された場合
    public void PushDeletButton()
    {
        inventoryMgr.RemoveItem(itemData);
        //アイテムの名前を表示
        itemName.text = null;
        //画像を初期化
        icon.sprite = null;
        //説明欄を初期化
        discription.text = null;

        //クラフトボタンが表示されていた場合クラフトボタンを非表示にする
        if (craftButton.activeSelf) { craftButton.SetActive(false); }
        //削除ボタンが表示されていた場合削除ボタンを非表示する
        if (deleteButton.activeSelf) { deleteButton.SetActive(false); }
    }
}
