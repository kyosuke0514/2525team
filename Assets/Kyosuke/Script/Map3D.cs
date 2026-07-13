using UnityEngine;

public class Map3D : MonoBehaviour
{
    [SerializeField] private Map2D map2D;
    [SerializeField] private Player player;

    [SerializeField] private GameObject frontWall;

    void Update()
    {
        int checkX = player.playerX;
        int checkY = player.playerY;

        switch (player.direction)
        {
            case Player.DIRECTION.UP:
                checkY--;
                break;

            case Player.DIRECTION.LEFT:
                checkX--;
                break;

            case Player.DIRECTION.DOWN:
                checkY++;
                break;

            case Player.DIRECTION.RIGHT:
                checkX++;
                break;
        }

        frontWall.SetActive(
            map2D.IsWall(checkX, checkY)
        );
    }
}
