using UnityEngine;
using TMPro;
using System.Collections;

public class cleartime1 : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    // ランク表示スクリプト
    public karikarikari rank;

    // クリアタイム
    public float clearTime;

    //カウントアップするか
    public float animationTime = 3f;

    void Start()
    {
        // メインゲーム 時間を受け取る
        clearTime = GameTimer.elapsedTime;

        StartCoroutine(CountUpTime());
    }

    IEnumerator CountUpTime()
    {
        float current = 0f;
        float elapsed = 0f;

        while (elapsed < animationTime)
        {
            elapsed += Time.deltaTime;

            current = Mathf.Lerp(0f, clearTime, elapsed / animationTime);

            int minute = (int)(current / 60);
            int second = (int)(current % 60);

            timeText.text = string.Format("{0:00}:{1:00}", minute, second);

            yield return null;
        }

       
        int finalMinute = (int)(clearTime / 60);
        int finalSecond = (int)(clearTime % 60);

        timeText.text = string.Format("{0:00}:{1:00}", finalMinute, finalSecond);

        // 少し待ってからランク表示    
        yield return new WaitForSeconds(0.3f);

        rank.ShowRank(clearTime);
    }
}