using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private HP hp;

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

    public int maxHP = 5;
    public int currentHP;

    int[,] move =
    {
        {0, -1},
        {1, 0},
        {0, 1},
        {-1, 0}
    };

    [SerializeField] Transform directionArrow;
    Vector3[] arrowPositions = new[] { new Vector3(0, 0.5f), new Vector3(0.5f, 0), new Vector3(0f, -0.5f), new Vector3(-0.5f, 0f) };

    public MapGenerator mapGenerator;
    private void Start()
    {
        direction = DIRECTION.DOWN;
        currentHP = maxHP;

        hp.Update();

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
        directionArrow.localPosition = arrowPositions[(int)direction];
    }
    void _move(int dir)
    {
        nextPos = currentPos + new Vector2Int(
            move[(int)direction, 0] * dir,
            move[(int)direction, 1] * dir);

        Debug.Log(nextPos);

        if (mapGenerator.GetNextMapType(nextPos) != MapGenerator.MAP_TYPE.WALL)
        {
            Debug.Log("壁じゃない");

            currentPos = nextPos;

            Debug.Log("現在位置：" + currentPos);

            transform.localPosition = mapGenerator.ScreenPos(currentPos);

            CheckEvent();
        }
    }

    

    public HP hP;
    public void Damage(int damage)
    {
        currentHP -= damage;

        Debug.Log("HP : " + currentHP);

        if (currentHP < 0)
            currentHP = 0;

        hp.Update();

        if (currentHP <= 0)
        {
            Debug.Log("ゲームオーバー");
        }
    }

    void CheckEvent()
    {
        MapGenerator.MAP_TYPE type = mapGenerator.GetNextMapType(currentPos);

        if (type == MapGenerator.MAP_TYPE.STAIR)
        {
            if (mapGenerator.CurrentFloor == 0)
                mapGenerator.ChangeFloor(1);
            else
                mapGenerator.ChangeFloor(0);
        }

        if (type == MapGenerator.MAP_TYPE.GOAL)
        {
            Debug.Log("ステージクリア！");
        }

        if (type == MapGenerator.MAP_TYPE.PIT)
        {
            Debug.Log("落とし穴！");

            // ダメージを受ける
            Damage(1);

            // 今いる座標を保存
            Vector2Int fallPos = currentPos;

            // 2Fなら1Fへ落ちる
            if (mapGenerator.CurrentFloor == 1)
            {
                // 階段へ移動しないで1Fへ
                mapGenerator.ChangeFloor(0, false);

                // 保存した座標へ移動
                currentPos = fallPos;
                transform.localPosition = mapGenerator.ScreenPos(currentPos);
            }
            else
            {
                currentPos = mapGenerator.startPos;
                transform.localPosition = mapGenerator.ScreenPos(currentPos);
            }
        }
        
    }

    
}