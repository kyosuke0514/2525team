using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    //==================================================
    // Inspector
    //==================================================

    [Header("パズルUI")]
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject Puzzle;
    [SerializeField] private GameObject Puzzle2;
    [SerializeField] private GameObject Puzzle3;
    [SerializeField] private GameObject Puzzle4;

    [Header("プレイヤー")]
    [SerializeField] private Player player;

    [Header("ボタン")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("謎解き確認")]
    [SerializeField] private Image puzzleConfirmImage;
    [SerializeField] private Sprite puzzleConfirmSprite;
    [SerializeField] private GameObject treasureChestImage;

    [Header("階段")]
    [SerializeField] private Image stairImage;

    [Header("ミニマップ")]
    [SerializeField] private MiniMapGenerator miniMapGenerator;

    //==================================================
    // Puzzle1 入力
    //==================================================

    [Header("Puzzle1")]
    [SerializeField] private TMP_Text redNumberText;
    [SerializeField] private TMP_Text greenNumberText;
    [SerializeField] private TMP_Text blueNumberText;

    private int redNumber = 1;
    private int greenNumber = 1;
    private int blueNumber = 1;

    //==================================================
    // Puzzle2 入力
    //==================================================

    [Header("Puzzle2")]
    [SerializeField] private TMP_Text redNumberText2;
    [SerializeField] private TMP_Text greenNumberText2;
    [SerializeField] private TMP_Text yellowNumberText2;
    [SerializeField] private TMP_Text blueNumberText2;

    private int redNumber2 = 1;
    private int greenNumber2 = 1;
    private int yellowNumber2 = 1;
    private int blueNumber2 = 1;

    //==================================================
    // Puzzle3・4
    //==================================================

    private Toggle[] puzzle3Toggles;
    private Toggle[] puzzle4Toggles;

    //==================================================
    // パズル状態
    //==================================================

    public bool puzzleSolved = false;
    public bool puzzle2Solved = false;
    public bool puzzle3Solved = false;
    public bool puzzle4Solved = false;

    private bool puzzleConfirm = false;
    private bool puzzle2Confirm = false;
    private bool puzzle3Confirm = false;
    private bool puzzle4Confirm = false;

    //==================================================
    // 初期化
    //==================================================

    private void Start()
    {
        Panel.SetActive(false);

        Puzzle.SetActive(false);
        Puzzle2.SetActive(false);
        Puzzle3.SetActive(false);
        Puzzle4.SetActive(false);

        puzzleConfirmImage.gameObject.SetActive(false);
        treasureChestImage.SetActive(false);

        redNumberText.text = redNumber.ToString();
        greenNumberText.text = greenNumber.ToString();
        blueNumberText.text = blueNumber.ToString();

        puzzle3Toggles =
            Puzzle3.GetComponentsInChildren<Toggle>(true);

        puzzle4Toggles =
            Puzzle4.GetComponentsInChildren<Toggle>(true);

        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);
    }
    public bool IsPuzzleConfirm()
    {
        return puzzleConfirm ||
               puzzle2Confirm ||
               puzzle3Confirm ||
               puzzle4Confirm;
    }

    //==================================================
    // Puzzle1 数字入力
    //==================================================

    public void RedUp()
    {
        if (redNumber < 9)
        {
            redNumber++;
            redNumberText.text = redNumber.ToString();
        }
    }

    public void RedDown()
    {
        if (redNumber > 1)
        {
            redNumber--;
            redNumberText.text = redNumber.ToString();
        }
    }

    public void GreenUp()
    {
        if (greenNumber < 9)
        {
            greenNumber++;
            greenNumberText.text = greenNumber.ToString();
        }
    }

    public void GreenDown()
    {
        if (greenNumber > 1)
        {
            greenNumber--;
            greenNumberText.text = greenNumber.ToString();
        }
    }

    public void BlueUp()
    {
        if (blueNumber < 9)
        {
            blueNumber++;
            blueNumberText.text = blueNumber.ToString();
        }
    }

    public void BlueDown()
    {
        if (blueNumber > 1)
        {
            blueNumber--;
            blueNumberText.text = blueNumber.ToString();
        }
    }

    //==================================================
    // Puzzle2 数字入力
    //==================================================

    public void RedUp2()
    {
        if (redNumber2 < 9)
        {
            redNumber2++;
            redNumberText2.text = redNumber2.ToString();
        }
    }

    public void RedDown2()
    {
        if (redNumber2 > 1)
        {
            redNumber2--;
            redNumberText2.text = redNumber2.ToString();
        }
    }

    public void GreenUp2()
    {
        if (greenNumber2 < 9)
        {
            greenNumber2++;
            greenNumberText2.text = greenNumber2.ToString();
        }
    }

    public void GreenDown2()
    {
        if (greenNumber2 > 1)
        {
            greenNumber2--;
            greenNumberText2.text = greenNumber2.ToString();
        }
    }

    public void YellowUp2()
    {
        if (yellowNumber2 < 9)
        {
            yellowNumber2++;
            yellowNumberText2.text = yellowNumber2.ToString();
        }
    }

    public void YellowDown2()
    {
        if (yellowNumber2 > 1)
        {
            yellowNumber2--;
            yellowNumberText2.text = yellowNumber2.ToString();
        }
    }

    public void BlueUp2()
    {
        if (blueNumber2 < 9)
        {
            blueNumber2++;
            blueNumberText2.text = blueNumber2.ToString();
        }
    }

    public void BlueDown2()
    {
        if (blueNumber2 > 1)
        {
            blueNumber2--;
            blueNumberText2.text = blueNumber2.ToString();
        }
    }

    //==================================================
    // 正解判定
    //==================================================

    public void CheckPuzzle()
    {
        if (redNumber == 2 &&
            greenNumber == 3 &&
            blueNumber == 9)
        {
            Debug.Log("謎解き正解！");
            SEManager.Instance.PlayCorrect();

            puzzleSolved = true;
            player.mapGenerator.RegisterPuzzleKey(1);
            miniMapGenerator.UpdateMinimap();
            Puzzle.SetActive(false);
            player.isPuzzle = false;

            ClosePanel();
        }
        else
        {
            Debug.Log("不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    public void CheckPuzzle2F()
    {
        if (redNumber2 == 1 &&
            greenNumber2 == 3 &&
            yellowNumber2 == 7 &&
            blueNumber2 == 5)
        {
            Debug.Log("2F謎解き正解！");
            SEManager.Instance.PlayCorrect();

            puzzle2Solved = true;
            player.mapGenerator.RegisterPuzzleKey(2);
            miniMapGenerator.UpdateMinimap();
            Puzzle2.SetActive(false);
            player.isPuzzle = false;

            ClosePanel();
        }
        else
        {
            Debug.Log("不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    private int GetSelectedAnswer(Toggle[] toggles)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                return i + 1;
            }
        }

        return 0;
    }

    public void CheckPuzzle3()
    {
        int answer = GetSelectedAnswer(puzzle3Toggles);

        if (answer == 1)
        {
            Debug.Log("Puzzle3正解！");
            SEManager.Instance.PlayCorrect();

            puzzle3Solved = true;
            player.mapGenerator.RegisterPuzzleKey(3);
            miniMapGenerator.UpdateMinimap();
            Puzzle3.SetActive(false);
            player.isPuzzle = false;

            ClosePanel();
        }
        else
        {
            Debug.Log("Puzzle3不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    public void CheckPuzzle4()
    {
        int answer = GetSelectedAnswer(puzzle4Toggles);

        if (answer == 4)
        {
            Debug.Log("Puzzle4正解！");
            SEManager.Instance.PlayCorrect();

            puzzle4Solved = true;
            player.mapGenerator.RegisterPuzzleKey(4);
            miniMapGenerator.UpdateMinimap();
            Puzzle4.SetActive(false);
            player.isPuzzle = false;

            ClosePanel();
        }
        else
        {
            Debug.Log("Puzzle4不正解！");
            SEManager.Instance.PlayWrong();
        }
    }

    //==================================================
    // パズルを開く
    //==================================================

    public void OpenPuzzle()
    {
        if (puzzleSolved) return;

        OpenConfirm();

        puzzleConfirm = true;
    }

    public void OpenPuzzle2()
    {
        if (puzzle2Solved) return;

        OpenConfirm();

        puzzle2Confirm = true;
    }

    public void OpenPuzzle3()
    {
        if (puzzle3Solved) return;

        OpenConfirm();

        puzzle3Confirm = true;
    }

    public void OpenPuzzle4()
    {
        if (puzzle4Solved) return;

        OpenConfirm();

        puzzle4Confirm = true;
    }

    private void OpenConfirm()
    {
        // 階段の画像を消す
        stairImage.gameObject.SetActive(false);
        treasureChestImage.SetActive(true);

        puzzleConfirmImage.sprite = puzzleConfirmSprite;
        puzzleConfirmImage.gameObject.SetActive(true);

        Panel.SetActive(true);
        player.isPuzzle = true;
    }
    public void Yes()
    {
        Panel.SetActive(false);
        treasureChestImage.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);
        player.isPuzzle = false;

        // Puzzle1
        if (puzzleConfirm)
        {
            puzzleConfirm = false;

            Puzzle.SetActive(true);
            player.isPuzzle = true;

            return;
        }

        // Puzzle2
        if (puzzle2Confirm)
        {
            puzzle2Confirm = false;

            Puzzle2.SetActive(true);
            player.isPuzzle = true;

            return;
        }

        // Puzzle3
        if (puzzle3Confirm)
        {
            puzzle3Confirm = false;

            Puzzle3.SetActive(true);
            player.isPuzzle = true;

            return;
        }

        // Puzzle4
        if (puzzle4Confirm)
        {
            puzzle4Confirm = false;

            Puzzle4.SetActive(true);
            player.isPuzzle = true;

            return;
        }
    }
    public void No()
    {
        Panel.SetActive(false);
        treasureChestImage.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);

        puzzleConfirm = false;
        puzzle2Confirm = false;
        puzzle3Confirm = false;
        puzzle4Confirm = false;

        player.isPuzzle = false;
    }

    public void Back()
    {
        // パネルを閉じる
        Panel.SetActive(false);

        // パズル画面を閉じる
        Puzzle.SetActive(false);
        Puzzle2.SetActive(false);
        Puzzle3.SetActive(false);
        Puzzle4.SetActive(false);

        // 確認画面を閉じる
        treasureChestImage.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);
        stairImage.gameObject.SetActive(false);

        // 確認状態をリセット
        puzzleConfirm = false;
        puzzle2Confirm = false;
        puzzle3Confirm = false;
        puzzle4Confirm = false;

        // プレイヤー操作を戻す
        player.isPuzzle = false;
    }
    public void OpenStair(Sprite stairSprite)
    {
        stairImage.sprite = stairSprite;
        stairImage.gameObject.SetActive(true);

        Panel.SetActive(true);
        player.isPuzzle = true;
    }

    //==================================================
    // 確認画面
    //==================================================


    private void ClosePanel()
    {
        Panel.SetActive(false);
        treasureChestImage.SetActive(false);
        puzzleConfirmImage.gameObject.SetActive(false);
    }
}