using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;//追加　saitou
using static MapGenerator;

public class MapGenerator : MonoBehaviour
{
    //==================================================
    // インスペクター設定
    //==================================================

    // マップデータ・マップ生成
    [SerializeField] StageData[] stages;
    [SerializeField] public GameObject[] prefabs;
    [SerializeField] Transform map2D;
    [SerializeField] private MiniMapGenerator miniMapGenerator;

    // プレイヤー
    public Player player;

    // UI
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject Puzzle;
    [SerializeField] GameObject Puzzle2;
    [SerializeField] GameObject Puzzle3;
    [SerializeField] GameObject Puzzle4;

    [SerializeField] TMP_Text stageText;
    [SerializeField] TMP_Text floorText;

    // 謎解き入力
    [SerializeField] TMP_Text redNumberText;
    [SerializeField] TMP_Text greenNumberText;
    [SerializeField] TMP_Text blueNumberText;
    int redNumber = 1;
    int greenNumber = 1;
    int blueNumber = 1;

    [SerializeField] TMP_Text redNumberText2;
    [SerializeField] TMP_Text greenNumberText2;
    [SerializeField] TMP_Text yellowNumberText2;
    [SerializeField] TMP_Text blueNumberText2;
    int redNumber2 = 1;
    int greenNumber2 = 1;
    int yellowNumber2 = 1;
    int blueNumber2 = 1;

    private Toggle[] puzzle3Toggles;
    private Toggle[] puzzle4Toggles;

    [SerializeField] UnityEngine.UI.Button yesButton;
    [SerializeField] UnityEngine.UI.Button noButton;
    [SerializeField] UnityEngine.UI.Button answerButton;

    // 階段
    [SerializeField] Image stairImage;
    [SerializeField] Sprite stairUpSprite;
    [SerializeField] Sprite stairDownSprite;

    // 謎解き確認
    [SerializeField] Image puzzleConfirmImage;
    [SerializeField] Sprite puzzleConfirmSprite;


    // ミニマップ
    [SerializeField] float miniMapScale = 0.3f;
    [SerializeField] Vector2 miniMapOffset = new Vector2(-7.4f, -3.5f);

    // 5*5ミニマップ
    [SerializeField] Transform minimap;
    [SerializeField] float minimapTileSize = 100f;
    [SerializeField] Sprite playerArrowSprite;

    [SerializeField] GameObject treasureChestImage;

    // ギミック
    [SerializeField] private GameObject pitGimmick1Image;
    [SerializeField] private GameObject pitGimmick2Image;
    [SerializeField] private GameObject GIMMICK2_1;
    [SerializeField] private GameObject GIMMICK2_2;
    [SerializeField] private GameObject GIMMICK2_3;
    [SerializeField] private GameObject GIMMICK2_4;
    [SerializeField] private GameObject GIMMICK2_5;
    [SerializeField] private GameObject GIMMICK2_6;
    [SerializeField] private GameObject GIMMICK2_7;
    [SerializeField] private GameObject GIMMICK2_8;
    [SerializeField] private GameObject GIMMICK2_9;
    [SerializeField] private GameObject GIMMICK2_10;
    [SerializeField] private GameObject GIMMICK2_11;
    [SerializeField] private GameObject GIMMICK2_12;
    [SerializeField] private GameObject GIMMICK2_13;
    [SerializeField] private GameObject GIMMICK2_14;
    [SerializeField] private GameObject GIMMICK2_15;
    [SerializeField] private GameObject GIMMICK3_1;
    [SerializeField] private GameObject GIMMICK3_2;
    [SerializeField] private GameObject GIMMICK3_3;
    [SerializeField] private GameObject GIMMICK3_4;
    [SerializeField] private GameObject GIMMICK3_5;
    [SerializeField] private GameObject GIMMICK3_R;
    [SerializeField] private GameObject GIMMICK3_G;
    [SerializeField] private GameObject GIMMICK3_Y;
    [SerializeField] private GameObject GIMMICK3_B;



    //==================================================
    // マップ関連
    //==================================================

