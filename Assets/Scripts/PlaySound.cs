using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public static PlaySound instance;
    public AudioSource audioSource;

    void Start()
    {
        instance = this;
    }

    public void PlaySoundEffect()
    {
        audioSource.Play();
    }
}