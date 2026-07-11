using UnityEngine;

public class Player : MonoBehaviour
{
    //•ûŒü
    public enum DIRECTION
    {
        UP,
        LEFT,
        DOWN,
        RIGHT,
        MAX
    }

    public DIRECTION direction;
    public Vector2Int currentPos, nextPos;
    int[,] move = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
}
