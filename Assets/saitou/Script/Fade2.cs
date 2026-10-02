using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade2 : MonoBehaviour
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

        while (time < 3f)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, time / 3f);

            fadeImage.color = color;

            yield return null;
        }

        // 最後は完全に透明
        color.a = 0f;
        fadeImage.color = color;
    }
}