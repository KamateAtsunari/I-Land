using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Tool {
    public class ObjectPlacementTool : ScriptableWizard {

        //　テレイン
        [SerializeField] private Terrain terrain;
        //　親にするゲームオブジェクトの名前
        [SerializeField] private string parentName;
        //　設置するゲームオブジェクト
        [SerializeField] private GameObject placementObj;
        //　重なりチェックを厳しくするか簡易にするか
        [SerializeField] private bool doubleCheckMode;
        //　シードを変更したランダムを生成するか？
        [SerializeField] private bool randomMode;
        //　地面のTerrainに設定したレイヤー名
        [SerializeField] private LayerMask fieldLayer;
        //　設置するゲームオブジェクトに設定したレイヤー名
        [SerializeField] private LayerMask placementObjLayer;
        //　デフォルトのFieldLayerの名前
        private LayerMask defaultFieldLayer;
        //　デフォルトのPlacementObjLayerの名前
        private LayerMask defaultPlacementObjLayer;
        //　設置する数
        [SerializeField] private int count = 1;
        //　地形の斜めに合わせるかどうか
        [SerializeField] private bool slope;
        //　ベースが下にない時用オフセット値上に移動させる
        [SerializeField] private float offset = 0.0f;
        //　木のスケールをランダムに変更するかどうか
        [SerializeField] private bool useScale;
        //　木の最小スケール
        [SerializeField] private float minScale = 0.1f;
        //　木の最大スケール
        [SerializeField] private float maxScale = 1.0f;

        private GameObject ins;

        void Awake() {
            if (EditorPrefs.HasKey("ObjectPlacementToolData")) {
                LoadData();
            } else {
                fieldLayer = LayerMask.GetMask("Field");
                placementObjLayer = LayerMask.GetMask("Obj");
                //　デフォルトのレイヤーを設定
                defaultFieldLayer = fieldLayer;
                defaultPlacementObjLayer = placementObjLayer;
                //　ヒエラルキー上からTerrainを取得
                terrain = GameObject.FindObjectOfType<Terrain>();
            }
        }

        [MenuItem("ObjectPlacementTool/ObjectPlacementTool")]
        static void CreateWizard() {
            //　ウィザードを表示
            ScriptableWizard.DisplayWizard<ObjectPlacementTool>("自動ゲームオブジェクト設置ツール", "ゲームオブジェクトを設置", "データの初期化");
        }

        //　ウィザードの作成ボタンを押した時に実行
        void OnWizardCreate() {

            //　テレインがセットされていなければエラー
            if (terrain == null) {
                Debug.LogError("not set terrain", this);
                return;
                //　オブジェクトがセットされていなければエラー
            } else if (placementObj == null) {
                Debug.LogError("not set placementObj", this);
                return;
            } else if (count <= 0) {
                Debug.LogError("A numerical value less than or equal to 0 is set");
                return;
            } else if (fieldLayer == 0) {
                Debug.LogError("not set fieldLayer");
                return;
            } else if(placementObjLayer == 0) {
                Debug.LogError("not set placementObjLayer");
                return;
            }

            //　テレインデータを確保
            Vector3 terrainPos = terrain.GetPosition();
            TerrainData terrainData = terrain.terrainData;

            //　親の作成
            GameObject parentObj = new GameObject();

            if (parentName == "") {
                parentName = "parentObj";
            }

            parentObj.name = parentName;

            //　親要素のUndo登録をして親を消す事で子も全部消えるようにしておく
            Undo.RegisterCreatedObjectUndo(parentObj, "Create " + parentObj.name);

            // シードを指定してランダムクラスを作成
            System.Random rand = new System.Random(Time.time.ToString().GetHashCode());

            for (var i = 0; i < count; i++) {

                //　X軸の位置
                float x;
                //　Z軸の位置
                float z;
                //　角度
                Quaternion rot;

                //　Terrainの大きさからランダム値を計算 
                if (randomMode) {
                    x = (float)(terrainPos.x + rand.NextDouble() * terrainData.size.x);
                    z = (float)(terrainPos.z + rand.NextDouble() * terrainData.size.z);
                    //　Y軸をランダムに回転
                    rot = Quaternion.Euler(0, (float)(rand.NextDouble() * 360), 0);
                } else {
                    x = Random.Range(terrainPos.x, terrainPos.x + terrainData.size.x);
                    z = Random.Range(terrainPos.z, terrainPos.z + terrainData.size.z);
                    //　Y軸をランダムに回転
                    rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                }

                //　ランダムな位置を作成
                Vector3 pos = new Vector3(x, terrainPos.y + terrainData.size.y, z);


                int rePosCount = 0;
                bool RePos = false;

                //　自身のレイヤー名が設定されていて他の木にぶつかった場合は位置を再設定
                if (!doubleCheckMode) {
                    while (true) {
                        if (Physics.Raycast(pos, Vector3.down, terrainPos.y + terrainData.size.y + 100f, placementObjLayer)) {
                            if (randomMode) {
                                x = (float)(terrainPos.x + rand.NextDouble() * (terrainPos.x + terrainData.size.x));
                                z = (float)(terrainPos.z + rand.NextDouble() * (terrainPos.z + terrainData.size.z));
                            } else {
                                x = Random.Range(terrainPos.x, terrainPos.x + terrainData.size.x);
                                z = Random.Range(terrainPos.z, terrainPos.z + terrainData.size.z);
                            }
                            pos = new Vector3(x, terrainPos.y + terrainData.size.y, z);
                            Debug.Log("他の木にぶつかった");
                        } else {
                            break;
                        }
                        rePosCount++;

                        //　3回位置を直したらもう直さない
                        if (rePosCount > 3) {
                            RePos = true;
                            break;
                        }
                    }
                }

                RaycastHit hit;

                //　地面の位置とゲームオブジェクトの位置を合わせる
                if (!RePos && Physics.Raycast(pos, Vector3.down, out hit, terrainPos.y + terrainData.size.y + 100f, fieldLayer)) {

                    //　高さを地面に合わせる
                    pos.y = hit.point.y;
                    //　木のインスタンスの作成
                    ins = Instantiate<GameObject>(placementObj, pos, rot);

                    //　地面の傾斜に合わせて木を傾ける設定の場合は木を回転させる
                    if (slope) {
                        ins.transform.rotation = Quaternion.FromToRotation(ins.transform.up, hit.normal) * ins.transform.rotation;
                    }
                    //　ランダムに木のスケールを変える場合
                    if (useScale) {
                        float randomScale = 1f;
                        if (randomMode) {
                            randomScale = (float)(minScale + rand.NextDouble() * maxScale);
                        } else {
                            randomScale = Random.Range(minScale, maxScale);
                        }
                        ins.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                        ins.transform.position += (ins.transform.up * offset) * randomScale;
                    }

                    //　ある程度重なり合っている場合は削除（指定した設置数より大幅に減る）
                    if (doubleCheckMode) {

                        Collider[] hitCol = Physics.OverlapSphere(ins.transform.position, maxScale / 2f, placementObjLayer);

                        //　自身以外の同じレイヤー名のものにヒットしたら残念ながらインスタンスを削除する
                        if (hitCol != null) {
                            bool check = false;
                            foreach (var col in hitCol) {
                                if (col.gameObject.GetInstanceID() != ins.gameObject.GetInstanceID()) {
                                    check = true;
                                }
                            }
                            if (check) {
                                DestroyImmediate(ins.gameObject);
                            }
                        }
                    }
                    if (ins != null) {
                        ins.transform.SetParent(parentObj.transform);
                    }
                }
            }

            SaveData();
        }

        //　ウィザードの他のボタンを押した時に実行
        void OnWizardOtherButton() {
            if(EditorPrefs.HasKey("ObjectPlacementToolData")) {
                EditorPrefs.DeleteKey("ObjectPlacementToolData");
            }
            terrain = GameObject.FindObjectOfType<Terrain>();
            placementObj = null;
            doubleCheckMode = false;
            randomMode = false;
            count = 1;
            slope = false;
            offset = 0.0f;
            useScale = false;
            minScale = 0.1f;
            maxScale = 1.0f;
            parentName = "";
            fieldLayer = defaultFieldLayer;
            placementObjLayer = defaultPlacementObjLayer;
        }

        //　ウィザードで更新があった時に実行
        void OnWizardUpdate() {

            //　設置する木の制限を加える
            if (count > 10000) {
                count = 10000;
            } else if (count <= 0) {
                count = 1;
            }
            //　スケールが0以下に設定されていたら0.1fにする
            if (minScale <= 0) {
                minScale = 0.1f;
            }
        }

        void SaveData() {
            Data data = new Data(terrain, placementObj, doubleCheckMode, randomMode, count, slope, offset, useScale, minScale, maxScale, parentName, fieldLayer, placementObjLayer);

            var jsonData = JsonUtility.ToJson(data);
            EditorPrefs.SetString("ObjectPlacementToolData", jsonData);
        }

        void LoadData() {
            var data = JsonUtility.FromJson<Data>(EditorPrefs.GetString("ObjectPlacementToolData"));
            terrain = data.terrain;
            placementObj = data.placementObj;
            doubleCheckMode = data.doubleCheckMode;
            randomMode = data.randomMode;
            count = data.count;
            slope = data.slope;
            offset = data.offset;
            useScale = data.useScale;
            minScale = data.minScale;
            maxScale = data.maxScale;
            parentName = data.parentName;
            fieldLayer = data.fieldLayer;
            placementObjLayer = data.placementObjLayer;
        }
        [System.Serializable]
        class Data {
            public Terrain terrain;
            public GameObject placementObj;
            public bool doubleCheckMode;
            public bool randomMode;
            public int count;
            public bool slope;
            public float offset;
            public bool useScale;
            public float minScale;
            public float maxScale;
            public string parentName;
            public LayerMask fieldLayer;
            public LayerMask placementObjLayer;

            public Data() {

            }

            public Data(Terrain terrain, GameObject placementObj, bool doubleCheckMode, bool randomMode, int count, bool slope, float offset, bool useScale, float minScale, float maxScale, string parentName, LayerMask fieldLayer, LayerMask placementObjLayer) {
                this.terrain = terrain;
                this.placementObj = placementObj;
                this.doubleCheckMode = doubleCheckMode;
                this.randomMode = randomMode;
                this.count = count;
                this.slope = slope;
                this.offset = offset;
                this.useScale = useScale;
                this.minScale = minScale;
                this.maxScale = maxScale;
                this.parentName = parentName;
                this.fieldLayer = fieldLayer;
                this.placementObjLayer = placementObjLayer;
            }
        }
    }
}
