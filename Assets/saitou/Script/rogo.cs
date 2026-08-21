using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class rogo : MonoBehaviour
{
    public Image clearImage;
    public float fadeTime = 2f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // ç≈èâìßñæ
        Color color = clearImage.color;
        color.a = 0f;
        clearImage.color = color;

        float time = 0f;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, time / fadeTime);
            clearImage.color = color;

            yield return null;
        }

        // äÆëSï\é¶
        color.a = 1f;
        clearImage.color = color;
    }
}