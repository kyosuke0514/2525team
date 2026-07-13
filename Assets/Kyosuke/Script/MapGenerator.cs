using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    [SerializeField] TextAsset mapText;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] private Transform mapRoot;

    //追加
    enum MAP_TYPE
    {
        GROUND, //0
        WALL,   //1
        PLAYER  //2
    }
    MAP_TYPE[,] mapTable;

   

    void _loadMapData()
    {
        string[] mapLines = mapText.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);


        int row = mapLines.Length;
        int col = mapLines[0].Split(new char[] { ',' }).Length;
        mapTable = new MAP_TYPE[col, row];

        //追加　行の数だけループ
        for (int y = 0; y < row; y++)
        {
            //1行をカンマ区切りで分割
            string[] mapValues = mapLines[y].Split(new char[] { ',' });
            //列の数だけループ
            for (int x = 0; x < col; x++)
            {
                //mapValuesのx番目をMAP_TYPEにキャストしてmapTable[x,y]番目に代入
                mapTable[x, y] = (MAP_TYPE)int.Parse(mapValues[x]);
            }
        }
    }

    void Start()
    {
        _loadMapData();
        _createMap();
    }

    void _createMap()
    {
        //mapTableの行のループ
        for (int y = 0; y < mapTable.GetLength(1); y++)
        {
            //mapTableの列のループ
            for (int x = 0; x < mapTable.GetLength(0); x++)
            {
                //prefabsの中のmapTable[x,y]に当たるものを生成
                GameObject _map = Instantiate(prefabs[(int)mapTable[x, y]]);
                //生成したゲームオブジェクトの位置を設定
                _map.transform.position = new Vector2(x, y);
            }
        }
    }
}