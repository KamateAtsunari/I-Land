using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    //ドロップするアイテムの種類と数
    [SerializeField] DropItemData dropItemData = null;

    public DropItemData GetDropItemData() {
        return dropItemData; 
    }
}
