using UnityEngine;
using System.Collections.Generic;

public class MiniMapGenerator : MonoBehaviour
{
    //==================================================
    // MapGenerator
    //==================================================

    [SerializeField] private MapGenerator mapGenerator;


    //==================================================
    // ミニマップ設定
    //==================================================

    [SerializeField] private Transform minimap;
    [SerializeField] private float minimapTileSize = 100f;
    [SerializeField] private Sprite playerArrowSprite;


    //==================================================
    // 更新
    //==================================================

    public void UpdateMinimap()
    {
        Debug.Log("★★★ ミニマップ更新 ★★★");

        //==================================================
        // 前のミニマップを削除
        //==================================================

        for (int i = minimap.childCount - 1; i >= 0; i--)
        {
            Destroy(minimap.GetChild(i).gameObject);
        }


        //==================================================
        // 5×5ミニマップを作成
        //==================================================

        for (int y = -2; y <= 2; y++)
        {
            for (int x = -2; x <= 2; x++)
            {
                Vector2Int pos =
                    mapGenerator.player.currentPos + new Vector2Int(x, y);

                MapGenerator.MAP_TYPE type =
                    mapGenerator.GetNextMapType(pos);

                string mapKey =
                    mapGenerator.currentStage + "_" +
                    mapGenerator.currentFloor;


                //==================================================
                // 落とし穴が発見済みか
                //==================================================

                bool pitDiscovered = false;

                if (pos.x >= 0 &&
                    pos.x < mapGenerator.mapTable.GetLength(0) &&
                    pos.y >= 0 &&
                    pos.y < mapGenerator.mapTable.GetLength(1))
                {
                    if (mapGenerator.discoveredPitMaps.ContainsKey(mapKey))
                    {
                        pitDiscovered =
                            mapGenerator.discoveredPitMaps[mapKey][pos.x, pos.y];
                    }
                }


                //==================================================
                // 黒い背景を作成
                //==================================================

                GameObject tile =
                    Instantiate(
                        mapGenerator.prefabs[(int)MapGenerator.MAP_TYPE.GROUND],
                        minimap
                    );

                SpriteRenderer sr =
                    tile.GetComponent<SpriteRenderer>();

                if (sr == null)
                    continue;


                // 黒背景
                sr.color = Color.black;
                sr.sortingOrder = 97;


                //==================================================
                // マップの種類に応じて表示
                //==================================================

                if (type == MapGenerator.MAP_TYPE.WALL)
                {
                    // 壁
                    sr.color = Color.gray;
                }
                else if (type == MapGenerator.MAP_TYPE.STAIR_1_2 ||
                         type == MapGenerator.MAP_TYPE.STAIR_2_3 ||
                         type == MapGenerator.MAP_TYPE.STAIR_3_4)
                {
                    // 階段
                    CreateMinimapIcon(tile, type);
                }
                else if (type == MapGenerator.MAP_TYPE.PUZZLE &&
                         !mapGenerator.puzzleSolved)
                {
                    // 1F謎解き
                    CreateMinimapIcon(tile, type);
                }
                else if (type == MapGenerator.MAP_TYPE.PUZZLE2 &&
                         !mapGenerator.puzzle2Solved)
                {
                    // 2F謎解き
                    CreateMinimapIcon(tile, type);
                }
                else if (type == MapGenerator.MAP_TYPE.PIT &&
                         pitDiscovered)
                {
                    // 発見済み落とし穴
                    CreateMinimapIcon(tile, type);
                }
                else if (type >= MapGenerator.MAP_TYPE.WARP1 &&
                         type <= MapGenerator.MAP_TYPE.WARP16)
                {
                    // ワープ
                    CreateMinimapIcon(tile, type);
                }


                //==================================================
                // 位置・大きさ
                //==================================================

                tile.transform.localPosition = new Vector3(x * 100f, -y * 100f, 0);

                tile.transform.localScale = Vector3.one * 100f;

                tile.transform.localScale =
                    Vector3.one * 100f;
            }
        }

        CreatePlayerArrow();
    }


    //==================================================
    // ミニマップアイコン
    //==================================================

