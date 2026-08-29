using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //==================================================
    // インスペクター設定
    //==================================================

    [SerializeField] private HP hp;
    [SerializeField] GameObject treasureChestImage;

    //==================================================
    // プレイヤーの向き
    //==================================================

    public enum DIRECTION
    {
        TOP,
        RIGHT,
        DOWN,
        LEFT,
        MAX
    }

    public DIRECTION direction;


    //==================================================
    // プレイヤーの位置
    //==================================================

    public Vector2Int currentPos;
    public Vector2Int nextPos;

    public MapGenerator mapGenerator;


    //==================================================
    // HP
    //==================================================

    public int maxHP = 5;
    public int currentHP;


    //==================================================
    // 状態
    //==================================================

    // 謎解き・パネル・全体マップ表示中など
    // trueの間はプレイヤーを操作できない
    public bool isPuzzle = false;


    //==================================================
    // 移動方向
    //==================================================

    int[,] move =
    {
        { 0, -1 },  // TOP
        { 1,  0 },  // RIGHT
        { 0,  1 },  // DOWN
        {-1,  0 }   // LEFT
    };




    //==================================================
    // 初期化
    //==================================================

    private void Start()
    {
        // 初期の向きを上にする
        direction = DIRECTION.TOP;

        // HPを最大値にする
        currentHP = maxHP;

        // プレイヤーをマップより前に表示
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.sortingOrder = 30;
        }

        // HP表示を更新
        hp.Update();
    }


    //==================================================
    // プレイヤー操作
    //==================================================

    private void Update()
    {
        Debug.Log("Player Update");

        // 謎解き・パネルなどが表示されている間は操作できない
        if (isPuzzle)
        {
            Debug.Log("Puzzle中なので移動できません");
            return;
        }

        // 前進
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = DIRECTION.TOP;
            _setDirection();
            _move(1);
            //_move(1);
        }

        // 右を向く
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = DIRECTION.RIGHT;
            _setDirection();
            _move(1);
            //direction++;
            //_setDirection();

        }

        // 後退
        if (Input.GetKeyDown(KeyCode.S))
        {
            direction = DIRECTION.DOWN;
            _setDirection();
            _move(1);
            //_move(-1);
        }

        // 左を向く
        if (Input.GetKeyDown(KeyCode.A))
        {
            direction = DIRECTION.LEFT;
            _setDirection();
            _move(1);
            //direction--;
            //_setDirection();

        }
    }


    //==================================================
    // 向きの調整
    //==================================================

    void _setDirection()
    {
        int d = ((int)direction + (int)DIRECTION.MAX) % (int)DIRECTION.MAX;
        direction = (DIRECTION)d;
    }



    //==================================================
    // プレイヤー移動
    //==================================================

    void _move(int dir)
    {
        // 次に移動する場所を計算
        nextPos = currentPos + new Vector2Int(move[(int)direction, 0] * dir,move[(int)direction, 1] * dir);

        Debug.Log(nextPos);

        // 壁ではない場合だけ移動
        if (mapGenerator.GetNextMapType(nextPos)
            != MapGenerator.MAP_TYPE.WALL)
        {
            Debug.Log("壁じゃない");

            currentPos = nextPos;

            Debug.Log("現在位置：" + currentPos);

            // プレイヤーを移動
            transform.localPosition = mapGenerator.ScreenPos(currentPos);

            // 通った場所を記録
            mapGenerator.DiscoverPlayerPosition();

            // 3×3ミニマップを更新
            mapGenerator.UpdateMinimap();

            // 移動先のイベントを確認
            CheckEvent();
        }
    }


    //==================================================
    // イベント確認
    //==================================================

    void CheckEvent()
    {
        MapGenerator.MAP_TYPE type =
            mapGenerator.GetNextMapType(currentPos);


        //--------------- 謎解き① ---------------

        if (type == MapGenerator.MAP_TYPE.PUZZLE)
        {
            mapGenerator.OpenPuzzle();
        }


        //--------------- 謎解き② ---------------

        if (type == MapGenerator.MAP_TYPE.PUZZLE2)
        {
            mapGenerator.OpenPuzzle2();
        }


        //--------------- 階段 ---------------

        if (type == MapGenerator.MAP_TYPE.STAIR)
        {
            mapGenerator.CheckStair();
        }


        //--------------- ゴール ---------------

        if (type == MapGenerator.MAP_TYPE.GOAL)
        {
            Debug.Log("ステージクリア！");
            mapGenerator.ShowTreasureChest();
        }


        //--------------- 落とし穴 ---------------

        if (type == MapGenerator.MAP_TYPE.PIT)
        {
            Debug.Log("落とし穴に落ちた！");

            Damage(1);


            // 2Fから落ちた場合
            if (mapGenerator.CurrentFloor == 1)
            {
                Debug.Log("2F → 1F");

                mapGenerator.ChangeFloor(0, false);

                Debug.Log("1Fへ移動完了！");
            }

            // 1Fで落ちた場合
            else
            {
                Debug.Log("1F → スタート地点");

                currentPos =
                    mapGenerator.startPos;

                transform.localPosition =
                    mapGenerator.ScreenPos(currentPos);
            }
        }
    }


    //==================================================
    // HP・ダメージ
    //==================================================

    public void Damage(int damage)
    {
        currentHP -= damage;

        Debug.Log("HP : " + currentHP);

        // HPが0未満にならないようにする
        if (currentHP < 0)
        {
            currentHP = 0;
        }

        // HP表示を更新
        hp.Update();

        // HPが0になったらゲームオーバー
        if (currentHP <= 0)
        {
            Debug.Log("ゲームオーバー");
        }
    }
}