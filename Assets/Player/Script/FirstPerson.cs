using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FirstPerson : MonoBehaviour
{
    //キャラクターステータス
    [SerializeField] private MyStatus myStatus = null;
    //SEマネージャー
    [SerializeField] private SEManager seMgr = null;
    [SerializeField] private MobManager mobMgr = null;
    //歩くスピード
    [SerializeField] private float walkSpeed = 1.5f;
    //走るスピード
    [SerializeField] private float runSpeed = 2.5f;
    //マウス移動のスピード
    [SerializeField] private float mouseSpeed = 2f;
    //キャラクター、カメラ（視点）の回転スピード
    [SerializeField] private float rotateSpeed = 2f;
    //プレイヤーの攻撃が届く距離
    [SerializeField] private float attackRange = 0f;
    //プレイヤーの攻撃の範囲
    [SerializeField] private float attackAngle = 0f;
    //キャラクター視点のカメラの角度限界
    [SerializeField] private float cameraRotateLimit = 30f;
    //カメラの上下の移動方法。マウスを上で上を向く場合はtrue、マウスを上で下を向く場合はfalseを設定
    [SerializeField] private bool cameraRotForward = true;
    //アイテム入手用のレイ
    [SerializeField] private GameObject rayOrigin = null;
    //アイテムを拾う時に表示されるテキスト
    [SerializeField] private GameObject pickUpText = null;
    //メニューUI
    [SerializeField] private GameObject menuUi = null;
    //インベントリのマネージャー
    [SerializeField] private InventoryManager inventoryMgr = null;
    //クラフトのUI
    [SerializeField] private GameObject craftUi = null;
    //装備のUI
    [SerializeField] private GameObject equipmentUi = null;
    

    //キャラクター視点のカメラ
    private Transform myCamera;
    //キャラクターコントローラー
    private CharacterController cCon;
    //キャラクターの速度
    private Vector3 velocity;
    //Animator
    private Animator animator;
    //カメラの角度の初期値
    private Quaternion initCameraRot;
    //　キャラクターのY軸の角度
    private Quaternion charaRotate;
    //　カメラのX軸の角度
    private Quaternion cameraRotate;
    //移動速度
    private float moveSpeed = 0;
    private bool cursorDisplay;
    private RaycastHit raycastHit;
    
    

    void Start()
    {
        //キャラクターコントローラの取得
        cCon = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        //　キャラクター視点のカメラの取得
        myCamera = GetComponentInChildren<Camera>().transform;  
        //カメラの初期位置を格納
        initCameraRot = myCamera.localRotation;
        //キャラクターの位置情報を格納
        charaRotate = transform.localRotation;
        //キャラクターの角度情報を格納
        cameraRotate = myCamera.localRotation;
        moveSpeed = walkSpeed;
        
        //マウスカーソルをウィンドウ内に固定
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = cursorDisplay;

    }

    void Update()
    {
        ExtinguishMob();
        //メニューが開いている場合プレイヤーの動きを制限する
        if (!menuUi.activeSelf)
        {
            //キャラクターの向きを変更する
            RotateChara();
            //視点の向きを変える
            RotateCamera();
            //アイテムを拾う
            CanPickUpItem();
            
            //　キャラクターコントローラのコライダが地面と接触してるかどうか
            if (cCon.isGrounded)
            {
                velocity = Vector3.zero;
                velocity = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")).normalized;

                if (Input.GetKey(KeyCode.W))
                {
                    //キャラクターダッシュ
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //スタミナが０になったらダッシュを解除
                        if (myStatus.GetStamina() > 0) { moveSpeed = runSpeed; }
                        else { moveSpeed = walkSpeed; }
                    }
                    //キャラクターダッシュ解除
                    else if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        moveSpeed = walkSpeed;
                    }
                }
                else if (Input.GetKeyUp(KeyCode.W))
                {
                    moveSpeed = walkSpeed;
                }
                //ダッシュ中はスタミナを減らす
                if (moveSpeed == runSpeed)
                {
                    myStatus.ReleaseStamina(0.2f);
                }
                //ダッシュしていない場合はスタミナを回復する
                else
                {
                    //上限値以上回復しないようにする
                    if (myStatus.GetMaxStamina() > myStatus.GetStamina()) { myStatus.AddStamina(0.2f); }
                }
                animator.SetFloat("DirectionZ", Input.GetAxis("Vertical"));
                animator.SetFloat("DirectionX", Input.GetAxis("Horizontal"));
            }
            velocity.y += Physics.gravity.y * Time.deltaTime; //　重力値を計算
            cCon.Move(velocity * Time.deltaTime * moveSpeed); //　キャラクターコントローラのMoveを使ってキャラクターを移動させる
            //プレイヤーの攻撃
            if (Input.GetMouseButtonDown(0))
            {
                //手に持っているアイテムによってアニメーションを変更
                myStatus.HandItemAnimator(animator);
                
            }
        }

        //装備画面を開く
        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (craftUi.activeSelf)
            {
                craftUi.SetActive(false);
                equipmentUi.SetActive(true);
            }
            else
            {
                cursorDisplay = !cursorDisplay;
                Cursor.visible = cursorDisplay;
                //表示切替
                menuUi.SetActive(!menuUi.activeSelf);
                equipmentUi.SetActive(!equipmentUi.activeSelf);
            }
            
            //クラフトのUIが表示されていた場合非表示にする
            //if (craftUi.activeSelf) { craftUi.SetActive(false); }

            
            //craftUi.SetActive(!craftUi.activeSelf);
        }
        //クラフト画面を開く
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (equipmentUi.activeSelf)
            {
                craftUi.SetActive(true);
                equipmentUi.SetActive(false);
            }
            else
            {
                cursorDisplay = !cursorDisplay;
                Cursor.visible = cursorDisplay;
                //表示切替
                menuUi.SetActive(!menuUi.activeSelf);
                craftUi.SetActive(!craftUi.activeSelf);
            }

            //装備のUIが表示されていた場合非表示にする
            //if (equipmentUi.activeSelf) { equipmentUi.SetActive(false); }
        }
    }
    //キャラクターの角度を変更
    void RotateChara()
    {
        //　横の回転値を計算
        float yRotate = Input.GetAxis("Mouse X") * mouseSpeed;

        charaRotate *= Quaternion.Euler(0f, yRotate, 0f);

        //　キャラクターの回転を実行
        transform.localRotation = Quaternion.Slerp(transform.localRotation, charaRotate, rotateSpeed * Time.deltaTime);
    }
    //カメラの角度を変更
    void RotateCamera()
    {

        float xRotate = Input.GetAxis("Mouse Y") * mouseSpeed;

        //　マウスを上に移動した時に上を向かせたいなら反対方向に角度を反転させる
        if (cameraRotForward)
        {
            xRotate *= -1;
        }
        //　一旦角度を計算する	
        cameraRotate *= Quaternion.Euler(xRotate, 0f, 0f);
        //　カメラのX軸の角度が限界角度を超えたら限界角度に設定
        var resultYRot = Mathf.Clamp(Mathf.DeltaAngle(initCameraRot.eulerAngles.x, cameraRotate.eulerAngles.x), -cameraRotateLimit, cameraRotateLimit);
        //　角度を再構築
        cameraRotate = Quaternion.Euler(resultYRot, cameraRotate.eulerAngles.y, cameraRotate.eulerAngles.z);
        //　カメラの視点変更を実行
        myCamera.localRotation = Quaternion.Slerp(myCamera.localRotation, cameraRotate, rotateSpeed * Time.deltaTime);
    }
    public void CanPickUpItem()
    {
        Ray ray = new Ray(rayOrigin.transform.position, rayOrigin.transform.TransformDirection(Vector3.forward));
        int raydis = 4;
        //レイキャスト（原点, 飛ばす方向, 衝突した情報, 長さ）
        if (Physics.Raycast(ray, out RaycastHit hit, raydis))
        {
            //当たった時の処理
            if (hit.collider.tag == "SmallItem")
            {
                //Debug.Log("aa");
                //テキストの表示
                pickUpText.SetActive(true);
                //アイテムを拾う
                if (Input.GetKeyDown(KeyCode.E))
                {
                    seMgr.PlayBagSE();
                    DropItemManager dropMgr = hit.transform.gameObject.GetComponent<DropItemManager>();
                    List<DropItem> dropItemList = dropMgr.GetDropItemData().GetDropItemList();
                    GetDropItem(dropItemList, hit.transform.gameObject);
                    return;
                }
            }
            else
            {
                //テキストの非表示
                pickUpText.SetActive(false);
            }
        }
        else if(pickUpText.activeSelf)
        {
            //Debug.Log("bb");
            //テキストの非表示
            pickUpText.SetActive(false);
        }
        //Debug.Log(rayOrigin.transform.position);
        Debug.DrawRay(rayOrigin.transform.position, rayOrigin.transform.TransformDirection(Vector3.forward) * raydis, Color.red);
    }
    private void HitCheck()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, attackRange, transform.forward);
        foreach (RaycastHit hit in hits)
        {
            //Mobタグを持つものと衝突したとき
            if (hit.transform.gameObject.tag == "Mob")
            {
                
                //対象と自分の角度を取得
                float angle = Vector3.Angle(hit.transform.position - transform.position, transform.forward);
                //対象と自分との距離を取得
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (angle <= attackAngle / 2 && distance <= attackRange)
                {
                    //対象にダメージを与える
                    //hit.transform.gameObject.SendMessage("ReceiveDamage", myStatus.GetAttack());
                    //Debug.Log("Attack");
                    DropItemManager dropItemMgr = hit.transform.GetComponent<DropItemManager>();
                    //Enemyの場合
                    if (dropItemMgr as EnemyAi)
                    {
                       
                        EnemyAi enemyAi = dropItemMgr as EnemyAi;
                        //ダメージを与え体力が0になったらドロップアイテムを入手する
                        if (enemyAi.ReceiveDamage(myStatus.GetAttack()))
                        {
                            GetDropItem(enemyAi.GetDropItemData().GetDropItemList(),null);
                            //全体のMob数の減少
                            mobMgr.ReduceMobNum();
                            return;
                        }
                    }
                    else if (dropItemMgr as MobAi)
                    {
                        MobAi mobAi = dropItemMgr as MobAi;
                        if (mobAi.ReceiveDamage(myStatus.GetAttack()))
                        {
                            GetDropItem(mobAi.GetDropItemData().GetDropItemList(), null);
                            //全体のMob数の減少
                            mobMgr.ReduceMobNum();
                            return;
                        }
                    }
                    
                }
            }
            //マップ上のアイテムに衝突したとき
            else if(hit.transform.gameObject.tag == "MapItem")
            {
                //対象と自分の角度を取得
                float angle = Vector3.Angle(hit.transform.position - transform.position, transform.forward);
                //対象と自分との距離を取得
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                //対象との角度と距離が設定した範囲内なら
                if (angle <= attackAngle / 2 && distance <= attackRange)
                {
                    //アイテムのコンポーネントを取得
                    MapItem mapItem = hit.transform.GetComponent<MapItem>();
                    //手に持っているアイテムをToolDataに変換する
                    ToolData toolData = myStatus.GetHandItem() as ToolData;
                    //マップにあるアイテムの耐久を減らして0になったらドロップアイテムを取得
                    if (mapItem.ReduceDurability(toolData)) { 
                        GetDropItem(mapItem.GetDropItemData().GetDropItemList(), hit.transform.gameObject);
                        return;
                    }
                }
            }
        }
    }
    //ドロップしたアイテムの取得
    private void GetDropItem(List<DropItem> dropItems,GameObject targetObj)
    {
        //拾ったアイテムの削除
        Destroy(targetObj);
        //for (int i = 0;i < dropItems.Count;i++){
        //    inventoryMgr.AddInventory(dropItems[i].GetItemName(), dropItems[i].GetItemCount());
        //}
        foreach (DropItem dropItem in dropItems)
        {
            //インベントリにアイテム追加
            inventoryMgr.AddInventory(dropItem.GetItemName(), dropItem.GetItemCount());
        }

    }
    //Mobが範囲外に出たら破壊する
    private void ExtinguishMob()
    {
        //Mobタグを持っているオブジェクトを取得
        GameObject[] mobObjs = GameObject.FindGameObjectsWithTag("Mob");

        foreach (GameObject mobObj in mobObjs)
        {
            //プレイヤーとの距離を計測
            float distance = Vector3.Distance(transform.position, mobObj.transform.position);
            //Debug.Log(distance);
            if (distance > 200)
            {
                Destroy(mobObj);
                mobMgr.ReduceMobNum();
            }
        }
    }
    
    void Attack()
    {
        //Debug.Log("Attack");
        HitCheck();
    }
}
