using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private Sprite Heart;
    [SerializeField] private Sprite WhiteHeart;

    [SerializeField] private Image[] hearts;

    void Start()
    {
        Update();
    }

    public void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < player.currentHP)
            {
                hearts[i].sprite = Heart;
            }
            else
            {
                hearts[i].sprite = WhiteHeart;
            }
        }
    }
}
