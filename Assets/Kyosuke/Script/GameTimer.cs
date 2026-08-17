using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;

    float elapsedTime = 0f;

    void Update()
    {
        Debug.Log("TImer Update");
        elapsedTime += Time.deltaTime;

        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);

        timeText.text = string.Format("TIME {0:00}:{1:00}", minutes, seconds);
    }
}
