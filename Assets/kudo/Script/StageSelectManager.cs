using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems; //UIの選択検知に必要

public class StageSelectManager : MonoBehaviour
{
    [Header("各ステージのボタン(1～3の順に登録)")]
    public Button[] stageButton;

    [Header("各ステージのイラスト(1～3の順に登録)")]
    public Image[] stageImages;

    [Header("クリア済みのステージ用のスプライト(水色＋王冠)")]
    public Sprite clearedSprite;

    [Header("未クリア用スプライト(赤)")]
    public Sprite unclearedSprite;

    [Header("解放されていない用スプライト(黒)")]
    public Sprite lockedSprite;

    [Header("プレイヤーの画像(PlayerCursor)")]
    public RectTransform playerCursor;

    [Header("プレイヤーの浮き上がりの高さ")]
    public float yOffset = 100;

    void Start()
    {

        //起動時に「ステージ１」のボタンを自動的に選択状態にする
        if (stageButton != null && stageButton.Length > 0)
        {
            stageButton[0].Select();
            //最初はステージ１の真上にプレヤーを配置
            UpdateCursorPosition(stageButton[0].gameObject);

        }

        //各ボタンのロック状態をチェック
        for (int i = 0; i < stageButton.Length; i++)
        {
            int stageNumber = i + 1; //ステージ１，ステージ２，ステージ3

            //1.自分自身がクリアされているか
            int isCurrentCleared = PlayerPrefs.GetInt("Stage" + stageNumber + "_Cleared", 0);

            //2.前のステージがクリアされているか（又はステージ1か）
            bool isPlayable = false;
            if (i == 0)
            {
                isPlayable = true; //ステージ1は最初から遊べる
            }
            else
            {
                int isPreviousCleared = PlayerPrefs.GetInt("Stage" + i + "_Cleared", 0);
                isPlayable = (isPreviousCleared == 1); //前のステージがクリアされていれば遊べる
            }

            //---3パターンの分岐処理---

            //クリアしたステージ（水色＋王冠）
            if (isCurrentCleared == 1)
            {
                stageButton[i].interactable = true;
                if (clearedSprite != null) stageButton[i].image.sprite = clearedSprite;
                SetImageMonochrome(i, false); //カラー
            }

            //未クリアステージ（赤）
            else if (isPlayable)
            {
                stageButton[i].interactable = true;
                if (unclearedSprite != null) stageButton[i].image.sprite = unclearedSprite;
                SetImageMonochrome(i, false); //カラー
            }

            //未開放ステージ（黒）
            else 
            {
                stageButton[i].interactable = false;
                if (lockedSprite != null) stageButton[i].image.sprite = lockedSprite;
                SetImageMonochrome(i, true); //カラー
            }
        }
    }

    // イラストのカラー・モノクロ（簡易版）を切り替える関数
    void SetImageMonochrome(int index, bool isMonochrome)
    {
        // 配列の範囲内かつ、画像が登録されているかチェック
        if (stageImages != null && index < stageImages.Length && stageImages[index] != null)
        {
            if (isMonochrome)
            {
                // モノクロ（暗めのグレーにして未解放感を出す）
                stageImages[index].color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            }
            else
            {
                // 通常カラー
                stageImages[index].color = Color.white;
            }
        }
    }

    void Update()
    {
        //現在EventSystemで選ばれるゲームオブジェクトを取得
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && playerCursor != null)
        {
            // 選ばれているオブジェクトの場所にプレイヤーを滑らかに移動させる
            UpdateCursorPosition(currentSelected);
        }
    }

    //プレイヤーの位置をボタンの真上に合わせる処理
    void UpdateCursorPosition(GameObject targetButton)
    {
        Vector3 targetPos = targetButton.transform.position;
        //y0ffsetの分だけ、ボタンより少し上に表示させる
        targetPos.y += yOffset;

        //Vector3.Lerpを使って、カクカクしないでスーッと滑らかに追従させる
        playerCursor.position = Vector3.Lerp(playerCursor.position, targetPos, Time.deltaTime * 10f);

    }

    //ボタンが押されたときにシーンを読み込む
    public void LoadStageScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
