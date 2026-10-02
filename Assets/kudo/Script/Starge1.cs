using UnityEngine;
using UnityEngine.SceneManagement; //シーン移動に必要


public class Starge1 : MonoBehaviour 
{
    //ボタンクリックされたときに呼び出す関数
    [SerializeField] MapGenerator mapGenerator;
    public void OnClickStart()
    {
        //ステージ1
        mapGenerator.ChangeStage(0);
    }
}
