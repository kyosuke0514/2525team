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
    public void ShowRank(float clearTime)
    {
        // 3•ªˆÈ“à ¨ A
        if (clearTime <= 180)
        {
            rankImage.sprite = imageA;
        }
        // 3•ª’´`6•ªˆÈ“à ¨ B
        else if (clearTime <= 360)
        {
            rankImage.sprite = imageB;
        }
        // 6•ª’´`8•ªˆÈ“à ¨ C
        else
        {
            rankImage.sprite = imageC;
        }

        rankImage.enabled = true;
    }
}