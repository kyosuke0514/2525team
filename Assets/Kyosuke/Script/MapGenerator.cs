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

    // 全体マップ
    [SerializeField] GameObject FullMap;
    [SerializeField] GameObject fullMapTilePrefab;

    // ミニマップ
    [SerializeField] float miniMapScale = 0.3f;
    [SerializeField] Vector2 miniMapOffset = new Vector2(-7.4f, -3.5f);

    // 3*3ミニマップ
    [SerializeField] Transform minimap;
    [SerializeField] float minimapTileSize = 100f;
    [SerializeField] Sprite playerArrowSprite;

    [SerializeField] GameObject treasureChestImage;

    //==================================================
    // マップ関連
    //==================================================

    public enum MAP_TYPE
    {
        GROUND, // 0
        WALL,   // 1
        PLAYER, // 2
        STAIR,  // 3
        GOAL,   // 4
        PIT,    // 5
        PUZZLE, // 6
        PUZZLE2 // 7
    }

    MAP_TYPE[,] mapTable;

    // 探索済みマップ
    Dictionary<string, bool[,]> discoveredMaps =
        new Dictionary<string, bool[,]>();

    bool[,] discovered;

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
        // Mキーで全体マップを開閉
        if (Input.GetKeyDown(KeyCode.M))
        {
            FullMap.SetActive(!FullMap.activeSelf);

            if (FullMap.activeSelf)
            {
                CreateFullMap();
                player.isPuzzle = true;
            }
            else
            {
                player.isPuzzle = false;
            }
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

        if (!discoveredMaps.ContainsKey(mapKey))
        {
            discoveredMaps[mapKey] = new bool[col, row];
        }

        discovered = discoveredMaps[mapKey];
    }


    //==================================================
    // マップ生成
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

                GameObject _ground =
                    Instantiate(
                        prefabs[(int)MAP_TYPE.GROUND],
                        map2D);

                GameObject _map =
                    Instantiate(
                        prefabs[(int)mapTable[x, y]],
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


    //==================================================
    // 全体マップ生成
    //==================================================

    void CreateFullMap()
    {
        // 前回の全体マップを削除
        for (int i = FullMap.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(FullMap.transform.GetChild(i).gameObject);
        }

        int width = mapTable.GetLength(0);
        int height = mapTable.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool show = false;

                // 探索済みなら表示
                if (discovered[x, y])
                {
                    show = true;
                }

                // 探索済みマスの周囲にある壁を表示
                if (!show)
                {
                    Vector2Int[] directions =
                    {
                        new Vector2Int(0, 1),
                        new Vector2Int(0, -1),
                        new Vector2Int(1, 0),
                        new Vector2Int(-1, 0)
                    };

                    foreach (Vector2Int dir in directions)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;

                        if (nx >= 0 && nx < width &&
                            ny >= 0 && ny < height)
                        {
                            if (discovered[nx, ny] &&
                                mapTable[x, y] == MAP_TYPE.WALL)
                            {
                                show = true;
                            }
                        }
                    }
                }

                // マップ外周は常に表示
                if (x == 0 || x == width - 1 ||
                    y == 0 || y == height - 1)
                {
                    show = true;
                }

                if (!show)
                {
                    continue;
                }

                GameObject tile =
                    Instantiate(
                        fullMapTilePrefab,
                        FullMap.transform);

                RectTransform rect =
                    tile.GetComponent<RectTransform>();

                float tileSize = 30f;

                float mapWidth =
                    width * tileSize;

                float mapHeight =
                    height * tileSize;

                // マップを中央に配置
                rect.anchoredPosition = new Vector2(
                    x * tileSize
                    - mapWidth / 2f
                    + tileSize / 2f,

                    -y * tileSize
                    + mapHeight / 2f
                    - tileSize / 2f
                );

                // 壁を黒く表示
                if (mapTable[x, y] == MAP_TYPE.WALL)
                {
                    Image image =
                        tile.GetComponent<Image>();

                    if (image != null)
                    {
                        image.color = Color.black;
                    }
                }
            }
        }
    }

    public void UpdateMinimap()
    {
        Debug.Log("★★★ ミニマップ更新 ★★★");

        //==================================================
        // 3×3ミニマップを更新
        //==================================================

        // 前に作った3×3マップを削除
        for (int i = minimap.childCount - 1; i >= 0; i--)
        {
            Destroy(minimap.GetChild(i).gameObject);
        }

        //==================================================
        // プレイヤーを中心に3×3を表示
        //==================================================

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                // プレイヤーから見たマップ上の位置
                Vector2Int pos =
                    player.currentPos +
                    new Vector2Int(x, y);

                // マップ外なら壁として扱う
                MAP_TYPE type =
                    GetNextMapType(pos);


                //==================================================
                // マスを作成
                //==================================================

                GameObject tile =
                    Instantiate(
                        prefabs[(int)MAP_TYPE.GROUND],
                        minimap
                    );

                SpriteRenderer sr =
                    tile.GetComponent<SpriteRenderer>();


                if (sr != null)
                {
                    // マップの種類に応じた画像を取得
                    SpriteRenderer original =
                        prefabs[(int)type]
                        .GetComponent<SpriteRenderer>();

                    if (original != null)
                    {
                        sr.sprite = original.sprite;
                    }

                    // プレイヤーの初期位置だった場所は
                    // ミニマップでは普通の床として表示
                    if (type == MAP_TYPE.PLAYER)
                    {
                        SpriteRenderer ground =
                            prefabs[(int)MAP_TYPE.GROUND]
                            .GetComponent<SpriteRenderer>();

                        if (ground != null)
                        {
                            sr.sprite = ground.sprite;
                        }

                        sr.color = Color.black;
                    }

                    // 色を設定
                    if (x == 0 && y == 0)
                    {
                        // プレイヤー → 白
                        sr.color = Color.white;

                        // プレイヤーを一番手前に表示
                        sr.sortingOrder = 110;
                    }
                    else if (type == MAP_TYPE.WALL)
                    {
                        // 壁 → グレー
                        sr.color = Color.gray;

                        sr.sortingOrder = 100;
                    }
                    else
                    {
                        // 床 → 黒
                        sr.color = Color.black;

                        sr.sortingOrder = 100;
                    }

                    //==================================================
                    // プレイヤーを中央に表示
                    //==================================================

                    if (x == 0 && y == 0)
                    {
                        if (x == 0 && y == 0)
                        {
                            //==================================================
                            // 中央は「黒い床」
                            //==================================================

                            sr.color = Color.black;
                            sr.sortingOrder = 100;


                            //==================================================
                            // その上にプレイヤーの矢印を作る
                            //==================================================

                            GameObject arrow =
                                new GameObject("MinimapPlayerArrow");

                            arrow.transform.SetParent(minimap);

                            arrow.transform.localPosition = Vector3.zero;

                            arrow.transform.localScale =
                                Vector3.one * 100f;

                            SpriteRenderer arrowSR =
                                arrow.AddComponent<SpriteRenderer>();

                            // 作った矢印画像
                            arrowSR.sprite = playerArrowSprite;

                            // 白色
                            arrowSR.color = Color.white;

                            // 床より前
                            arrowSR.sortingOrder = 110;


                            //==================================================
                            // プレイヤーの向き
                            //==================================================

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
                    }
                    else
                    {
                        sr.sortingOrder = 100;
                    }
                }


                //==================================================
                // 3×3上の位置
                //==================================================

                tile.transform.localPosition =
                    new Vector3(
                        x * minimapTileSize,
                        -y * minimapTileSize,
                        0
                    );

                // 3×3用の大きさ
                tile.transform.localScale =
                    Vector3.one*100f;
            }
        }
    }

    public void ShowTreasureChest()
    {
        treasureChestImage.SetActive(true);

        player.isPuzzle = true;
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
                if (mapTable[x, y] == MAP_TYPE.STAIR)
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
        if (GetNextMapType(player.currentPos)
            == MAP_TYPE.STAIR)
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
        if (currentFloor == 0)
        {
            ChangeFloor(1);
        }
        else
        {
            ChangeFloor(0);
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

        // 全体マップを開いている場合は更新
        if (FullMap.activeSelf)
        {
            CreateFullMap();
        }

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