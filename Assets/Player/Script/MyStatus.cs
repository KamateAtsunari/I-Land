
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyStatus : MonoBehaviour
{
    //SEマネージャー
    [SerializeField] private SEManager seMgr = null;
    //最大体力
    [SerializeField] private float maxHitPoint = 0;
    //最大スタミナ
    [SerializeField] private float maxStaminaPoint = 0;
    //最大満腹度
    [SerializeField] private float maxSatietyPoint = 0;
    //初期攻撃力
    [SerializeField] private int initAttack = 0;
    //初期防御力
    [SerializeField] private int initDefense = 0;
    //体力バーのマネージャー
    [SerializeField] private SimpleHealthBar hpBar = null;
    //スタミナバーのマネージャー
    [SerializeField] private SimpleHealthBar staminaBar = null;
    //空腹度バーのマネージャー
    [SerializeField] private SimpleHealthBar satietyBar = null;
    //インスペクターのマネージャー
    [SerializeField] private EquipmentManager equipmentMgr = null;
    //ショートカットのマネージャー
    [SerializeField] private ShortCutManager shortCutMgr = null;

    public enum HandItemKind { None, Tool, Food, Medicine, Other,}

    //現在体力
    private float hitPoint;
    //現在スタミナ
    private float staminaPoint;
    //現在満腹度
    private float satietyPoint;
    //現在攻撃力
    private int attackPoint;
    //現在防御力
    private int defensePoint;
    //個別の防具の防御力
    private int[] defenseList = new int[3];
    //手に持っているアイテム
    private ItemData handItem;
    //手に持っているアイテムの種類
    private HandItemKind handItemKind;

    private void Start()
    {
        //最大値を現在値に格納
        hitPoint = maxHitPoint;
        staminaPoint = maxStaminaPoint;
        satietyPoint = maxSatietyPoint;
        //hitPoint = 10;
        //satietyPoint = 40;
        //satietyBar.UpdateBar(satietyPoint, maxSatietyPoint);
        //hpBar.UpdateBar(hitPoint, maxHitPoint);
        //初期値を現在値に格納
        attackPoint = initAttack;
        defensePoint = initDefense;

        //20秒ごとに満腹度を減らす
        StartCoroutine(ReleaseSatiety());
    }
    //防御力の変更
    public void ChangeDefense(int defense,int armorType)
    {
        //防御力を初期化
        defensePoint = initDefense;
        defenseList[armorType] = defense;
        //装備欄全部の防御力を加算
        for (int i = 0;i < 3;i++)
        {
            defensePoint += defenseList[i]; 
        }
        equipmentMgr.UpdateDefenseText(defensePoint);
        //Debug.Log("防御力" + defensePoint);
    }
    //攻撃力の変更
    public void ChangeAttack(int attack)
    {
        attackPoint = attack + initAttack;
        equipmentMgr.UpdateAttackText(attackPoint);
        //Debug.Log("防御力" + defensePoint);
    }
    //ダメージを受けた時の処理
    void ReceiveDamage(int damege)
    {
        //ダメージを防御力分減らす
        damege -= defensePoint;
        //ダメージが0より大きければ体力を減らす
        if (damege > 0)
        {
            //体力の減少
            hitPoint -= damege;
            if(hitPoint > 0)
            {
                //体力バーの更新
                hpBar.UpdateBar(hitPoint, maxHitPoint);
            }
            else
            {
                SceneManager.LoadScene("GameOverScene");
            }
            
        }
        
    }
    //スタミナの減少
    public void ReleaseStamina(float num)
    {
        if (staminaPoint > 0)
        {
            //スタミナの減少
            staminaPoint -= num;
            //スタミナバーの更新
            staminaBar.UpdateBar(staminaPoint, maxStaminaPoint);
        } 
    }
    //スタミナ回復
    public void AddStamina(float num)
    {
        if(staminaPoint < maxStaminaPoint)
        {
            //スタミナの回復
            staminaPoint += num;
            //スタミナバーの更新
            staminaBar.UpdateBar(staminaPoint, maxStaminaPoint);
        }  
    }
    //アイテムの使用
    public void UseItem()
    {
        seMgr.PlayEatSE();
        //Debug.Log(handItemKind);
        //switch文
        switch (handItemKind)
        {
            //食べ物の場合満腹度を回復
            case HandItemKind.Food:
                FoodData foodData = handItem as FoodData;
                //満腹度の回復
                satietyPoint += foodData.GetSatiety();
                //満腹度が上限を超えないようにする
                if (satietyPoint > maxSatietyPoint)
                {
                    satietyPoint = maxSatietyPoint;
                }
                //満腹度バーの更新
                satietyBar.UpdateBar(satietyPoint, maxSatietyPoint);
                //アイテムの個数が０になったらhandItemKindを初期化する
                if (shortCutMgr.ReduceInventory(1)) { handItemKind = HandItemKind.None; }

                break;
            //回復の場合は体力を回復
            case HandItemKind.Medicine:
                MedicineData medicineData = handItem as MedicineData;
                //体力の回復
                hitPoint += medicineData.GetHealHp();
                //体力が上限を超えないようにする
                if(hitPoint > maxHitPoint)
                {
                    hitPoint = maxHitPoint;
                }
                //体力バーの更新
                hpBar.UpdateBar(hitPoint, maxHitPoint);
                //アイテムの個数が０になったらhandItemKindを初期化する
                if(shortCutMgr.ReduceInventory(1)) { handItemKind = HandItemKind.None; }
                break;
            default:
                break;
        }

    }
    public void SetHandItem(ItemData item)
    {
        handItem = item;
        //手に持っているアイテムの種類に合わせてhandItemKindを変更する
        //空の場合
        if (handItem == null) { handItemKind = HandItemKind.None; }
        //武器・道具の場合
        else if (handItem as ToolData) { handItemKind = HandItemKind.Tool; }
        //食べ物の場合
        else if (handItem as FoodData) { handItemKind = HandItemKind.Food; }
        //回復の場合
        else if (handItem as MedicineData) { handItemKind = HandItemKind.Medicine; }
        else { handItemKind = HandItemKind.Other; }
       
    }

    public void HandItemAnimator(Animator animator)
    {
        switch (handItemKind)
        {
            //何もない場合はパンチのアニメーションを使用する
            case HandItemKind.None:
                animator.SetTrigger("Punch");
                break;
           //武器・道具を持っている場合は攻撃のアニメーションを使用する
            case HandItemKind.Tool:
                animator.SetTrigger("Attack");
                break;
           //食べ物・回復の場合はアイテムの使用処理をする
            case HandItemKind.Food:
            case HandItemKind.Medicine:
                UseItem();
                break;
            default:
                break;

        }

                //if(handItemKind == HandItemKind.Tool) { return true; }
                //return false;
    }
    public ItemData GetHandItem() { return handItem; }

    IEnumerator ReleaseSatiety()
    {
        //Debug.Log("check");
        yield return new WaitForSeconds(20f);

        satietyPoint --;
        if(satietyPoint > 0)
        {
            //満腹度バーの更新
            satietyBar.UpdateBar(satietyPoint, maxSatietyPoint);
        }
        else
        {
            SceneManager.LoadScene("GameOverScene");
        }
        
        StartCoroutine(ReleaseSatiety());
    }
    public int GetAttack() { return attackPoint; }
    public int GetDefense() { return defensePoint; }
    public float GetStamina() { return staminaPoint; }
    public float GetMaxStamina() { return maxStaminaPoint; }
}
