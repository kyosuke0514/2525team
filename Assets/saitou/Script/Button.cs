using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverButton : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSE;

    Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void ScripDown()
    {
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        transform.localScale = originalScale * 0.9f;

      //  audioSource.PlayOneShot(clickSE);

        yield return new WaitForSeconds(0.1f);

        transform.localScale = originalScale;

        yield return new WaitForSeconds(0.3f);

        // SceneManager.LoadScene("タイトルかステージ選択");
    }
}