    public enum MAP_TYPE
    {
        GROUND = 0, 
        WALL = 1,   
        PLAYER = 2, 
        GOAL = 3,   
        PIT = 4,    
        POISON = 5,
        PUZZLE = 30, 
        PUZZLE2 = 31, 
        PUZZLE3 = 32, 
        PUZZLE4 = 33,    
        PUZZLE5 = 34,     
        PUZZLE6 = 35,    
        STAIR_1_2 = 40,  
        STAIR_2_3 = 41,  
        STAIR_3_4 = 42,
        PIT_GIMMICK1 = 50,
        PIT_GIMMICK2 = 51,
        GIMMICK2_1 = 60,
        GIMMICK2_2 = 61,
        GIMMICK2_3 = 62,
        GIMMICK2_4 = 63,
        GIMMICK2_5 = 64,
        GIMMICK2_6 = 65,
        GIMMICK2_7 = 66,
        GIMMICK2_8 = 67,
        GIMMICK2_9 = 68,
        GIMMICK2_10 = 69,
        GIMMICK2_11 = 70,
        GIMMICK2_12 = 71,
        GIMMICK2_13 = 72,
        GIMMICK2_14 = 73,
        GIMMICK2_15 = 74,
        GIMMICK3_1 = 80,
        GIMMICK3_2 = 81,
        GIMMICK3_3 = 82,
        GIMMICK3_4 = 83,
        GIMMICK3_5 = 84,
        GIMMICK3_R = 85,
        GIMMICK3_G = 86,
        GIMMICK3_Y = 87,
        GIMMICK3_B = 88,
        WARP1 = 90,
        WARP2 = 91,
        WARP3 = 92,
        WARP4 = 93,
        WARP5 = 94,
        WARP6 = 95,
        WARP7 = 96,
        WARP8 = 97,
        WARP9 = 98,
        WARP10 = 99,
        WARP11 = 100,
        WARP12 = 101,
        WARP13 = 102,
        WARP14 = 103,
        WARP15 = 104,
        WARP16 = 105
    }

    public MAP_TYPE[,] mapTable;

    // 探索済みマップ
    public Dictionary<string, bool[,]> discoveredMaps = new Dictionary<string, bool[,]>();
    // 発見した落とし穴
    public Dictionary<string, bool[,]> discoveredPitMaps = new Dictionary<string, bool[,]>();
    // 謎解きクリア済み
    public Dictionary<string, bool[,]> solvedPuzzleMaps = new Dictionary<string, bool[,]>();
    // 発見した毒
    public Dictionary<string, bool[,]> discoveredPoisonMaps = new Dictionary<string, bool[,]>();
    // 現在の階の探索済みマップ
    bool[,] discovered;
    // 現在の階の落とし穴探索済み
    bool[,] discoveredPit;　

    Vector2 centerPos;
    float mapSize;

    public Vector2Int startPos;


    //==================================================
    // ステージ・階層
    //==================================================

    public int currentStage = 0;
    public int currentFloor = 0;


    //==================================================
    // 謎解き状態
    //==================================================

    public bool puzzleSolved = false;
    public bool puzzle2Solved = false;
    public bool puzzle3Solved = false;
    public bool puzzle4Solved = false;
    bool puzzleConfirm = false;
    bool puzzle2Confirm = false;
    bool puzzle3Confirm = false;
    bool puzzle4Confirm = false;

    //==================================================
    // 初期化
    //==================================================

    private void Start()
    {
        Panel.SetActive(false);
        Puzzle.SetActive(false);
        Puzzle2.SetActive(false);
        Puzzle3.SetActive(false);
        Puzzle4.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);
        treasureChestImage.SetActive(false);

