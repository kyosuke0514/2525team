using UnityEngine;

public class View3D : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    //==================================================
    // Near：1マス先
    //==================================================

    [SerializeField] GameObject nearFront;
    [SerializeField] GameObject nearLeft;
    [SerializeField] GameObject nearRight;
    [SerializeField] GameObject nearLeftPath;
    [SerializeField] GameObject nearRightPath;
    [SerializeField] GameObject nearLeft2;
    [SerializeField] GameObject nearRight2;

    //==================================================
    // Mid：2マス先
    //==================================================

    [SerializeField] GameObject midFront;
    [SerializeField] GameObject midLeft;
    [SerializeField] GameObject midRight;
    [SerializeField] GameObject midLeftPath;
    [SerializeField] GameObject midRightPath;
    [SerializeField] GameObject midLeft2;
    [SerializeField] GameObject midRight2;

    //==================================================
    // Far：3マス先
    //==================================================

    [SerializeField] GameObject farFront;
    [SerializeField] GameObject farLeft;
    [SerializeField] GameObject farRight;
    [SerializeField] GameObject farLeftPath;
    [SerializeField] GameObject farRightPath;
    [SerializeField] GameObject farLeft2;
    [SerializeField] GameObject farRight2;


    void Update()
    {
        UpdateView();
    }


    void UpdateView()
    {

        //==================================================
        // 各距離の座標を取得
        //==================================================

        // Near：1マス先

        // 正面
        Vector2Int nearF = GetMapPos(1, 0);

        // プレイヤーのすぐ左右
        Vector2Int nearL = GetMapPos(0, -1);
        Vector2Int nearR = GetMapPos(0, 1);

        // 左右奥
        Vector2Int nearLL = GetMapPos(1, -2);
        Vector2Int nearRR = GetMapPos(1, 2);


        // Mid：2マス先
        Vector2Int midF = GetMapPos(2, 0);
        Vector2Int midLL = GetMapPos(2, -2);
        Vector2Int midRR = GetMapPos(2, 2);


        // Far：3マス先
        Vector2Int farF = GetMapPos(3, 0);
        Vector2Int farLL = GetMapPos(3, -2);
        Vector2Int farRR = GetMapPos(3, 2);


        //==================================================
        // 各座標が壁かどうかを判定
        //==================================================

        // Near
        bool nearLeftWall = IsWall(nearL);
        bool nearFrontWall = IsWall(nearF);
        bool nearRightWall = IsWall(nearR);
        bool nearLeft2Wall = IsWall(nearLL);
        bool nearRight2Wall = IsWall(nearRR);

        // Mid
        bool midFrontWall = IsWall(midF);
        bool midLeft2Wall = IsWall(midLL);
        bool midRight2Wall = IsWall(midRR);

        // Far
        bool farFrontWall = IsWall(farF);
        bool farLeft2Wall = IsWall(farLL);
        bool farRight2Wall = IsWall(farRR);


        //==================================================
        // Near：1マス先の表示
        //==================================================

        // プレイヤーのすぐ左右
        nearLeft.SetActive(nearLeftWall);
        nearRight.SetActive(nearRightWall);

        // 正面
        nearFront.SetActive(nearFrontWall);

        // 正面の壁に対応する通路
        nearLeftPath.SetActive(nearFrontWall);
        nearRightPath.SetActive(nearFrontWall);

        // 左右奥
        nearLeft2.SetActive(nearLeft2Wall);
        nearRight2.SetActive(nearRight2Wall);


        //==================================================
        // Mid：2マス先の表示
        //==================================================

        // 正面
        midFront.SetActive(midFrontWall);

        // 正面の壁に対応する通路
        midLeftPath.SetActive(midFrontWall);
        midRightPath.SetActive(midFrontWall);

        // 左右奥
        midLeft2.SetActive(midLeft2Wall);
        midRight2.SetActive(midRight2Wall);


        //==================================================
        // Far：3マス先の表示
        //==================================================

        // 正面
        farFront.SetActive(farFrontWall);

        // 正面の壁に対応する通路
        farLeftPath.SetActive(farFrontWall);
        farRightPath.SetActive(farFrontWall);

        // 左右奥
        farLeft2.SetActive(farLeft2Wall);
        farRight2.SetActive(farRight2Wall);
    }


    //==================================================
    // 指定した座標が壁かどうかを判定
    //==================================================

    bool IsWall(Vector2Int pos)
    {
        return mapGenerator.GetNextMapType(pos)
            == MapGenerator.MAP_TYPE.WALL;
    }


    //==================================================
    // プレイヤーから見たマップ座標を取得
    //==================================================

    Vector2Int GetMapPos(int forward, int side)
    {
        Player player = mapGenerator.player;
        Vector2Int pos = player.currentPos;

        switch (player.direction)
        {
            case Player.DIRECTION.TOP:
                return pos + new Vector2Int(side, -forward);

            case Player.DIRECTION.RIGHT:
                return pos + new Vector2Int(forward, side);

            case Player.DIRECTION.DOWN:
                return pos + new Vector2Int(-side, forward);

            case Player.DIRECTION.LEFT:
                return pos + new Vector2Int(-forward, -side);
        }

        return pos;
    }
}