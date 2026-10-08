using UnityEngine;
using UnityEngine.UI;

public class karikarikari : MonoBehaviour
{
    public Image rankImage;

    public Sprite imageA;
    public Sprite imageB;
    public Sprite imageC;

    public enum Rank
    {
        A,
        B,
        C
    }

    void Start()
    {
        // Å‰‚Í”ñ•\¦
        rankImage.enabled = false;
    }

    // ResultTime‚©‚çŒÄ‚Ño‚·
    public void ShowRank(float clearTime, int stageNum)
    {
        float rankATime = stageNum * 60;
        // 60•bˆÈ“à  A
        if (clearTime <= rankATime)
        {
            rankImage.sprite = imageA;
        }
        // ‚Q•ª  B
        else if (clearTime <= rankATime+60)
        {
            rankImage.sprite = imageB;
        }
        // ‚R•ª  C
        else if(clearTime <= rankATime + 120)
        {
            rankImage.sprite = imageC;
        }

        rankImage.enabled = true;
    }
}