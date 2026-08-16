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

    public bool isPuzzle = false;

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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 30;
        }

        hp.Update();
        _viewArrow();
    }

    private void Update()
    {
        
        
        if (isPuzzle)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            _move(1);
            Debug.Log("Wきた！");
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
        SpriteRenderer arrowSR = directionArrow.GetComponent<SpriteRenderer>();

        if (arrowSR != null)
        {
            arrowSR.sortingOrder = 31;
        }
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

        if (type == MapGenerator.MAP_TYPE.PUZZLE)
        {
            mapGenerator.OpenPuzzle();
        }

        if (type == MapGenerator.MAP_TYPE.PUZZLE2)
        {
            mapGenerator.OpenPuzzle2();
        }

        if (type == MapGenerator.MAP_TYPE.STAIR)
        {
            mapGenerator.CheckStair();
        }

        if (type == MapGenerator.MAP_TYPE.GOAL)
        {
            Debug.Log("ステージクリア！");
        }

        if (type == MapGenerator.MAP_TYPE.PIT)
        {
            Debug.Log("落とし穴に落ちた！");

            Damage(1);

            if (mapGenerator.CurrentFloor == 1)
            {
                Debug.Log("2F → 1F");

                mapGenerator.ChangeFloor(0, false);

                Debug.Log("1Fへ移動完了！");
            }
            else
            {
                Debug.Log("1F → スタート地点");

                currentPos = mapGenerator.startPos;
                transform.localPosition = mapGenerator.ScreenPos(currentPos);
            }
        }
    }
}