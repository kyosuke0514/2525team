using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] TMP_InputField redInput;
    [SerializeField] TMP_InputField greenInput;
    [SerializeField] TMP_InputField blueInput;

    [SerializeField] TMP_InputField redInput2;
    [SerializeField] TMP_InputField greenInput2;
    [SerializeField] TMP_InputField yellowInput2;
    [SerializeField] TMP_InputField blueInput2;

    [SerializeField] UnityEngine.UI.Button yesButton;
    [SerializeField] UnityEngine.UI.Button noButton;
    [SerializeField] UnityEngine.UI.Button answerButton;

    // 階段
    [SerializeField] Image stairImage;
    [SerializeField] Sprite stairUpSprite;
    [SerializeField] Sprite stairDownSprite;

    // 全体マップ
    [SerializeField] GameObject FullMap;
    [SerializeField] GameObject fullMapTilePrefab;

    // ミニマップ
    [SerializeField] float miniMapScale = 0.3f;
    [SerializeField] Vector2 miniMapOffset = new Vector2(-7.4f, -3.5f);


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


    //==================================================
    // 初期化
    //==================================================

    private void Start()
    {
        Panel.SetActive(false);
        Puzzle.SetActive(false);
        Puzzle2.SetActive(false);

        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);

        _loadMapData();
        _createMap();
        _updateStageText();
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


    //==================================================
    // プレイヤー位置・探索記録
    //==================================================

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
        for (int y = 0;
             y < mapTable.GetLength(1);
             y++)
        {
            for (int x = 0;
                 x < mapTable.GetLength(0);
                 x++)
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

    public void CheckPuzzle()
    {
        if (redInput.text == "2" &&
            greenInput.text == "3" &&
            blueInput.text == "9")
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
        Debug.Log("赤：" + redInput2.text);
        Debug.Log("緑：" + greenInput2.text);
        Debug.Log("黄：" + yellowInput2.text);
        Debug.Log("青：" + blueInput2.text);

        if (redInput2.text == "1" &&
            greenInput2.text == "3" &&
            yellowInput2.text == "7" &&
            blueInput2.text == "5")
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

        Puzzle.SetActive(true);
        player.isPuzzle = true;
    }

    public void OpenPuzzle2()
    {
        if (puzzle2Solved)
        {
            return;
        }

        Puzzle2.SetActive(true);
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
        player.isPuzzle = false;

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