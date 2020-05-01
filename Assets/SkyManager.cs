using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    //角度を入れる変数
    [SerializeField] private float sunPos = 0;
    [SerializeField] private MobManager mobMgr = null;
    //朝か夜かを格納
    private bool isMorning = true; 
    void Update()
    {
        //X軸を回転させる
        transform.eulerAngles = new Vector3(sunPos, 0, 0);

        //1日のスピードを調節する
        sunPos += Time.deltaTime * 0.5f;

        //360度以上になったら0に戻す
        if (sunPos > 360) {
            sunPos = 0;
            //朝にする
            isMorning = true;
        }

        //朝の場合
        if (isMorning)
        {
            //太陽の位置が200度以上になったら夜にする
            if (sunPos > 200)
            {
                isMorning = false;
            }
        }
        //Debug.Log(isMorning);
    }
    public bool GetIsMorning()
    {
        return isMorning;
    }
    //エネミーは朝になると削除される
    public void ExtinguishEnemy()
    {
        //Mobタグを持っているオブジェクトを取得
        GameObject[] mobObjs = GameObject.FindGameObjectsWithTag("Mob");

        foreach (GameObject mobObj in mobObjs)
        {
            if (mobObj.GetComponent<EnemyAi>())
            {
                Destroy(mobObj);
                mobMgr.ReduceMobNum();
            }
        }
    }
}
