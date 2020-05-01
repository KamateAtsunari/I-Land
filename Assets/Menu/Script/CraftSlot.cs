using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSlot : MonoBehaviour
{
    private CraftData craftData;
    private InspectorManager inspectorManager;
    public void onClickAct()
    {
        inspectorManager.SetDisplayCraftData(craftData);
        //transform.parent.GetComponent<CraftManager>().CraftItem(craftData);  
    }
    public void SetCraftData(CraftData craft,InspectorManager inspector)
    {
        craftData = craft;
        inspectorManager = inspector;
    }
}