        redNumberText.text = redNumber.ToString();
        greenNumberText.text = greenNumber.ToString();
        blueNumberText.text = blueNumber.ToString();

        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);

        int selectedStage = PlayerPrefs.GetInt("SelectedStage", 0);

        puzzle3Toggles = Puzzle3.GetComponentsInChildren<Toggle>(true);
        puzzle4Toggles = Puzzle4.GetComponentsInChildren<Toggle>(true);

        

        pitGimmick1Image.SetActive(false);
        pitGimmick2Image.SetActive(false);
        GIMMICK2_1.SetActive(false);
        GIMMICK2_2.SetActive(false);
        GIMMICK2_3.SetActive(false);
        GIMMICK2_4.SetActive(false);
        GIMMICK2_5.SetActive(false);
        GIMMICK2_6.SetActive(false);
        GIMMICK2_7.SetActive(false);
        GIMMICK2_8.SetActive(false);
        GIMMICK2_9.SetActive(false);
        GIMMICK2_10.SetActive(false);
        GIMMICK2_11.SetActive(false);
        GIMMICK2_12.SetActive(false);
        GIMMICK2_13.SetActive(false);
        GIMMICK2_14.SetActive(false);
        GIMMICK2_15.SetActive(false);
        GIMMICK3_1.SetActive(false);
        GIMMICK3_2.SetActive(false);
        GIMMICK3_3.SetActive(false);
        GIMMICK3_4.SetActive(false);
        GIMMICK3_5.SetActive(false);
        GIMMICK3_R.SetActive(false);
        GIMMICK3_G.SetActive(false);
        GIMMICK3_Y.SetActive(false);
        GIMMICK3_B.SetActive(false);



        currentStage = selectedStage;
        currentFloor = 0;

        _loadMapData();
        _createMap();
        _updateStageText();
        UpdateMinimap();

        // 元の2Dマップは非表示にする
        SpriteRenderer[] mapSprites =
            map2D.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in mapSprites)
        {
            sr.enabled = false;
        }
        // プレイヤーの見た目だけ非表示
        SpriteRenderer playerSR =
            player.GetComponent<SpriteRenderer>();

        if (playerSR != null)
        {
            playerSR.enabled = false;
        }
    }


    //==================================================
    // 更新処理
    //==================================================

    private void Update()
    {
        // 1キー → ステージ1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeStage(0);
        }

        // 2キー → ステージ2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeStage(1);
        }

        // 3キー → ステージ3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeStage(2);
        }
    }


    //==================================================
    // マップ情報取得
    //==================================================

    public MAP_TYPE GetNextMapType(Vector2Int _pos)
    {
        // マップ外なら壁として扱う
        if (_pos.x < 0 || _pos.x >= mapTable.GetLength(0) ||
            _pos.y < 0 || _pos.y >= mapTable.GetLength(1))
        {
            return MAP_TYPE.WALL;
        }

        return mapTable[_pos.x, _pos.y];
    }


    //==================================================
    // マップデータ読み込み
    //==================================================

    void _loadMapData()
    {
        TextAsset currentMap =
            stages[currentStage].floors[currentFloor];

        string[] mapLines =
            currentMap.text.Split(
                new[] { '\n', '\r' },
                System.StringSplitOptions.RemoveEmptyEntries);

        // 行数
        int row = mapLines.Length;

        // 列数
        int col = mapLines[0].Split(',').Length;

        // マップ配列を初期化
        mapTable = new MAP_TYPE[col, row];

        for (int y = 0; y < row; y++)
        {
            string[] mapValues =
                mapLines[y].Split(new char[] { ',' });

            for (int x = 0; x < col; x++)
            {
                mapTable[x, y] =
                    (MAP_TYPE)int.Parse(mapValues[x]);
            }
        }

        // ステージ・階層ごとに探索状況を保存
        string mapKey = currentStage + "_" + currentFloor;

        // 探索済み
        if (!discoveredMaps.ContainsKey(mapKey))
        {
            discoveredMaps[mapKey] = new bool[col, row];
        }

        discovered = discoveredMaps[mapKey];

        // 落とし穴
        if (!discoveredPitMaps.ContainsKey(mapKey))
        {
            discoveredPitMaps[mapKey] = new bool[col, row];
        }
        // 毒
        if (!discoveredPoisonMaps.ContainsKey(mapKey))
        {
            discoveredPoisonMaps[mapKey] = new bool[col, row];
        }
    }

    //==================================================
    // 全体マップ生成
    //==================================================

    void _createMap()
    {
        float tileSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

        mapSize = tileSize;

        // マップの中心位置を計算
        if (mapTable.GetLength(0) % 2 == 0)
        {
            centerPos.x =
                mapTable.GetLength(0) / 2 * mapSize
                - (mapSize / 2);
        }
        else
        {
            centerPos.x =
                mapTable.GetLength(0) / 2 * mapSize;
        }

        if (mapTable.GetLength(1) % 2 == 0)
        {
            centerPos.y =
                mapTable.GetLength(1) / 2 * mapSize
                - (mapSize / 2);
        }
        else
        {
            centerPos.y =
                mapTable.GetLength(1) / 2 * mapSize;
        }

        for (int y = 0; y < mapTable.GetLength(1); y++)
        {
            for (int x = 0; x < mapTable.GetLength(0); x++)
            {
                Vector2Int pos =
                    new Vector2Int(x, y);

                // 床Prefab
                GameObject _ground =
                    Instantiate(
                        prefabs[0],
                        map2D);

                // マップの種類に応じてPrefabを選択
                GameObject mapPrefab = null;

                switch (mapTable[x, y])
                {
                    case MAP_TYPE.GROUND:
                        // 床の場合は床Prefab
                        mapPrefab = prefabs[0];
                        break;

                    case MAP_TYPE.WALL:
                        mapPrefab = prefabs[1];
                        break;

                    case MAP_TYPE.PLAYER:
                        mapPrefab = prefabs[2];
                        break;

                    case MAP_TYPE.GOAL:
                        mapPrefab = prefabs[3];
                        break;

                    case MAP_TYPE.PIT:
                        mapPrefab = prefabs[4];
                        break;

                     

                    // パズル6種類は同じPrefabを使用
                    case MAP_TYPE.PUZZLE:
                    case MAP_TYPE.PUZZLE2:
                    case MAP_TYPE.PUZZLE3:
                    case MAP_TYPE.PUZZLE4:
                    case MAP_TYPE.PUZZLE5:
                    case MAP_TYPE.PUZZLE6:
                        mapPrefab = prefabs[5];
                        break;

                    // 階段3種類は同じPrefabを使用
                    case MAP_TYPE.STAIR_1_2:
                    case MAP_TYPE.STAIR_2_3:
                    case MAP_TYPE.STAIR_3_4:
                        mapPrefab = prefabs[6];
                        break;

                    case MAP_TYPE.PIT_GIMMICK1:
                    case MAP_TYPE.PIT_GIMMICK2:
                    case MAP_TYPE.GIMMICK2_1:
                    case MAP_TYPE.GIMMICK2_2:
                    case MAP_TYPE.GIMMICK2_3:
                    case MAP_TYPE.GIMMICK2_4:
                    case MAP_TYPE.GIMMICK2_5:
                    case MAP_TYPE.GIMMICK2_6:
                    case MAP_TYPE.GIMMICK2_7:
                    case MAP_TYPE.GIMMICK2_8:
                    case MAP_TYPE.GIMMICK2_9:
                    case MAP_TYPE.GIMMICK2_10:
                    case MAP_TYPE.GIMMICK2_11:
                    case MAP_TYPE.GIMMICK2_12:
                    case MAP_TYPE.GIMMICK2_13:
                    case MAP_TYPE.GIMMICK2_14:
                    case MAP_TYPE.GIMMICK2_15:
                    case MAP_TYPE.GIMMICK3_1:
                    case MAP_TYPE.GIMMICK3_2:
                    case MAP_TYPE.GIMMICK3_3:
                    case MAP_TYPE.GIMMICK3_4:
                    case MAP_TYPE.GIMMICK3_5:
                    case MAP_TYPE.GIMMICK3_R:
                    case MAP_TYPE.GIMMICK3_G:
                    case MAP_TYPE.GIMMICK3_Y:
                    case MAP_TYPE.GIMMICK3_B:
                        mapPrefab = prefabs[0];
                        break;

                    case MAP_TYPE.WARP1:
                    case MAP_TYPE.WARP2:
                    case MAP_TYPE.WARP3:
                    case MAP_TYPE.WARP4:
                    case MAP_TYPE.WARP5:
                    case MAP_TYPE.WARP6:
                    case MAP_TYPE.WARP7:
                    case MAP_TYPE.WARP8:
                    case MAP_TYPE.WARP9:
                    case MAP_TYPE.WARP10:
                    case MAP_TYPE.WARP11:
                    case MAP_TYPE.WARP12:
                    case MAP_TYPE.WARP13:
                    case MAP_TYPE.WARP14:
                    case MAP_TYPE.WARP15:
                    case MAP_TYPE.WARP16:
                        mapPrefab = prefabs[7];
                        Debug.Log("ワープPrefab：" + mapPrefab);
                        break;

                    case MAP_TYPE.POISON:
                        mapPrefab = prefabs[8];
                        break;

                    default:
                        Debug.LogError(
                            "対応するPrefabがありません：" +
                            mapTable[x, y]);

                        break;
                }

                if (mapPrefab == null)
                {
                    Destroy(_ground);
                    continue;
                }

                GameObject _map =
                    Instantiate(
                        mapPrefab,
                        map2D);

                _ground.transform.localPosition =
                    ScreenPos(pos);

                _map.transform.localPosition =
                    ScreenPos(pos);

                _ground.transform.localScale =
                    Vector3.one * miniMapScale;

                _map.transform.localScale =
                    Vector3.one * miniMapScale;

                SpriteRenderer groundSR =
                    _ground.GetComponent<SpriteRenderer>();

                SpriteRenderer mapSR =
                    _map.GetComponent<SpriteRenderer>();

                if (groundSR != null)
                {
                    groundSR.sortingOrder = 10;
                }

                if (mapSR != null)
                {
                    mapSR.sortingOrder = 20;
                }

                // プレイヤーの初期位置を設定
                if (mapTable[x, y] == MAP_TYPE.PLAYER)
                {
                    startPos = pos;

                    player.currentPos = pos;
                    player.transform.localPosition =
                        ScreenPos(pos);

                    player.mapGenerator = this;

                    DiscoverPlayerPosition();

                    Destroy(_map);
                }
            }
        }
    }

    public void UpdateMinimap()
    {
        miniMapGenerator.UpdateMinimap();
    }

    public void ShowTreasureChest()
    {
        treasureChestImage.SetActive(true);

        player.isPuzzle = true;

        // 現在のステージをクリア済みにする
        PlayerPrefs.SetInt("Stage" + (currentStage + 1) + "_Cleared", 1);
        PlayerPrefs.Save();
        CheckAllStageClear();//追加　
    }
    void CheckAllStageClear()
    {
        if (PlayerPrefs.GetInt("Stage1_Cleared", 0) == 1 &&
            PlayerPrefs.GetInt("Stage2_Cleared", 0) == 1 &&
            PlayerPrefs.GetInt("Stage3_Cleared", 0) == 1)
        {
            SceneManager.LoadScene("Ending");
        }
    }

    public void DiscoverPlayerPosition()
    {
        int x = player.currentPos.x;
        int y = player.currentPos.y;

        if (x >= 0 && x < discovered.GetLength(0) &&
            y >= 0 && y < discovered.GetLength(1))
        {
            discovered[x, y] = true;
        }
    }
    public void DiscoverPit(Vector2Int pos)
    {
        string mapKey = currentStage + "_" + currentFloor;

        if (discoveredPitMaps.ContainsKey(mapKey))
        {
            discoveredPitMaps[mapKey][pos.x, pos.y] = true;
        }

        UpdateMinimap();
    }
    public void DiscoverPoison(Vector2Int pos)
    {
        string mapKey = currentStage + "_" + currentFloor;

        if (discoveredPoisonMaps.ContainsKey(mapKey))
        {
            discoveredPoisonMaps[mapKey][pos.x, pos.y] = true;
        }

        UpdateMinimap();
    }

    public void CheckPitGimmick()
    {
        // まず両方消す
        pitGimmick1Image.SetActive(false);
        pitGimmick2Image.SetActive(false);
        GIMMICK2_1.SetActive(false);
        GIMMICK2_2.SetActive(false);
        GIMMICK2_3.SetActive(false);
        GIMMICK2_4.SetActive(false);
        GIMMICK2_5.SetActive(false);
        GIMMICK2_6.SetActive(false);
        GIMMICK2_7.SetActive(false);
        GIMMICK2_8.SetActive(false);
        GIMMICK2_9.SetActive(false);
        GIMMICK2_10.SetActive(false);
        GIMMICK2_11.SetActive(false);
        GIMMICK2_12.SetActive(false);
        GIMMICK2_13.SetActive(false);
        GIMMICK2_14.SetActive(false);
        GIMMICK2_15.SetActive(false);
        GIMMICK3_1.SetActive(false);
        GIMMICK3_2.SetActive(false);
        GIMMICK3_3.SetActive(false);
        GIMMICK3_4.SetActive(false);
        GIMMICK3_5.SetActive(false);
        GIMMICK3_R.SetActive(false);
        GIMMICK3_G.SetActive(false);
        GIMMICK3_Y.SetActive(false);
        GIMMICK3_B.SetActive(false);


        // プレイヤーがいる場所のマップタイプを取得
        MAP_TYPE type = GetNextMapType(player.currentPos);

        // 50
        if (type == MAP_TYPE.PIT_GIMMICK1)
        {
            pitGimmick1Image.SetActive(true);
        }
        // 51
        else if (type == MAP_TYPE.PIT_GIMMICK2)
        {
            pitGimmick2Image.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_1)
        {
            GIMMICK2_1.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_2)
        {
            GIMMICK2_2.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_3)
        {
            GIMMICK2_3.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_4)
        {
            GIMMICK2_4.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_5)
        {
            GIMMICK2_5.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_6)
        {
            GIMMICK2_6.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_7)
        {
            GIMMICK2_7.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_8)
        {
            GIMMICK2_8.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_9)
        { 
            GIMMICK2_9.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_10)
        {
            GIMMICK2_10.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_11)
        {
            GIMMICK2_11.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_12)
        {
            GIMMICK2_12.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_13)
        {
            GIMMICK2_13.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_14)
        {
            GIMMICK2_14.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK2_15)
        {
            GIMMICK2_15.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_1)
        {
            GIMMICK3_1.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_2)
        {
            GIMMICK3_2.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_3)
        {
            GIMMICK3_3.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_4)
        {
            GIMMICK3_4.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_5)
        {
            GIMMICK3_5.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_R)
        {
            GIMMICK3_R.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_G)
        {
            GIMMICK3_G.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_Y)
        {
            GIMMICK3_Y.SetActive(true);
        }
        else if (type == MAP_TYPE.GIMMICK3_B)
        {
            GIMMICK3_B.SetActive(true);
        }
    }

    public Vector2Int GetWarpTarget(Vector2Int currentPos)
    {
        MAP_TYPE type = GetNextMapType(currentPos);

        MAP_TYPE targetType;

        switch (type)
        {
            case MAP_TYPE.WARP1:
                targetType = MAP_TYPE.WARP2;
                break;

            case MAP_TYPE.WARP2:
                targetType = MAP_TYPE.WARP1;
                break;

            case MAP_TYPE.WARP3:
                targetType = MAP_TYPE.WARP4;
                break;

            case MAP_TYPE.WARP4:
                targetType = MAP_TYPE.WARP3;
                break;

            case MAP_TYPE.WARP5:
                targetType = MAP_TYPE.WARP6;
                break;

            case MAP_TYPE.WARP6:
                targetType = MAP_TYPE.WARP5;
                break;

            case MAP_TYPE.WARP7:
                targetType = MAP_TYPE.WARP8;
                break;

            case MAP_TYPE.WARP8:
                targetType = MAP_TYPE.WARP7;
                break;

            case MAP_TYPE.WARP9:
                targetType = MAP_TYPE.WARP10;
                break;

            case MAP_TYPE.WARP10:
                targetType = MAP_TYPE.WARP9;
                break;

            case MAP_TYPE.WARP11:
                targetType = MAP_TYPE.WARP12;
                break;

            case MAP_TYPE.WARP12:
                targetType = MAP_TYPE.WARP11;
                break;

            case MAP_TYPE.WARP13:
                targetType = MAP_TYPE.WARP14;
                break;

            case MAP_TYPE.WARP14:
                targetType = MAP_TYPE.WARP13;
                break;

            case MAP_TYPE.WARP15:
                targetType = MAP_TYPE.WARP16;
                break;

            case MAP_TYPE.WARP16:
                targetType = MAP_TYPE.WARP15;
                break;

            default:
                return currentPos;
        }

        for (int x = 0; x < mapTable.GetLength(0); x++)
        {
            for (int y = 0; y < mapTable.GetLength(1); y++)
            {
                if (mapTable[x, y] == targetType)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return currentPos;
    }


    public Vector2 ScreenPos(Vector2Int _pos)
    {
        return new Vector2(
            (_pos.x * mapSize - centerPos.x)
            * miniMapScale
            + miniMapOffset.x,

            (-(_pos.y * mapSize - centerPos.y))
            * miniMapScale
            + miniMapOffset.y
        );
    }


    //==================================================
    // 階段・ステージ情報
    //==================================================

    void _updateStageText()
    {
        stageText.text =
            "Stage" + (currentStage + 1);

        floorText.text =
            (currentFloor + 1) + "F";
    }

    public Vector2Int FindStairPos()
    {
        for (int y = 0;y < mapTable.GetLength(1);y++)
        {
            for (int x = 0;x < mapTable.GetLength(0);x++)
            {
                MAP_TYPE type = mapTable[x, y];

                if (type == MAP_TYPE.STAIR_1_2 || type == MAP_TYPE.STAIR_2_3 || type == MAP_TYPE.STAIR_3_4)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero;
    }


    //==================================================
    // 謎解き
    //==================================================

    public void RedUp()
    {
        if (redNumber < 9)
        {
            redNumber++;
            redNumberText.text = redNumber.ToString();
        }
    }

    public void RedDown()
    {
        if (redNumber > 1)
        {
            redNumber--;
            redNumberText.text = redNumber.ToString();
        }
    }

    public void GreenUp()
    {
        if (greenNumber < 9)
        {
            greenNumber++;
            greenNumberText.text = greenNumber.ToString();
        }
    }

    public void GreenDown()
    {
        if (greenNumber > 1)
        {
            greenNumber--;
            greenNumberText.text = greenNumber.ToString();
        }
    }

    public void BlueUp()
    {
        if (blueNumber < 9)
        {
            blueNumber++;
            blueNumberText.text = blueNumber.ToString();
        }
    }

    public void BlueDown()
    {
        if (blueNumber > 1)
        {
            blueNumber--;
            blueNumberText.text = blueNumber.ToString();
        }
    }

    public void RedUp2()
    {
        if (redNumber2 < 9)
        {
            redNumber2++;
            redNumberText2.text = redNumber2.ToString();                                                                               
        }
    }

    public void RedDown2()
    {
        if (redNumber2 > 1)
        {
            redNumber2--;
            redNumberText2.text = redNumber2.ToString();
        }
    }


    public void GreenUp2()
    {
        if (greenNumber2 < 9)
        {
            greenNumber2++;
            greenNumberText2.text = greenNumber2.ToString();
        }
    }

    public void GreenDown2()
    {
        if (greenNumber2 > 1)
        {
            greenNumber2--;
            greenNumberText2.text = greenNumber2.ToString();
        }
    }


    public void YellowUp2()
    {
        if (yellowNumber2 < 9)
        {
            yellowNumber2++;
            yellowNumberText2.text = yellowNumber2.ToString();
        }
    }

    public void YellowDown2()
    {
        if (yellowNumber2 > 1)
        {
            yellowNumber2--;
            yellowNumberText2.text = yellowNumber2.ToString();
        }
    }


    public void BlueUp2()
    {
        if (blueNumber2 < 9)
        {
            blueNumber2++;
            blueNumberText2.text = blueNumber2.ToString();
        }
    }

    public void BlueDown2()
    {
        if (blueNumber2 > 1)
        {
            blueNumber2--;
            blueNumberText2.text = blueNumber2.ToString();
        }
    }
    public void CheckPuzzle()
    {
        if (redNumber == 2 &&
            greenNumber ==3 &&
            blueNumber == 9)
        {
            Debug.Log("謎解き正解！");
            SEManager.Instance.PlayCorrect();

            puzzleSolved = true;
            Puzzle.SetActive(false);
            player.isPuzzle = false;
            UpdateMinimap();
        }
        else
        {
            Debug.Log("不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    public void CheckPuzzle2F()
    {
        if (redNumber2 == 1 &&
            greenNumber2 == 3 &&
            yellowNumber2 == 7 &&
            blueNumber2 == 5)
        {
            Debug.Log("2F謎解き正解！");
            SEManager.Instance.PlayCorrect();

            puzzle2Solved = true;
            Puzzle2.SetActive(false);
            player.isPuzzle = false;
            UpdateMinimap();
        }
        else
        {
            Debug.Log("不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    int GetSelectedAnswer(Toggle[] toggles)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                return i + 1;
            }
        }

        return 0;
    }

    public void CheckPuzzle3()
    {
        int answer = GetSelectedAnswer(puzzle3Toggles);

        if (answer == 1)
        {
            Debug.Log("Puzzle3正解！");
            SEManager.Instance.PlayCorrect();

            puzzle3Solved = true;
            Puzzle3.SetActive(false);
            player.isPuzzle = false;

            UpdateMinimap();
        }
        else
        {
            Debug.Log("Puzzle3不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    public void CheckPuzzle4()
    {
        int answer = GetSelectedAnswer(puzzle4Toggles);

        if (answer == 4)
        {
            Debug.Log("Puzzle4正解！");
            SEManager.Instance.PlayCorrect();

            puzzle4Solved = true;
            Puzzle4.SetActive(false);
            player.isPuzzle = false;

            UpdateMinimap();
        }
        else
        {
            Debug.Log("Puzzle4不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    public void OpenPuzzle()
    {
        if (puzzleSolved)
        {
            return;
        }

        treasureChestImage.SetActive(true);
        puzzleConfirm = true;
        puzzleConfirmImage.sprite = puzzleConfirmSprite;
        puzzleConfirmImage.gameObject.SetActive(true);
        Panel.SetActive(true);
        player.isPuzzle = true;
    }

    public void OpenPuzzle2()
    {
        if (puzzle2Solved)
        {
            return;
        }

        treasureChestImage.SetActive(true);
        puzzle2Confirm = true;
        puzzleConfirmImage.sprite = puzzleConfirmSprite;
        puzzleConfirmImage.gameObject.SetActive(true);
        Panel.SetActive(true);
        player.isPuzzle = true;
    }

    public void OpenPuzzle3()
    {
        if (puzzle3Solved)
        {
            return;
        }

        treasureChestImage.SetActive(true);
        puzzle3Confirm = true;
        puzzleConfirmImage.sprite = puzzleConfirmSprite;
        puzzleConfirmImage.gameObject.SetActive(true);
        Panel.SetActive(true);
        player.isPuzzle = true;
    }

    public void OpenPuzzle4()
    {
        if (puzzle4Solved)
        {
            return;
        }

        treasureChestImage.SetActive(true);
        puzzle4Confirm = true;
        puzzleConfirmImage.sprite = puzzleConfirmSprite;
        puzzleConfirmImage.gameObject.SetActive(true);
        Panel.SetActive(true);
        player.isPuzzle = true;
    }

    //==================================================
    // 階段
    //==================================================

    public void CheckStair()
    {
        MAP_TYPE nextMapType = GetNextMapType(player.currentPos);

        if (nextMapType == MAP_TYPE.STAIR_1_2 || nextMapType == MAP_TYPE.STAIR_2_3 || nextMapType == MAP_TYPE.STAIR_3_4)
        {
            // 現在の階と階段の種類に応じて画像を変更
            if (nextMapType == MAP_TYPE.STAIR_1_2)
            {
                if (currentFloor == 0)
                {
                    // 1F → 2F
                    stairImage.sprite = stairUpSprite;
                }
                else
                {
                    // 2F → 1F
                    stairImage.sprite = stairDownSprite;
                }
            }
            else if (nextMapType == MAP_TYPE.STAIR_2_3)
            {
                if (currentFloor == 1)
                {
                    // 2F → 3F
                    stairImage.sprite = stairUpSprite;
                }
                else
                {
                    // 3F → 2F
                    stairImage.sprite = stairDownSprite;
                }
            }
            else if (nextMapType == MAP_TYPE.STAIR_3_4)
            {
                if (currentFloor == 2)
                {
                    // 3F → 4F
                    stairImage.sprite = stairUpSprite;
                }
                else
                {
                    // 4F → 3F
                    stairImage.sprite = stairDownSprite;
                }
            }

            stairImage.gameObject.SetActive(true);
            Panel.SetActive(true);
            player.isPuzzle = true;
        }
    }

    public void Yes()
    {
        Panel.SetActive(false);
        treasureChestImage.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);
        player.isPuzzle = false;

        // Puzzle1
        if (puzzleConfirm)
        {
            puzzleConfirm = false;

            Puzzle.SetActive(true);
            player.isPuzzle = true;

            return;
        }

        // Puzzle2
        if (puzzle2Confirm)
        {
            puzzle2Confirm = false;

            Puzzle2.SetActive(true);
            player.isPuzzle = true;

            return;
        }

        // Puzzle3
        if (puzzle3Confirm)
        {
            puzzle3Confirm = false;

            Puzzle3.SetActive(true);
            player.isPuzzle = true;

            return;
        }

        // Puzzle4
        if (puzzle4Confirm)
        {
            puzzle4Confirm = false;

            Puzzle4.SetActive(true);
            player.isPuzzle = true;

            return;
        }

        // 階段の場合
        MAP_TYPE nextMapType = GetNextMapType(player.currentPos);

        switch (nextMapType)
        {
            case MAP_TYPE.STAIR_1_2:

                if (currentFloor == 0)
                {
                    // 1F → 2F
                    ChangeFloor(1);
                }
                else if (currentFloor == 1)
                {
                    // 2F → 1F
                    ChangeFloor(0);
                }

                break;


            case MAP_TYPE.STAIR_2_3:

                if (currentFloor == 1)
                {
                    // 2F → 3F
                    ChangeFloor(2);
                }
                else if (currentFloor == 2)
                {
                    // 3F → 2F
                    ChangeFloor(1);
                }

                break;


            case MAP_TYPE.STAIR_3_4:

                if (currentFloor == 2)
                {
                    // 3F → 4F
                    ChangeFloor(3);
                }
                else if (currentFloor == 3)
                {
                    // 4F → 3F
                    ChangeFloor(2);
                }

                break;
        }
    }

    public void No()
    {
        Panel.SetActive(false);
        treasureChestImage.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);

        puzzleConfirm = false;
        puzzle2Confirm = false;
        puzzle3Confirm = false;
        puzzle4Confirm = false;

        player.isPuzzle = false;
    }

    public void ChangeFloor(
        int floor,
        bool moveToStair = true)
    {
        Debug.Log("ChangeFloor開始：" + floor);

        if (floor < 0 ||
            floor >= stages[currentStage].floors.Length)
        {
            Debug.Log("階数が範囲外！");
            return;
        }

        currentFloor = floor;

        // 現在のマップを削除
        while (map2D.childCount > 0)
        {
            DestroyImmediate(
                map2D.GetChild(0).gameObject);
        }

        // 新しい階のマップを読み込む
        _loadMapData();
        _createMap();
        _updateStageText();


        // 階段の位置へプレイヤーを移動
        if (moveToStair)
        {
            Vector2Int stairPos = FindStairPos();

            player.currentPos = stairPos;
            player.transform.localPosition =
                ScreenPos(stairPos);

            DiscoverPlayerPosition();
        }
    }


    //==================================================
    // ステージ変更
    //==================================================

    public void ChangeStage(int stage)
    {
        currentStage = stage;
        currentFloor = 0;

        Debug.Log("★★★ ステージ" + (stage + 1) + "が呼ばれました ★★★");   

        while (map2D.childCount > 0)
        {
            DestroyImmediate(
                map2D.GetChild(0).gameObject);
        }

        _loadMapData();
        _createMap();
        _updateStageText();
    }


    //==================================================
    // 外部から現在の階を取得
    //==================================================

    public int CurrentFloor
    {
        get
        {
            return currentFloor;
        }
    }
    public int CurrentStage
    {
        get
        {
            return currentStage; 
        }
    }
}