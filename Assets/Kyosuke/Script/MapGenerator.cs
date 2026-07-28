using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //map.txtを読み込む
    [SerializeField] TextAsset mapText;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] Transform map2D;

    Vector2 centerPos;

    float mapSize;

    public enum MAP_TYPE
    {
        GROUND, //0
        WALL,   //1
        PLAYER  //2
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
        map2D.position = new Vector3(5f, 0);
    }
    void _createMap()
    {
        mapSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

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
                GameObject _ground = Instantiate(prefabs[(int)MAP_TYPE.GROUND], transform);
                GameObject _map = Instantiate(prefabs[(int)mapTable[x, y]]);
                _ground.transform.position = ScreenPos(pos);
                _map.transform.position = ScreenPos(pos);

                //追記　マップタイプがプレイヤーの場合
                //PlayerスクリプトのcurrentPosにposを代入する
                if (mapTable[x, y] == MAP_TYPE.PLAYER)
                {
                    _map.GetComponent<Player>().currentPos = pos;

                }
            }
        }
    }
    public Vector2 ScreenPos(Vector2Int _pos)
    {
        return new Vector2(
             _pos.x * mapSize - centerPos.x,
             -(_pos.y * mapSize - centerPos.y));
    }

    void _loadMapData()
    {
        string[] mapLines = mapText.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        //行の数
        int row = mapLines.Length;
        //列の数
        int col = mapLines[0].Split(new char[] { ',' }).Length;
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
}