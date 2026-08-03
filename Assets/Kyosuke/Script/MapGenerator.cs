using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //map.txtを読み込む
    [SerializeField] TextAsset map1F;
    [SerializeField] TextAsset map2F;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] Transform map2D;
    [SerializeField] float miniMapScale = 0.3f;
    [SerializeField] Vector2 miniMapOffset = new Vector2(-7.4f, -3.5f);

    Vector2 centerPos;

    int currentFloor = 1;

    public Player player;

    float mapSize;

    public enum MAP_TYPE
    {
        GROUND, //0
        WALL,   //1
        PLAYER
    }
    MAP_TYPE[,] mapTable;

    public MAP_TYPE GetNextMapType(Vector2Int _pos)
    {
        return mapTable[_pos.x, _pos.y];
    }
    private void Start()
    {
        _loadMapData();
        _createMap();

        Invoke(nameof(TestFloor), 3f);
    }

    void TestFloor()
    {
        ChangeFloor(2);
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

                //追記　マップタイプがプレイヤーの場合
                //PlayerスクリプトのcurrentPosにposを代入する
                if (mapTable[x, y] == MAP_TYPE.PLAYER)
                {
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
        TextAsset currentMap;

        if (currentFloor == 1)
        {
            currentMap = map1F;
        }
        else
        {
            currentMap = map2F;
        }

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

    public void ChangeFloor(int floor)
    {

        List<Transform> children = new List<Transform>();

        foreach (Transform child in map2D)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            Destroy(child.gameObject);
        }

        _loadMapData();
        _createMap();
    }
}