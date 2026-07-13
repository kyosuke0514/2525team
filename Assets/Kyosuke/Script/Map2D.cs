using UnityEngine;

public class Map2D : MonoBehaviour
{
    [SerializeField] private GameObject WallPrefab;
    [SerializeField] private GameObject FloorPrefab;
    [SerializeField] private Transform mapRoot;

    private float cellSize = 1f;

    [SerializeField] private Player player;
   
    private int width = 20;
    private int height = 20;

    //0 = ’Ê˜H
    //1 = •Ç
    private int[,] map;
    private bool[,] discovered;

    void Start()
    {
        map = new int[height,width];
        discovered = new bool[height, width];

        CreateMap();

        discovered[player.playerY,player.playerX] = true;
    }

    void CreateMap()
    {
        // ‘S•”•Ç
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[y, x] = 1;
            }
        }

        // ’Ê˜H‚ğì‚éi‰¼j
        for (int x = 1; x <= 5; x++)
        {
            map[1, x] = 0;
        }

        for (int y = 1; y <= 5; y++)
        {
            map[y, 5] = 0;
        }

        //Œ©‚½–Ú¶¬
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject obj;

                if (map[y, x] == 1)
                {
                    obj = Instantiate(WallPrefab, mapRoot);
                }
                else
                {
                    obj = Instantiate(FloorPrefab, mapRoot);
                }

                obj.transform.localPosition = new Vector3(x * cellSize, -y * cellSize, 0);
            }
        }
    }

    //•Ç‚©•Ç‚¶‚á‚È‚¢‚©
    public bool IsWall(int x, int y)
    {
        if (x < 0 || x >= width ||y < 0 || y >= height)
        {
            return true;
        }
        return map[y, x] == 1;
    }

    //’TõÏ‚İ‚É‚·‚é
    public void Discover(int x, int y)
    {
        discovered[y, x] = true;
    }

    //’TõÏ‚İ‚©’²‚×‚é
    public bool IsDiscovered(int x, int y)
    {
        return discovered[y, x];
    }
}


