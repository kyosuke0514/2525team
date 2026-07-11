using UnityEngine;

public class Map2D : MonoBehaviour
{
    [SerializeField] private Player player;
   
    private int width = 20;
    private int height = 20;

    private int[,] map;
    private bool[,] discoverd;

    void Start()
    {
        map = new int[width, height];
        discoverd = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[y, x] = 1;
                }else
                {
                    map[y, x] = 0;
                }
            }
        }
        discoverd[player.playerY, player.playerX] = true;
    }
}
