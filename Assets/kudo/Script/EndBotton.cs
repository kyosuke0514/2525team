using UnityEngine;

public class EndBotton : MonoBehaviour
{
    [SerializeField] private GameObject KuroPanel;
        
        //終わるボタンを押したときに呼び出す
        public void OpenConfirmPanel()
    {
        KuroPanel.SetActive(true); //パネルを表示する
    }

    //NOボタンを押したときによびだす
    public void CloseContfirmPanel()
    {
        KuroPanel.SetActive(false); //パネルを非表示にする
    }

    //YESボタンを押したときによびだす
    public void GameQuit()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false; //エディター再生終了
#else
        Application.Quit(); //ビルドしたゲーム終了
#endif
    }
}
