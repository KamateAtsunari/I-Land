using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobAi : DropItemManager
{
    [SerializeField] private AudioClip downSE = null;
    public enum EnemyState
    {
        Wait,
        Patrol,
        Escape,
    }
    [SerializeField] private CharaStatus charaStatus = null;

    private NavMeshAgent agent;
    private Vector3 startPoint;
    private Animator animator;

    private EnemyState state;
    //Mobの体力
    private int hitPoint;
    //逃走の対象となるオブジェクト
    private GameObject targetObj;
    AudioSource audioSource;
    // ゲームスタート時の処理
    void Start()
    {
        hitPoint = charaStatus.GetMobHp();
        // 変数"agent"に NavMesh Agent コンポーネントを格納
        agent = GetComponent<NavMeshAgent>();
        // 巡回地点間の移動を継続させるために自動ブレーキを無効化
        //（エージェントは目的地点に近づいても減速しない)
        agent.autoBraking = false;

        startPoint = transform.position;

        animator = GetComponent<Animator>();

        SetState(EnemyState.Patrol);

        GetComponent<SphereCollider>().enabled = true;
        //GotoNextPoint(Random.insideUnitCircle * 20);
        // 次の巡回地点の処理を実行
        //GotoNextPoint();
        //Componentを取得
        audioSource = GetComponent<AudioSource>();
    }

    // ゲーム実行中の繰り返し処理
    void Update()
    {
        switch (state)
        {
            case EnemyState.Wait:
                //
                return;

            case EnemyState.Patrol:
                // エージェントが現在の巡回地点に到達したら
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    // 次の巡回地点を設定する処理を実行
                    GotoNextRandomPoint();
                return;

            case EnemyState.Escape:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    // 次の巡回地点を設定する処理を実行
                    GotoNextRandomPoint();
                //// エージェントがプレイヤーの近くに到達したら
                //if (!agent.pathPending && agent.remainingDistance < 2f)
                //    SetState(EnemyState.Attack);
                return;

            default:
                Debug.LogError("Error");
                break;
        }
    }

    public void SetState(EnemyState tempState)
    {
        state = tempState;
        switch (state)
        {
            case EnemyState.Wait:
                agent.speed = 0;
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                return;

            case EnemyState.Patrol:
                //Debug.Log("patrol");
                agent.speed = charaStatus.GetWalkSeed();
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                return;

            case EnemyState.Escape:
                //Debug.Log("escape");
                agent.speed = charaStatus.GetRunSeed();
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                return;

            default:
                Debug.LogError("Error");
                break;
        }
    }

    // 次の巡回地点を設定する処理
    void GotoNextRandomPoint()
    {
        Vector3 destPoint = Random.insideUnitCircle * 20;

        destPoint = startPoint + new Vector3(destPoint.x, 0, destPoint.y);

        agent.destination = destPoint;

    }
    void GotoEscapePoint()
    {
        agent.destination = -targetObj.transform.position;
    }
    //プレイヤーが範囲内に入っているとき状態を逃走にする
    void OnTriggerEnter(Collider other)
    {
        //プレイヤータグ以外ならはじく
        if (other.tag != "Player") { return; }
        targetObj = other.gameObject;
        //状態を逃走に変更
        SetState(EnemyState.Escape);
        GotoEscapePoint();
        //Debug.Log(state);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") { return; }

        GotoNextRandomPoint();
        
        SetState(EnemyState.Patrol);
        //Debug.Log(state);
    }
    public bool ReceiveDamage(int damege)
    {
        //体力の減少
        hitPoint -= damege;
        //Debug.Log("Damage");

        //体力が０以下になったら死亡アニメーションを使用する
        if (hitPoint <= 0)
        {
            //Destroy(this.gameObject);
            //Debug.Log("deth");
            animator.SetTrigger("Death");
            audioSource.PlayOneShot(downSE);
            return true;
        }
        return false;
    }
    void DeathEnd()
    {
        //Debug.Log("destory");
        Destroy(this.gameObject);
    }
}
