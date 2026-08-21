using UnityEngine;
using TMPro;
using System.Collections;

public class Gameoverrogo : MonoBehaviour
{
    public TextMeshProUGUI logo;
    public float fadeTime = 2f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color color = logo.color;
        color.a = 0;
        logo.color = color;

        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, time / fadeTime);
            logo.color = color;
            yield return null;
        }

        color.a = 1;
        logo.color = color;
    }
}