using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
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

    void Start()
    {
        _loadMapData();
        _cleateMap();
        map2D.position = new Vector3(5f, 0);
    }

    void _cleateMap()
    {
        mapSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;

        //列が偶数の場合
        if(mapTable.GetLength(0)%2 ==0)
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize;
        }
        //行が偶数の場合
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

                _ground.transform.position = ScreenPos(pos);
                _map.transform.position = ScreenPos(pos);

                if (x == 1 && y == 1)
                {

                    //修正　プレイヤー用の変数名を変更＆親要素をmap2Dに変更
                    GameObject _player = Instantiate(prefabs[2], map2D);
                    _player.transform.position = ScreenPos(pos);
                    _player.GetComponent<Player>().currentPos = pos;

                    //修正　親要素がmapGeneratorからmap2Dに変わるので修正　（Player.csも修正する）
                    _player.GetComponent<Player>().mapGenerator = this;
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
        mapTable = new MAP_TYPE[col,row];

        for (int y = 0; y < row; y++)
        {
            string[] mapValues = mapLines[y].Split(new char[] { ',' });
            for (int x = 0; x < col; x++)
            {
                mapTable[x,y] = (MAP_TYPE)int.Parse(mapValues[x]);
            }
        }
    }
}