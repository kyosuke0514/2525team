using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2D : MonoBehaviour
{
    [SerializeField] TextAsset mapText;     //マップデータテキスト
    [SerializeField] GameObject[] prefabs;  //壁床プレハブ
    [SerializeField] Transform map2D;       //マップまとめる親オブジェ

    private bool[,] discoverd;
    private int width = 20;
    private int height = 20;

    private int playerY = 1;
    private int playerX = 1;

    void Start()
    {
        discoverd = new bool[width, height];
        discoverd[playerY, playerX] = true;
    }

}
