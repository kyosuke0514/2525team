using UnityEngine;
using UnityEngine.SceneManagement;//シーン移動に必要
using UnityEngine.EventSystems;//ボタンの選択解除に使う

public class SetumeiBotton : MonoBehaviour
{
    [SerializeField] private GameObject comfirmPanel;

    //右ボタンを押すと説明②の画面に移る
    public void OpenConfirmPanel()
    {
        comfirmPanel.SetActive(true);//パネルを表示す

        //ボタンの選択状態を解除
        EventSystem.current.SetSelectedGameObject(null);
    }

    //左ボタンを押すと前のページに戻る
    public void CloseConfirmPanel()
    {
        comfirmPanel.SetActive(false);//パネルを非表示にする

        //ボタンの選択状態を解除
        EventSystem.current.SetSelectedGameObject(null);
    }

    //ステージ選択へボタンを押したら次のシーンに移る
    public void OnClickStart()
    {

        //""の中は移動したいシーン名
        SceneManager.LoadScene("SentakuScene");
    }
}