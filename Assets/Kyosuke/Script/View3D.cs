using UnityEngine;

public class View3D : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] GameObject FrontWall;
    [SerializeField] GameObject LeftWall;
    [SerializeField] GameObject RightWall;
    void Update()
    {
        UpdateView();
    }

    void UpdateView()
    {
        Vector2Int leftFront = GetMapPos(1, -1);
        Vector2Int front = GetMapPos(1, 0);
        Vector2Int rightFront = GetMapPos(1, 1);

        // ê≥ñ 
        FrontWall.SetActive(
            mapGenerator.GetNextMapType(front) == MapGenerator.MAP_TYPE.WALL);

        // ç∂
        LeftWall.SetActive(
            mapGenerator.GetNextMapType(leftFront) == MapGenerator.MAP_TYPE.WALL);

        // âE
        RightWall.SetActive(
            mapGenerator.GetNextMapType(rightFront) == MapGenerator.MAP_TYPE.WALL);
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
