using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : CreateSlot
{
    [SerializeField] private MyStatus myStatus = null;
    //プレイヤーの攻撃力表示用テキスト
    [SerializeField] private Text attackText = null;
    //プレイヤーの防御力表示用テキスト
    [SerializeField] private Text defenseText = null;
    private List<GameObject> slotList = new List<GameObject>();
    
    public void Awake()
    {
        EquipmentSlot[] equipmentSlots = new EquipmentSlot[slotNum];
        //int type = 8;
        for (int i = 0; i < slotNum; i++)
        {
            CreateSlotList(slotList, i);
            equipmentSlots[i] = slotList[i].GetComponent<EquipmentSlot>();
            equipmentSlots[i].SetArmorType(i,myStatus);
            //type++;
        }
       
    }
    private void OnEnable()
    {
        UpdateAttackText(myStatus.GetAttack());
        UpdateDefenseText(myStatus.GetDefense());
    }

    //プレイヤーの攻撃力のテキスト更新
    public void UpdateAttackText(int attack)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("攻撃力：" + attack.ToString());
        attackText.text = builder.ToString();
    }
    //プレイヤーの防御力のテキスト更新
    public void UpdateDefenseText(int defense)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("防御力：" + defense.ToString());
        defenseText.text = builder.ToString();
    }
}
