using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Map2D map2D;

    public int playerX = 1;
    public int playerY = 1;
    //•ûŒü
    public enum DIRECTION
    {
        UP,
        LEFT,
        DOWN,
        RIGHT,
        MAX
    }
    public DIRECTION direction = DIRECTION.RIGHT;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            int nextX = playerX;
            int nextY = playerY;

            //ˆÚ“®
            switch (direction)
            {
                case DIRECTION.UP:
                    nextY--;
                    break;

                case DIRECTION.LEFT:
                    nextX--;
                    break;

                case DIRECTION.DOWN:
                    nextY++;
                    break;

                case DIRECTION.RIGHT:
                    nextX++;
                    break;
            }

            //•Ç‚¶‚á‚È‚¯‚ê‚ÎˆÚ“®
            if (!map2D.IsWall(nextX, nextY))
            {
                playerX = nextX;
                playerY = nextY;

                map2D.Discover(playerX, playerY);
            }
            Debug.Log($"X:{playerX} Y:{playerY}");
        }

        transform.localPosition =
        new Vector3(playerX, -playerY, 0);


    }
}
