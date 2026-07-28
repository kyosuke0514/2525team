using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum DIRECTION
    {
        TOP,
        RIGHT,
        DOWN,
        LEFT,
        MAX
    }

    public DIRECTION direction;
    public Vector2Int currentPos, nextPos;



    int[,] move =
    {
        {0, -1},
        {1, 0},
        {0, 1},
        {-1, 0}
    };

    [SerializeField] Transform directionArrow;
    Vector3[] arrowPositions = new[] { new Vector3(0, 0.35f), new Vector3(0.35f, 0), new Vector3(0f, -0.35f), new Vector3(-0.35f, 0f) };

    public MapGenerator mapGenerator;
    private void Start()
    {
        //mapGenerator = transform.parent.GetComponent<MapGenerator>();
        direction = DIRECTION.DOWN;
        _viewArrow();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _move(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction++;
            _setDirection();
            _viewArrow();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _move(-1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            direction--;
            _setDirection();
            _viewArrow();
        }
    }
    //追加　向きを取得
    void _setDirection()
    {
        int d = ((int)direction + (int)DIRECTION.MAX) % (int)DIRECTION.MAX;
        direction = (DIRECTION)d;

    }

    //追加　矢印の位置を変更
    void _viewArrow()
    {
        //if (directionArrow == null)
        //{
        //    return;
        //}
        directionArrow.localPosition = arrowPositions[(int)direction];
    }
    void _move(int dir)
    {
        nextPos = currentPos + new Vector2Int(move[(int)direction, 0] * dir, move[(int)direction, 1] * dir);
        if (mapGenerator.GetNextMapType(nextPos) != MapGenerator.MAP_TYPE.WALL)
        {
            transform.localPosition = mapGenerator.ScreenPos(nextPos);
            currentPos = nextPos;
        }
    }
}