using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    
    public Image fadeImage;

    IEnumerator Start()
    {
        Color color = fadeImage.color;

        // 最初は黒
        color.a = 1f;
        fadeImage.color = color;

        // フェードイン
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, time / 1f);

            fadeImage.color = color;

            yield return null;
        }

        // 5秒待つ
        yield return new WaitForSeconds(5f);

        // フェードアウト
        time = 0f;

        while (time < 0.4f)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, time / 0.4f);

            fadeImage.color = color;

            yield return null;
        }

        // シーン移動
        SceneManager.LoadScene("SentakuScene");
    }

}

