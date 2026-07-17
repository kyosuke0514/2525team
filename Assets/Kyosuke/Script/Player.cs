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
        LEFT
    }

    public DIRECTION direction;
    public Vector2Int currentPos, nextPos;

    public MapGenerator mapGenerator;

    int[,] move =
    {
        {0, -1},
        {1, 0},
        {0, 1},
        {-1, 0}
    };
    private void Start()
    {
        mapGenerator = transform.parent.GetComponent<MapGenerator>();
        direction = DIRECTION.DOWN;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = DIRECTION.TOP;
            _move();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = DIRECTION.RIGHT;
            _move();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = DIRECTION.DOWN;
            _move();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = DIRECTION.LEFT;
            _move();
        }
    }
    void _move()
    {
        nextPos = currentPos + new Vector2Int(move[(int)direction, 0], move[(int)direction, 1]);
        transform.localPosition = mapGenerator.ScreenPos(nextPos);
        currentPos = nextPos;
    }
}