using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    [Header("SE")]
    public AudioClip walkSE;
    public AudioClip chestSE;
    public AudioClip correctSE;
    public AudioClip wrongSE;
    public AudioClip pitSE;
    public AudioClip poisonSE;
    public AudioClip damageSE;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWalk()
    {
        audioSource.PlayOneShot(walkSE);
    }

    public void PlayChest()
    {
        audioSource.PlayOneShot(chestSE);
    }

    public void PlayCorrect()
    {
        audioSource.PlayOneShot(correctSE);
    }

    public void PlayWrong()
    {
        audioSource.PlayOneShot(wrongSE);
    }

    public void PlayPit()
    {
        audioSource.PlayOneShot(pitSE);
    }

    public void PlayPoison()
    {
        audioSource.PlayOneShot(poisonSE);
    }

    public void PlayDamage()
    {
        audioSource.PlayOneShot(damageSE);
    }
}