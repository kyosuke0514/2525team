using UnityEngine;

public class View3D : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    //==================================================
    // Near
    //==================================================

    [SerializeField] GameObject nearFront;
    [SerializeField] GameObject nearLeft;
    [SerializeField] GameObject nearRight;
    [SerializeField] GameObject nearLeftPath;
    [SerializeField] GameObject nearRightPath;
    [SerializeField] GameObject nearLeft2;
    [SerializeField] GameObject nearRight2;
    [SerializeField] GameObject nearStair;

    //==================================================
    // Mid
    //==================================================

    [SerializeField] GameObject midFront;
    [SerializeField] GameObject midLeft;
    [SerializeField] GameObject midRight;
    [SerializeField] GameObject midLeftPath;
    [SerializeField] GameObject midRightPath;
    [SerializeField] GameObject midLeft2;
    [SerializeField] GameObject midRight2;
    [SerializeField] GameObject midStair;

    //==================================================
    // Far
    //==================================================

    [SerializeField] GameObject farFront;
    [SerializeField] GameObject farLeft;
    [SerializeField] GameObject farRight;
    [SerializeField] GameObject farLeftPath;
    [SerializeField] GameObject farRightPath;
    [SerializeField] GameObject farLeft2;
    [SerializeField] GameObject farRight2;
    [SerializeField] GameObject farStair;

    void Update()
    {
        UpdateView();
    }


    //==================================================
    // 3D表示を更新
    //==================================================

    void UpdateView()
    {
        // 最初に全部消す
        HideAll();


        //==================================================
        // 1マス前後
        //==================================================

        // (-2,0) 左2
        if (IsWall(GetMapPos(0, -2)))
        {
            nearLeft2.SetActive(true);
        }

        // (-1,0) 左
        if (IsWall(GetMapPos(0, -1)))
        {
            nearLeft.SetActive(true);
        }

        // (1,0) 右
        if (IsWall(GetMapPos(0, 1)))
        {
            nearRight.SetActive(true);
        }

        // (2,0) 右2
        if (IsWall(GetMapPos(0, 2)))
        {
            nearRight2.SetActive(true);
        }


        //==================================================
        // 1マス前
        //==================================================

        // (-1,1)
        // NearLeftPath + MidLeft
        if (IsWall(GetMapPos(1, -1)))
        {
            nearLeftPath.SetActive(true);
            midLeft.SetActive(true);
        }

        // (0,1)
        // NearFront
        if (IsWall(GetMapPos(1, 0)))
        {
            nearFront.SetActive(true);
        }

        // 階段
        if (IsStair(GetMapPos(1, 0)))
        {
            nearStair.SetActive(true);
        }

        // (1,1)
        // NearRightPath + MidRight
        if (IsWall(GetMapPos(1, 1)))
        {
            nearRightPath.SetActive(true);
            midRight.SetActive(true);
        }

        // (-2,1)
        // MidLeft2
        if (IsWall(GetMapPos(1, -2)))
        {
            midLeft2.SetActive(true);
        }

        // (2,1)
        // MidRight2
        if (IsWall(GetMapPos(1, 2)))
        {
            midRight2.SetActive(true);
        }


        //==================================================
        // 2マス前
        //==================================================

        // (-1,2)
        // MidLeftPath + FarLeft
        if (IsWall(GetMapPos(2, -1)))
        {
            midLeftPath.SetActive(true);
            farLeft.SetActive(true);
        }

        // (0,2)
        // MidFront
        if (IsWall(GetMapPos(2, 0)))
        {
            midFront.SetActive(true);
        }

        // 階段
        if (IsStair(GetMapPos(2, 0)))
        {
            midStair.SetActive(true);
        }

        // (1,2)
        // MidRightPath + FarRight
        if (IsWall(GetMapPos(2, 1)))
        {
            midRightPath.SetActive(true);
            farRight.SetActive(true);
        }

        // (-2,2)
        // FarLeft2
        if (IsWall(GetMapPos(2, -2)))
        {
            farLeft2.SetActive(true);
        }

        // (2,2)
        // FarRight2
        if (IsWall(GetMapPos(2, 2)))
        {
            farRight2.SetActive(true);
        }


        //==================================================
        // 3マス前
        //==================================================

        // (-1,3)
        // FarLeftPath
        if (IsWall(GetMapPos(3, -1)))
        {
            farLeftPath.SetActive(true);
        }

        // (0,3)
        // FarFront
        if (IsWall(GetMapPos(3, 0)))
        {
            farFront.SetActive(true);
        }

        // 階段
        if (IsStair(GetMapPos(3, 0)))
        {
            farStair.SetActive(true);
        }

        // (1,3)
        // FarRightPath
        if (IsWall(GetMapPos(3, 1)))
        {
            farRightPath.SetActive(true);
        }

        //==================================================
        // 壁が近い場合、2枚目の壁を消す
        //==================================================

        // (-1,0) に壁がある
        bool leftWall = IsWall(GetMapPos(0, -1));

        // (1,0) に壁がある
        bool rightWall = IsWall(GetMapPos(0, 1));

        // (-1,1) に壁がある
        bool leftPathWall = IsWall(GetMapPos(1, -1));

        // (1,1) に壁がある
        bool rightPathWall = IsWall(GetMapPos(1, 1));


        // 左側
        if (leftWall || leftPathWall)
        {
            nearLeft2.SetActive(false);
        }

        if (leftPathWall)
        {
            midLeft2.SetActive(false);
        }


        // 右側
        if (rightWall || rightPathWall)
        {
            nearRight2.SetActive(false);
        }

        if (rightPathWall)
        {
            midRight2.SetActive(false);
        }

        //==================================================
        // 手前の壁がある場合、奥のPathを消す
        //==================================================

        // -------------------------
        // 左側
        // -------------------------

        // (-1,0) に壁がある
        if (nearLeft.activeSelf)
        {
            // (-1,1) のPathを消す
            nearLeftPath.SetActive(false);
        }

        // (-1,1) に壁がある
        if (midLeft.activeSelf)
        {
            // (-1,2) のPathを消す
            midLeftPath.SetActive(false);
        }

        // (-1,2) に壁がある
        if (farLeft.activeSelf)
        {
            // (-1,3) のPathを消す
            farLeftPath.SetActive(false);
        }


        // -------------------------
        // 右側
        // -------------------------

        // (1,0) に壁がある
        if (nearRight.activeSelf)
        {
            // (1,1) のPathを消す
            nearRightPath.SetActive(false);
        }

        // (1,1) に壁がある
        if (midRight.activeSelf)
        {
            // (1,2) のPathを消す
            midRightPath.SetActive(false);
        }

        // (1,2) に壁がある
        if (farRight.activeSelf)
        {
            // (1,3) のPathを消す
            farRightPath.SetActive(false);
        }

        //==================================================
        // MidPathが表示されている場合、左右の2枚目を全部消す
        //==================================================

        // 左のMidPathが表示されている
        if (midLeftPath.activeSelf)
        {
            nearLeft2.SetActive(false);
            midLeft2.SetActive(false);
            farLeft2.SetActive(false);
        }

        // 右のMidPathが表示されている
        if (midRightPath.activeSelf)
        {
            nearRight2.SetActive(false);
            midRight2.SetActive(false);
            farRight2.SetActive(false);
        }

    }


    //==================================================
    // 全オブジェクトを非表示
    //==================================================

    void HideAll()
    {
        nearFront.SetActive(false);
        nearLeft.SetActive(false);
        nearRight.SetActive(false);
        nearLeftPath.SetActive(false);
        nearRightPath.SetActive(false);
        nearLeft2.SetActive(false);
        nearRight2.SetActive(false);
        nearStair.SetActive(false);

        midFront.SetActive(false);
        midLeft.SetActive(false);
        midRight.SetActive(false);
        midLeftPath.SetActive(false);
        midRightPath.SetActive(false);
        midLeft2.SetActive(false);
        midRight2.SetActive(false);
        midStair.SetActive(false);

        farFront.SetActive(false);
        farLeft.SetActive(false);
        farRight.SetActive(false);
        farLeftPath.SetActive(false);
        farRightPath.SetActive(false);
        farLeft2.SetActive(false);
        farRight2.SetActive(false);
        farStair.SetActive(false);
    }


    //==================================================
    // 指定座標が壁か判定
    //==================================================

    bool IsWall(Vector2Int pos)
    {
        return mapGenerator.GetNextMapType(pos)
            == MapGenerator.MAP_TYPE.WALL;
    }

    bool IsStair(Vector2Int pos)
    {
        MapGenerator.MAP_TYPE type = mapGenerator.GetNextMapType(pos);

        return type == MapGenerator.MAP_TYPE.STAIR_1_2
            || type == MapGenerator.MAP_TYPE.STAIR_2_3
            || type == MapGenerator.MAP_TYPE.STAIR_3_4;
    }

    //==================================================
    // プレイヤーから見た座標を取得
    //
    // forward = 前方向
    // side    = 左右方向
    //
    // 例：
    // GetMapPos(1, -1)
    // → 左前1マス
    //
    // GetMapPos(2, 1)
    // → 右前2マス
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