using UnityEngine;
using UnityEngine.SceneManagement;シーン移動に必要
using UnityEngine.UI; //ボタンコンポーネントを操作する為に必要

public class Starge2 : MonoBehaviour
{
    void Start()
    {
        //自分のオブジェクトからコンポーネントを取得
        Button myBotton = GetComponent<Button>();

        //まだ保存されていない（未クリア）なら０が返ってくる
        int isClesred = PlayerPrefs.GetInt("Stage1_Cleared, 0");

        if (isClesred == 1)
        {
            myBotton.interactable = true; //ボタンを押せるようにする
        }
        else
        {
            myBotton.interactable = false; //ボタンを押せないように
        }

    }
        //ボタンがクリックされた時に呼び出す関数
        public void OnClickStart()
        {
           //移動したいシーン
           SceneManager.LoadScene("");
        }  
}

//クリア画面の次へボタンとかにこのコードを書く
//PlaterPrefs.SetInt("Stage1_Cleared", 1);
//PlayerPrefs.Save(); //データを確実に保存