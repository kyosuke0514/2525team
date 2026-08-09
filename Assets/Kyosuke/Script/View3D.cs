using UnityEngine;

public class View3D : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    [SerializeField] GameObject nearFront;
    [SerializeField] GameObject nearLeft;
    [SerializeField] GameObject nearRight;
    [SerializeField] GameObject nearLeftPath;
    [SerializeField] GameObject nearRightPath;
    [SerializeField] GameObject nearLeft2;
    [SerializeField] GameObject nearRight2;

    [SerializeField] GameObject midFront;
    [SerializeField] GameObject midLeft;
    [SerializeField] GameObject midRight;
    [SerializeField] GameObject midLeftPath;
    [SerializeField] GameObject midRightPath;
    [SerializeField] GameObject midLeft2;
    [SerializeField] GameObject midRight2;

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
        // 1マス先
        Vector2Int nearLL = GetMapPos(1, -2);
        Vector2Int nearL = GetMapPos(1, -1);
        Vector2Int nearF = GetMapPos(1, 0);
        Vector2Int nearR = GetMapPos(1, 1);
        Vector2Int nearRR = GetMapPos(1, 2);

        // 2マス先
        Vector2Int midLL = GetMapPos(2, -2);
        Vector2Int midL = GetMapPos(2, -1);
        Vector2Int midF = GetMapPos(2, 0);
        Vector2Int midR = GetMapPos(2, 1);
        Vector2Int midRR = GetMapPos(2, 2);

        // 3マス先
        Vector2Int farLL = GetMapPos(3, -2);
        Vector2Int farL = GetMapPos(3, -1);
        Vector2Int farF = GetMapPos(3, 0);
        Vector2Int farR = GetMapPos(3, 1);
        Vector2Int farRR = GetMapPos(3, 2);

        // 4マス先
        Vector2Int ultraLL = GetMapPos(4, -2);
        Vector2Int ultraL = GetMapPos(4, -1);
        Vector2Int ultraF = GetMapPos(4, 0);
        Vector2Int ultraR = GetMapPos(4, 1);
        Vector2Int ultraRR = GetMapPos(4, 2);

        // 5マス先
        Vector2Int extremeLL = GetMapPos(5, -2);
        Vector2Int extremeL = GetMapPos(5, -1);
        Vector2Int extremeF = GetMapPos(5, 0);
        Vector2Int extremeR = GetMapPos(5, 1);
        Vector2Int extremeRR = GetMapPos(5, 2);

        // ===== 壁判定 =====
        bool nearLeftWall = mapGenerator.GetNextMapType(nearL) == MapGenerator.MAP_TYPE.WALL;
        bool nearFrontWall = mapGenerator.GetNextMapType(nearF) == MapGenerator.MAP_TYPE.WALL;
        bool nearRightWall = mapGenerator.GetNextMapType(nearR) == MapGenerator.MAP_TYPE.WALL; 
        bool nearLeft2Wall = mapGenerator.GetNextMapType(nearLL) == MapGenerator.MAP_TYPE.WALL;
        bool nearRight2Wall = mapGenerator.GetNextMapType(nearRR) == MapGenerator.MAP_TYPE.WALL;

        bool midLeftWall = mapGenerator.GetNextMapType(midL) == MapGenerator.MAP_TYPE.WALL;
        bool midFrontWall = mapGenerator.GetNextMapType(midF) == MapGenerator.MAP_TYPE.WALL;
        bool midRightWall = mapGenerator.GetNextMapType(midR) == MapGenerator.MAP_TYPE.WALL;
        bool midLeft2Wall = mapGenerator.GetNextMapType(midLL) == MapGenerator.MAP_TYPE.WALL;
        bool midRight2Wall = mapGenerator.GetNextMapType(midRR) == MapGenerator.MAP_TYPE.WALL;

        bool farLeftWall = mapGenerator.GetNextMapType(farL) == MapGenerator.MAP_TYPE.WALL;
        bool farFrontWall = mapGenerator.GetNextMapType(farF) == MapGenerator.MAP_TYPE.WALL;
        bool farRightWall = mapGenerator.GetNextMapType(farR) == MapGenerator.MAP_TYPE.WALL;
        bool farLeft2Wall = mapGenerator.GetNextMapType(farLL) == MapGenerator.MAP_TYPE.WALL;
        bool farRight2Wall = mapGenerator.GetNextMapType(farRR) == MapGenerator.MAP_TYPE.WALL;


        // ===== Near =====
        nearLeft.SetActive(nearLeftWall);
        nearFront.SetActive(nearFrontWall);
        nearRight.SetActive(nearRightWall);
        nearLeft2.SetActive(nearLeft2Wall);
        nearRight2.SetActive(nearRight2Wall);

        // ===== Mid =====
        midLeft.SetActive(midLeftWall);
        midFront.SetActive(midFrontWall);
        midRight.SetActive(midRightWall);
        midLeft2.SetActive(midLeft2Wall);
        midRight2.SetActive(midRight2Wall);

        // ===== Far =====
        farLeft.SetActive(farLeftWall);
        farFront.SetActive(farFrontWall);
        farRight.SetActive(farRightWall);
        farLeft2.SetActive(farLeft2Wall);
        farRight2.SetActive(farRight2Wall);

        // ===== 左右の通路 =====

        nearLeftPath.SetActive(!nearLeftWall);
        nearRightPath.SetActive(!nearRightWall);

        midLeftPath.SetActive(!midLeftWall);
        midRightPath.SetActive(!midRightWall);

        farLeftPath.SetActive(!farLeftWall);
        farRightPath.SetActive(!farRightWall);

        bool openSpace =
    !nearLeftWall && !nearRightWall &&
    !midLeftWall && !midRightWall &&
    !farLeftWall && !farRightWall;

        if (openSpace)
        {
            nearLeft.SetActive(false);
            nearRight.SetActive(false);

            midLeft.SetActive(false);
            midRight.SetActive(false);

            farLeft.SetActive(false);
            farRight.SetActive(false);

            // ここでLeft2 / Right2を表示
            nearLeft2.SetActive(true);
            nearRight2.SetActive(true);

            midLeft2.SetActive(true);
            midRight2.SetActive(true);

            farLeft2.SetActive(true);
            farRight2.SetActive(true);
        }
        else
        {
            // 普通の通路ではLeft2 / Right2を消す
            nearLeft2.SetActive(false);
            nearRight2.SetActive(false);

            midLeft2.SetActive(false);
            midRight2.SetActive(false);

            farLeft2.SetActive(false);
            farRight2.SetActive(false);
        }

        bool leftPath =
    mapGenerator.GetNextMapType(GetMapPos(1, -1)) != MapGenerator.MAP_TYPE.WALL &&
    mapGenerator.GetNextMapType(GetMapPos(1, -2)) != MapGenerator.MAP_TYPE.WALL;
        bool rightPath =
    mapGenerator.GetNextMapType(GetMapPos(1, 1)) != MapGenerator.MAP_TYPE.WALL &&
    mapGenerator.GetNextMapType(GetMapPos(1, 2)) != MapGenerator.MAP_TYPE.WALL;
        if (leftPath)
        {
            Debug.Log("左に通路が続いている！");
        }

        if (rightPath)
        {
            Debug.Log("右に通路が続いている！");
        }

    }

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
