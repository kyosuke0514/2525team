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

    // ‰¼‚Ìƒ‰ƒ“ƒN
    public Rank rank = Rank.A;

    void Start()
    {
        // Å‰‚Í”ñ•\¦
        rankImage.enabled = false;

        switch (rank)
        {
            case Rank.A:
                rankImage.sprite = imageA;
                break;

            case Rank.B:
                rankImage.sprite = imageB;
                break;

            case Rank.C:
                rankImage.sprite = imageC;
                break;
        }
    }

    // ResultTime‚©‚çŒÄ‚Ño‚·
    public void ShowRank()
    {
        rankImage.enabled = true;
    }
}