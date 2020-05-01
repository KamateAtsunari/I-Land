using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemDataBase/Tool")]
public class ToolDataBase : ScriptableObject
{
    [SerializeField]
    private List<ToolData> toolLists = new List<ToolData>();

    public List<ToolData> GetItemDataList(){ return toolLists; }
}
