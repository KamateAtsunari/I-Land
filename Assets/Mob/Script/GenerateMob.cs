using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMob : MonoBehaviour
{
    const float minPosX = -12.5f;
    const float maxPosX = 12.5f;
    const float minPosZ = -12.5f;
    const float maxPosZ = 12.5f;
    //生成の時間間隔の最小値
    const float minTime = 10f;
    //生成の時間間隔の最大値
    const float maxTime = 20f;
    //Mobが出現する確率
    const float rate = 50f;

    [SerializeField] private GameObject playerObj = null;
    [SerializeField] private MobManager mobMgr = null;
    [SerializeField] private SkyManager skyMgr = null; 
    //地面のゲームオブジェクト
    [SerializeField] private Terrain terrain = null;
    //朝に生成されるMobのリスト
    [SerializeField] private List<GenerateRate> morningGenerates = new List<GenerateRate>();
    //夜に生成されるMobのリスト
    [SerializeField] private List<GenerateRate> nightGenerates = new List<GenerateRate>();


    //Mobが生成される位置
    private Vector3 generatePos;
    //生成の時間間隔
    private float interval;
    //経過時間
    private float time = 0f;
    //生成されるMobのリスト
    private List<GenerateRate> generateRates;

    [System.Serializable]
    //生成されるMobの種類と確率
    private class GenerateRate
    {
        //モンスターのプレハブ
        [SerializeField] private GameObject mobObj = null;
        //出現確率
        [SerializeField] private float selectRate = 0;

        public GameObject GetMobObject() { return mobObj; }
        public float GetSelectRate() { return selectRate; }
    }

        // Start is called before the first frame update
    void Start()
    {
        //時間間隔を決定する
        interval = GetRandomTime();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerObj.transform.position);
        if (distance < 70)
        {
            //時間計測
            time += Time.deltaTime;
            //経過時間が生成時間になったとき
            if (time > interval)
            {
                //Debug.Log(mobMgr.CheakMobNum());
                //全体のMobの数が上限以下ならMobの生成を行う
                if (mobMgr.CheakMobNum())
                {
                    ChangeGenerates();
                    RandomGenerate(generateRates);
                    //経過時間を初期化
                    time = 0;
                }
                
            }
        }
        else
        {
            time = 0;
        }
    }
    private void RandomGenerate(List<GenerateRate> gList)
    {
        //Debug.Log(skyMgr.GetIsMorning());
        generatePos.y = terrain.GetPosition().y;
        //ランダムな数値を取得
        float random = Random.Range(0, 100f);
        //ランダムな数値がMobの出現率より低ければMobを生成
        if (random < rate)
        {
            
            //ランダムな数値を取得
            random = Random.Range(0, 100f);
            for (int i = 0; i < gList.Count; i++)
            {
                //Debug.Log("generate");
                if (gList[i].GetSelectRate() > random)
                {
                    //mobの生成位置を格納
                    generatePos.x = Random.Range(minPosX, maxPosX);
                    generatePos.z = Random.Range(minPosZ, maxPosZ);
                    generatePos += this.transform.position;

                    //mobの生成
                    Instantiate(gList[i].GetMobObject(), generatePos, transform.rotation);
                    //Mobの数を増やす
                    mobMgr.AddMobNum();
                    //Debug.Log(generateRateList[i].GetMobObject());
                    return;
                }
                else
                {
                    random -= gList[i].GetSelectRate();
                }
            }
        }
    }
    //ランダムな時間を生成する関数
    private float GetRandomTime()
    {
        return Random.Range(minTime, maxTime);
    }
    private void ChangeGenerates()
    {
        //朝か夜かによって生成するMobの種類を変更する
        generateRates = (skyMgr.GetIsMorning()) ? morningGenerates : nightGenerates;
    }
    //void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log("aa");
    //    if (other.gameObject.tag == "Player")
    //    {
    //        if (!isGenerate) { isGenerate = true; }
    //    }
    //}

}
