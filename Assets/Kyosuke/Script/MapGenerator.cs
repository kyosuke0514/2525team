using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MapGenerator;

public class MapGenerator : MonoBehaviour
{
    //==================================================
    // インスペクター設定
    //==================================================

    // マップデータ・マップ生成
    [SerializeField] StageData[] stages;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] Transform map2D;

    // プレイヤー
    public Player player;

    // UI
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject Puzzle;
    [SerializeField] GameObject Puzzle2;

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
        PUZZLE = 30, 
        PUZZLE2 = 31, 
        PUZZLE3 = 32, 
        PUZZLE4 = 33,    
        PUZZLE5 = 34,     
        PUZZLE6 = 35,    
        STAIR_1_2 = 40,  
        STAIR_2_3 = 41,  
        STAIR_3_4 = 42   
    }

    MAP_TYPE[,] mapTable;

    // 探索済みマップ
    Dictionary<string, bool[,]> discoveredMaps = new Dictionary<string, bool[,]>();
    // 発見した落とし穴
    Dictionary<string, bool[,]> discoveredPitMaps = new Dictionary<string, bool[,]>();
    // 謎解きクリア済み
    Dictionary<string, bool[,]> solvedPuzzleMaps = new Dictionary<string, bool[,]>();

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

    int currentStage = 0;
    int currentFloor = 0;


    //==================================================
    // 謎解き状態
    //==================================================

    bool puzzleSolved = false;
    bool puzzle2Solved = false;
    bool puzzleConfirm = false;
    bool puzzle2Confirm = false;

    //==================================================
    // 初期化
    //==================================================

    private void Start()
    {
        Panel.SetActive(false);
        Puzzle.SetActive(false);
        Puzzle2.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);
        treasureChestImage.SetActive(false);

        redNumberText.text = redNumber.ToString();
        greenNumberText.text = greenNumber.ToString();
        blueNumberText.text = blueNumber.ToString();

        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);

        int selectedStage = PlayerPrefs.GetInt("SelectedStage", 0);

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
    }

    //==================================================
    // 全体マップ生成
    //==================================================

    void _createMap()
    {
        float tileSize =
            prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

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
        Debug.Log("★★★ ミニマップ更新 ★★★");

        //==================================================
        // 前のミニマップを削除
        //==================================================

        for (int i = minimap.childCount - 1; i >= 0; i--)
        {
            Destroy(minimap.GetChild(i).gameObject);
        }


        //==================================================
        // 5×5ミニマップを作成
        //==================================================

        for (int y = -2; y <= 2; y++)
        {
            for (int x = -2; x <= 2; x++)
            {
                Vector2Int pos =
                    player.currentPos + new Vector2Int(x, y);

                MAP_TYPE type = GetNextMapType(pos);

                string mapKey =
                    currentStage + "_" + currentFloor;


                //==================================================
                // 落とし穴が発見済みか
                //==================================================

                bool pitDiscovered = false;

                if (pos.x >= 0 &&
                    pos.x < mapTable.GetLength(0) &&
                    pos.y >= 0 &&
                    pos.y < mapTable.GetLength(1))
                {
                    if (discoveredPitMaps.ContainsKey(mapKey))
                    {
                        pitDiscovered =
                            discoveredPitMaps[mapKey][pos.x, pos.y];
                    }
                }


                //==================================================
                // 黒い背景を作成
                //==================================================

                GameObject tile =
                    Instantiate(
                        prefabs[(int)MAP_TYPE.GROUND],
                        minimap
                    );

                SpriteRenderer sr =
                    tile.GetComponent<SpriteRenderer>();

                if (sr == null)
                    continue;


                // 黒背景
                sr.color = Color.black;
                sr.sortingOrder = 97;


                //==================================================
                // マップの種類に応じて表示
                //==================================================

                if (type == MAP_TYPE.WALL)
                {
                    // 壁
                    sr.color = Color.gray;
                }
                else if (type == MAP_TYPE.STAIR_1_2 || type == MAP_TYPE.STAIR_2_3 || type == MAP_TYPE.STAIR_3_4)
                {
                    // 階段
                    CreateMinimapIcon(tile, type);
                }
                else if (type == MAP_TYPE.PUZZLE && !puzzleSolved)
                {
                    // 1F謎解き
                    CreateMinimapIcon(tile, type);
                }
                else if (type == MAP_TYPE.PUZZLE2 && !puzzle2Solved)
                {
                    // 2F謎解き
                    CreateMinimapIcon(tile, type);
                }
                else if (type == MAP_TYPE.PIT && pitDiscovered)
                {
                    // 発見済み落とし穴
                    CreateMinimapIcon(tile, type);
                }

                //==================================================
                // 位置・大きさ
                //==================================================

                tile.transform.localPosition =
                    new Vector3(
                        x * minimapTileSize,
                        -y * minimapTileSize,
                        0
                    );

                tile.transform.localScale =
                    Vector3.one * 100f;
            }
        }
        CreatePlayerArrow();
    }

    private void CreatePlayerArrow()
    {
        // プレイヤー画像を作成
        GameObject arrow = new GameObject("Player");

        // ミニマップの子にする
        arrow.transform.SetParent(minimap, false);

        // 5×5ミニマップの中央
        arrow.transform.localPosition = Vector3.zero;

        // タイルと同じくらいの大きさ
        arrow.transform.localScale = Vector3.one * 100f;

        // SpriteRendererを追加
        SpriteRenderer arrowSR =
            arrow.AddComponent<SpriteRenderer>();

        arrowSR.sprite = playerArrowSprite;
        arrowSR.color = Color.white;

        // タイルより前に表示
        arrowSR.sortingOrder = 100;

        // プレイヤーの向きに合わせて回転
        switch (player.direction)
        {
            case Player.DIRECTION.TOP:
                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, 0);
                break;

            case Player.DIRECTION.RIGHT:
                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, -90);
                break;

            case Player.DIRECTION.DOWN:
                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, 180);
                break;

            case Player.DIRECTION.LEFT:
                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, 90);
                break;
        }
    }

    private void CreateMinimapIcon(GameObject tile, MAP_TYPE type)
    {
        GameObject sourcePrefab = null;

        // 表示するアイコンの元Prefabを種類ごとに選択
        switch (type)
        {
            case MAP_TYPE.PUZZLE:
            case MAP_TYPE.PUZZLE2:
            case MAP_TYPE.PUZZLE3:
            case MAP_TYPE.PUZZLE4:
            case MAP_TYPE.PUZZLE5:
            case MAP_TYPE.PUZZLE6:
                sourcePrefab = prefabs[5];
                break;

            case MAP_TYPE.STAIR_1_2:
            case MAP_TYPE.STAIR_2_3:
            case MAP_TYPE.STAIR_3_4:
                sourcePrefab = prefabs[6];
                break;

            case MAP_TYPE.PIT:
                sourcePrefab = prefabs[4];
                break;

            default:
                return;
        }

        if (sourcePrefab == null)
        {
            return;
        }

        SpriteRenderer original =
            sourcePrefab.GetComponent<SpriteRenderer>();

        if (original == null || original.sprite == null)
        {
            return;
        }

        // ミニマップ用アイコンを作成
        GameObject icon =
            new GameObject("MinimapIcon");

        icon.transform.SetParent(tile.transform);

        icon.transform.localPosition =
            Vector3.zero;

        icon.transform.localScale =
            Vector3.one;

        SpriteRenderer iconSR =
            icon.AddComponent<SpriteRenderer>();

        iconSR.sprite = original.sprite;
        iconSR.color = Color.white;
        iconSR.sortingOrder = 98;
    }

    public void ShowTreasureChest()
    {
        treasureChestImage.SetActive(true);

        player.isPuzzle = true;

        // 現在のステージをクリア済みにする
        PlayerPrefs.SetInt("Stage" + (currentStage + 1) + "_Cleared", 1);
        PlayerPrefs.Save();
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

            puzzleSolved = true;
            Puzzle.SetActive(false);
            player.isPuzzle = false;
            UpdateMinimap();
        }
        else
        {
            Debug.Log("不正解！");
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

            puzzle2Solved = true;
            Puzzle2.SetActive(false);
            player.isPuzzle = false;
            UpdateMinimap();
        }
        else
        {
            Debug.Log("不正解！");
        }
    }

    public void OpenPuzzle()
    {
        if (puzzleSolved)
        {
            return;
        }

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

        puzzle2Confirm = true;
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
        puzzleConfirmImage.gameObject.SetActive(false);
        puzzleConfirm = false;
        puzzle2Confirm = false;
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
}