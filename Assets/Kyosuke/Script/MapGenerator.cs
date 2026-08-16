using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //map.txtを読み込む
    [SerializeField] StageData[] stages;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] Transform map2D;
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject Puzzle;
    [SerializeField] GameObject Puzzle2;
    [SerializeField] UnityEngine.UI.Button yesButton;
    [SerializeField] UnityEngine.UI.Button noButton;
    [SerializeField] TMP_InputField redInput;
    [SerializeField] TMP_InputField greenInput;
    [SerializeField] TMP_InputField blueInput;
    [SerializeField] TMP_InputField redInput2;
    [SerializeField] TMP_InputField greenInput2;
    [SerializeField] TMP_InputField yellowInput2;
    [SerializeField] TMP_InputField blueInput2;
    [SerializeField] UnityEngine.UI.Button answerButton;
    [SerializeField] float miniMapScale = 0.3f;
    [SerializeField] Vector2 miniMapOffset = new Vector2(-7.4f, -3.5f);

    Vector2 centerPos;
    public Vector2Int startPos;

    int currentStage = 0;
    int currentFloor = 0;

    public Player player;
    float mapSize;

    public enum MAP_TYPE
    {
        GROUND, //0
        WALL,   //1
        PLAYER, //2
        STAIR,  //3
        GOAL,   //4
        PIT,    //5
        PUZZLE, //6
        PUZZLE2 //7
    }
    MAP_TYPE[,] mapTable;

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
    private void Start()
    {
        Panel.SetActive(false);
        Puzzle.SetActive(false);
        Puzzle2.SetActive(false);

        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);

        _loadMapData();
        _createMap();
    }

    void _createMap()
    {
        float tileSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;
        mapSize = tileSize;

        if (mapTable.GetLength(0) % 2 == 0)
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize;
        }

        if (mapTable.GetLength(1) % 2 == 0)
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize;
        }

        for (int y = 0; y < mapTable.GetLength(1); y++)
        {
            for (int x = 0; x < mapTable.GetLength(0); x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                GameObject _ground = Instantiate(prefabs[(int)MAP_TYPE.GROUND], map2D);
                GameObject _map = Instantiate(prefabs[(int)mapTable[x, y]], map2D);
                _ground.transform.localPosition = ScreenPos(pos);
                _map.transform.localPosition = ScreenPos(pos);
                _ground.transform.localScale = Vector3.one * miniMapScale;
                _map.transform.localScale = Vector3.one * miniMapScale;

                SpriteRenderer groundSR = _ground.GetComponent<SpriteRenderer>();
                SpriteRenderer mapSR = _map.GetComponent<SpriteRenderer>();

                if (groundSR != null)
                {
                    groundSR.sortingOrder = 10;
                }

                if (mapSR != null)
                {
                    mapSR.sortingOrder = 20;
                }
                //追記　マップタイプがプレイヤーの場合
                //PlayerスクリプトのcurrentPosにposを代入する
                if (mapTable[x, y] == MAP_TYPE.PLAYER)
                {
                    startPos = pos;

                    player.currentPos = pos;
                    player.transform.localPosition = ScreenPos(pos);
                    player.mapGenerator = this;

                    Destroy(_map);
                }
            }
        }
    }
    public Vector2 ScreenPos(Vector2Int _pos)
    {
        return new Vector2(
            (_pos.x * mapSize - centerPos.x) * miniMapScale + miniMapOffset.x,
            (-(_pos.y * mapSize - centerPos.y)) * miniMapScale + miniMapOffset.y
        );
    }

    void _loadMapData()
    {
        TextAsset currentMap = stages[currentStage].floors[currentFloor];

        string[] mapLines = currentMap.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        //行の数
        int row = mapLines.Length;
        //列の数
        int col = mapLines[0].Split(',').Length;
        //初期化
        mapTable = new MAP_TYPE[col, row];

        for (int y = 0; y < row; y++)
        {
            string[] mapValues = mapLines[y].Split(new char[] { ',' });
            for (int x = 0; x < col; x++)
            {
                mapTable[x, y] = (MAP_TYPE)int.Parse(mapValues[x]);
            }
        }
    }

    public Vector2Int FindStairPos()
    {
        for (int y = 0; y < mapTable.GetLength(1); y++)
        {
            for (int x = 0; x < mapTable.GetLength(0); x++)
            {
                if (mapTable[x, y] == MAP_TYPE.STAIR)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero;
    }

    bool puzzleSolved = false;
    bool puzzle2Solved = false;
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

    public void CheckStair()
    {
        if (GetNextMapType(player.currentPos) == MAP_TYPE.STAIR)
        {
            Panel.SetActive(true);
        }
    }

    public void OpenPuzzle()
    {
        if (puzzleSolved)
            return;

        Puzzle.SetActive(true);
        player.isPuzzle = true;
    }

    public void OpenPuzzle2()
    {
        if (puzzle2Solved)
            return;

        Puzzle2.SetActive(true);
        player.isPuzzle = true;
    }
    
    public void Yes()
    {
        Panel.SetActive(false);

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
    }

    public void ChangeFloor(int floor, bool moveToStair = true)
    {
        Debug.Log("ChangeFloor開始：" + floor);

        if (floor < 0 || floor >= stages[currentStage].floors.Length)
        {
            Debug.Log("階数が範囲外！");
            return;
        }

        currentFloor = floor;

        Debug.Log("currentFloor変更：" + currentFloor);

        while (map2D.childCount > 0)
        {
            DestroyImmediate(map2D.GetChild(0).gameObject);
        }

        Debug.Log("古いマップ削除完了");

        _loadMapData();

        Debug.Log("マップ読み込み完了");

        _createMap();

        Debug.Log("新しいマップ作成完了");

        if (moveToStair)
        {
            Vector2Int stairPos = FindStairPos();

            player.currentPos = stairPos;
            player.transform.localPosition = ScreenPos(stairPos);
        }

        Debug.Log("ChangeFloor終了");
    }

    public void ChangeStage(int stage)
    {
        currentStage = stage;
        currentFloor = 0;

        while (map2D.childCount > 0)
        {
            DestroyImmediate(map2D.GetChild(0).gameObject);
        }

        _loadMapData();
        _createMap();
    }

    public int CurrentFloor
    {
        get { return currentFloor; }
    }
}