    private void CreateMinimapIcon(
        GameObject tile,
        MapGenerator.MAP_TYPE type)
    {
        GameObject sourcePrefab = null;


        //==================================================
        // 表示するアイコンの元Prefabを種類ごとに選択
        //==================================================

        switch (type)
        {
            // パズル
            case MapGenerator.MAP_TYPE.PUZZLE:
            case MapGenerator.MAP_TYPE.PUZZLE2:
            case MapGenerator.MAP_TYPE.PUZZLE3:
            case MapGenerator.MAP_TYPE.PUZZLE4:
            case MapGenerator.MAP_TYPE.PUZZLE5:
            case MapGenerator.MAP_TYPE.PUZZLE6:

                sourcePrefab =
                    mapGenerator.prefabs[5];

                break;


            // 階段
            case MapGenerator.MAP_TYPE.STAIR_1_2:
            case MapGenerator.MAP_TYPE.STAIR_2_3:
            case MapGenerator.MAP_TYPE.STAIR_3_4:

                sourcePrefab =
                    mapGenerator.prefabs[6];

                break;


            // 落とし穴
            case MapGenerator.MAP_TYPE.PIT:

                sourcePrefab =
                    mapGenerator.prefabs[4];

                break;


            // ワープ
            case MapGenerator.MAP_TYPE.WARP1:
            case MapGenerator.MAP_TYPE.WARP2:
            case MapGenerator.MAP_TYPE.WARP3:
            case MapGenerator.MAP_TYPE.WARP4:
            case MapGenerator.MAP_TYPE.WARP5:
            case MapGenerator.MAP_TYPE.WARP6:
            case MapGenerator.MAP_TYPE.WARP7:
            case MapGenerator.MAP_TYPE.WARP8:
            case MapGenerator.MAP_TYPE.WARP9:
            case MapGenerator.MAP_TYPE.WARP10:
            case MapGenerator.MAP_TYPE.WARP11:
            case MapGenerator.MAP_TYPE.WARP12:
            case MapGenerator.MAP_TYPE.WARP13:
            case MapGenerator.MAP_TYPE.WARP14:
            case MapGenerator.MAP_TYPE.WARP15:
            case MapGenerator.MAP_TYPE.WARP16:

                sourcePrefab =
                    mapGenerator.prefabs[7];

                break;


            default:
                return;
        }


        //==================================================
        // Prefab確認
        //==================================================

        if (sourcePrefab == null)
            return;


        SpriteRenderer original =
            sourcePrefab.GetComponent<SpriteRenderer>();

        if (original == null || original.sprite == null)
            return;


        //==================================================
        // ミニマップ用アイコンを作成
        //==================================================

        GameObject icon =
            new GameObject("MinimapIcon");

        icon.transform.SetParent(tile.transform);

        icon.transform.localPosition =
            Vector3.zero;

        icon.transform.localScale =
            Vector3.one;


        SpriteRenderer iconSR =
            icon.AddComponent<SpriteRenderer>();

        iconSR.sprite =
            original.sprite;

        iconSR.color =
            Color.white;

        iconSR.sortingOrder =
            98;
    }


    //==================================================
    // プレイヤー矢印
    //==================================================

    private void CreatePlayerArrow()
    {
        // プレイヤー画像を作成
        GameObject arrow =
            new GameObject("Player");

        // ミニマップの子にする
        arrow.transform.SetParent(
            minimap,
            false
        );

        // 5×5ミニマップの中央
        arrow.transform.localPosition =
            Vector3.zero;

        // タイルと同じくらいの大きさ
        arrow.transform.localScale =
            Vector3.one * 100f;


        // SpriteRendererを追加
        SpriteRenderer arrowSR =
            arrow.AddComponent<SpriteRenderer>();

        arrowSR.sprite =
            playerArrowSprite;

        arrowSR.color =
            Color.white;

        // タイルより前に表示
        arrowSR.sortingOrder =
            100;


        //==================================================
        // プレイヤーの向きに合わせて回転
        //==================================================

        switch (mapGenerator.player.direction)
        {
            case Player.DIRECTION.TOP:

                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, 0);

                break;

            case Player.DIRECTION.RIGHT:

                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, -90);

                break;

            case Player.DIRECTION.DOWN:

                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, 180);

                break;

            case Player.DIRECTION.LEFT:

                arrow.transform.localRotation =
                    Quaternion.Euler(0, 0, 90);

                break;
        }
    }
}