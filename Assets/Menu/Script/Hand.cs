using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private ItemData grabbingItem;
    private int grabbingItemCount;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Input.mousePosition;
    }

    public ItemData GetGrabbingItem()
    {
        ItemData oldItem = grabbingItem;
        grabbingItem = null;
        return oldItem;
    }
    public int GetGrabbingItemCount()
    {
        int oldCount = grabbingItemCount;
        grabbingItemCount = 0;
        return oldCount;
    }
    public void SetGrabbingItem(ItemData item,int itemCount)
    {
        grabbingItem = item;
        grabbingItemCount = itemCount;
    }

    public bool isHavingItem()
    {
        return grabbingItem != null;
    }
}
