using UnityEngine;

public class View3D : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    [SerializeField] GameObject nearFront;
    [SerializeField] GameObject nearLeft;
    [SerializeField] GameObject nearRight;
    [SerializeField] GameObject nearLeftPath;
    [SerializeField] GameObject nearRightPath;

    [SerializeField] GameObject midFront;
    [SerializeField] GameObject midLeft;
    [SerializeField] GameObject midRight;
    [SerializeField] GameObject midLeftPath;
    [SerializeField] GameObject midRightPath;

    [SerializeField] GameObject farFront;
    [SerializeField] GameObject farLeft;
    [SerializeField] GameObject farRight;
    [SerializeField] GameObject farLeftPath;
    [SerializeField] GameObject farRightPath;
    void Update()
    {
        UpdateView();
    }

    void UpdateView()
    {
        // 1マス先
        Vector2Int nearL = GetMapPos(1, -1);
        Vector2Int nearF = GetMapPos(1, 0);
        Vector2Int nearR = GetMapPos(1, 1);

        // 2マス先
        Vector2Int midL = GetMapPos(2, -1);
        Vector2Int midF = GetMapPos(2, 0);
        Vector2Int midR = GetMapPos(2, 1);

        // 3マス先
        Vector2Int farL = GetMapPos(3, -1);
        Vector2Int farF = GetMapPos(3, 0);
        Vector2Int farR = GetMapPos(3, 1);

        // ===== 壁判定 =====
        bool nearLeftWall = mapGenerator.GetNextMapType(nearL) == MapGenerator.MAP_TYPE.WALL;
        bool nearFrontWall = mapGenerator.GetNextMapType(nearF) == MapGenerator.MAP_TYPE.WALL;
        bool nearRightWall = mapGenerator.GetNextMapType(nearR) == MapGenerator.MAP_TYPE.WALL;

        bool midLeftWall = mapGenerator.GetNextMapType(midL) == MapGenerator.MAP_TYPE.WALL;
        bool midFrontWall = mapGenerator.GetNextMapType(midF) == MapGenerator.MAP_TYPE.WALL;
        bool midRightWall = mapGenerator.GetNextMapType(midR) == MapGenerator.MAP_TYPE.WALL;

        bool farLeftWall = mapGenerator.GetNextMapType(farL) == MapGenerator.MAP_TYPE.WALL;
        bool farFrontWall = mapGenerator.GetNextMapType(farF) == MapGenerator.MAP_TYPE.WALL;
        bool farRightWall = mapGenerator.GetNextMapType(farR) == MapGenerator.MAP_TYPE.WALL;

        // ===== Near =====
        nearLeft.SetActive(nearLeftWall);
        nearFront.SetActive(nearFrontWall);
        nearRight.SetActive(nearRightWall);

        // ===== Mid =====
        midLeft.SetActive(midLeftWall);
        midFront.SetActive(midFrontWall);
        midRight.SetActive(midRightWall);

        // ===== Far =====
        farLeft.SetActive(farLeftWall);
        farFront.SetActive(farFrontWall);
        farRight.SetActive(farRightWall);

        // ===== 左右の通路 =====

        nearLeftPath.SetActive(!nearLeftWall);
        nearRightPath.SetActive(!nearRightWall);

        midLeftPath.SetActive(!midLeftWall);
        midRightPath.SetActive(!midRightWall);

        farLeftPath.SetActive(!farLeftWall);
        farRightPath.SetActive(!farRightWall);
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
