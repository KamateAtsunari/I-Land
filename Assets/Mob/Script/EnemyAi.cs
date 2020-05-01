using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// "NavMeshAgent"関連クラスを使用できるようにする
using UnityEngine.AI;
	 
public class EnemyAi : DropItemManager
{
    public enum EnemyState
    {
        Wait,
        Patrol,
        Chace,
        Attack,
        Death
    }
    //キャラクターのステータス
    [SerializeField] public CharaStatus charaStatus = null;
    //キャラクターの攻撃が届く距離
    [SerializeField] private float attackRange = 0f;
    //キャラクターの攻撃の範囲
    [SerializeField] private float attackAngle = 0f;
    [SerializeField] private AudioClip attackSE = null;
    [SerializeField] private AudioClip downSE = null;

    private NavMeshAgent agent;
    private Vector3 startPoint;
    private Animator animator;
    //Enemyの体力
    private int hitPoint;
    //追尾・攻撃の対象となるオブジェクト
    private GameObject targetObj;
    AudioSource audioSource;

    private EnemyState state;
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
        //ステータスをパトロールに変更
        SetState(EnemyState.Patrol);

        GetComponent<SphereCollider>().enabled = true;
        //agent.speed = charaStatus.GetWalkSeed();
        //Debug.Log(charaStatus.GetWalkSeed());
        audioSource = GetComponent<AudioSource>();
    }

    // ゲーム実行中の繰り返し処理
    void Update()
    {
        if (state != EnemyState.Death)
        {
            switch (state)
            {
                case EnemyState.Wait:
                    return;

                case EnemyState.Patrol:
                    // エージェントが現在の巡回地点に到達したら
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        // 次の巡回地点を設定する処理を実行
                        GotoNextRandomPoint();
                    return;

                case EnemyState.Chace:
                    GotoPlayerPoint();
                    // エージェントがプレイヤーの近くに到達したら
                    if (!agent.pathPending && agent.remainingDistance < 2f)
                        SetState(EnemyState.Attack);
                    return;

                case EnemyState.Attack:
                    //Debug.Log("Attack");
                    this.transform.LookAt(targetObj.transform);
                    animator.SetTrigger("Attack");
                    //target.gameObject.SendMessage("ReceiveDamage", (float)charaStatus.statusAttack);
                    SetState(EnemyState.Wait);
                    return;

                default:
                    Debug.LogError("Error");
                    break;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") { return; }
        //ステータスを追跡にする
        SetState(EnemyState.Chace);
        //対象オブジェクトが入っていなかった場合、格納
        if(targetObj == null){ targetObj = other.gameObject; }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") { return; }
        GotoNextRandomPoint();
        //ステータスをパトロールにする
        SetState(EnemyState.Patrol);
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
                StartCoroutine(FriezeTime());
                break;

            case EnemyState.Patrol:
                agent.speed = charaStatus.GetWalkSeed();
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                break;

            case EnemyState.Chace:
                agent.speed = charaStatus.GetRunSeed();
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                break;

            case EnemyState.Attack:
                animator.SetBool("isRunning", false);
                
                //animator.SetTrigger("Attack");
                break;
            case EnemyState.Death:
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                animator.ResetTrigger("Attack");
                //agent.destination = this.transform.position;
                break;
            default:
                Debug.LogError("Error");
                break;
        }
    }

    // 次の巡回地点を設定する処理
    void GotoNextRandomPoint()
    {
        //（エージェントは目的地点に近づいても減速しない)
        agent.autoBraking = false;

        Vector3 destPoint = Random.insideUnitCircle * 20;

        destPoint = startPoint + new Vector3(destPoint.x, 0, destPoint.y);

        agent.destination = destPoint;

    }
    void GotoPlayerPoint()
    {
        //（エージェントは目的地点に近づいたら減速する)
        agent.autoBraking = true;
        //対象オブジェクトに向かって移動
        agent.destination = targetObj.transform.position;
    }
    IEnumerator FriezeTime()
    {
        //Debug.Log("check");
        yield return new WaitForSeconds(2f);
        GotoPlayerPoint();
        // エージェントがプレイヤーの近くに到達したら
        if (!agent.pathPending && agent.remainingDistance < 2f)
            SetState(EnemyState.Attack);
        else
            SetState(EnemyState.Chace);
    }
    //ダメージを受けたとき
    public bool ReceiveDamage(int damege)
    {
        //ダメージを防御力分減らす
        damege -= charaStatus.statusDefence;

        //ダメージが0より大きければ体力を減らす
        if (damege > 0)
        {
            //体力の減少
            hitPoint -= damege;
        }
        //体力が０以下になったら死亡アニメーションを使用する
        if(hitPoint <= 0)
        {
            SetState(EnemyState.Death);
            //Debug.Log("deth");
            animator.SetTrigger("Death");
            audioSource.PlayOneShot(downSE);
            agent.isStopped = true;
            return true;
        }
        return false;
    }
    void Attack()
    {
        audioSource.PlayOneShot(attackSE);
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, attackRange, transform.forward);
        foreach (RaycastHit hit in hits)
        {

            //マップ上のオブジェクトに衝突したとき
            if (hit.transform.gameObject == targetObj)
            {
                //対象と自分の角度を取得
                float angle = Vector3.Angle(hit.transform.position - transform.position, transform.forward);
                //Debug.Log(angle);
                //対象と自分との距離を取得
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (angle <= attackAngle / 2 && distance <= attackRange)
                {
                    //Debug.Log("eAttack");
                    hit.transform.gameObject.SendMessage("ReceiveDamage", charaStatus.statusAttack);
                    return;
                }
            }
        }
    }
    void DeathEnd()
    {
        Destroy(this.gameObject);
    }
    public void GetHitPoint()
    {

    }
}
