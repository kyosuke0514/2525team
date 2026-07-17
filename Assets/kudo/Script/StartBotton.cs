using UnityEngine;
using UnityEngine.SceneManagement;//シーン

public class StartBotton : MonoBehaviour
{
    //ボタンがクリックされたときに呼び出す関数
    public void OnClickStart()
    {
        //""の中は移動したいシーン名
        SceneManager.LoadScene("SentakuScene");
    }
}
