using UnityEngine;
using UnityEngine.UI;

public class AvatarCycler : MonoBehaviour
{
    public Image avatarImage;           // Reference to the UI Image
    public Sprite[] avatarSprites;      // Assign manually or load from Resources
    private float switchInterval = 0.2f; // How fast to cycle

    private int currentIndex = 0;
    private float timer = 0f;
    private bool isCycling = false;

    void Start()
    {
        // Load avatars from Resources if not set manually
        if (avatarSprites == null || avatarSprites.Length == 0)
        {
            avatarSprites = Resources.LoadAll<Sprite>("Avatars");
        }
        Invoke("StartCycling", 0.5f);
    }

    void Update()
    {
        if (!isCycling || avatarSprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= switchInterval)
        {
            timer = 0f;
            CycleAvatar();
        }
    }

    void CycleAvatar()
    {
        currentIndex = (currentIndex + 1) % avatarSprites.Length;
        avatarImage.sprite = avatarSprites[currentIndex];
    }

    // Call this to start cycling
    public void StartCycling()
    {
        isCycling = true;
        currentIndex = 0;
        timer = 0f;
    }

    // Call this to stop cycling and select a random final avatar
    public void StopCyclingAndSelectFinal()
    {
        isCycling = false;

        // Choose a random final avatar (or you can use currentIndex if preferred)
        int finalIndex = Random.Range(0, avatarSprites.Length);
        avatarImage.sprite = avatarSprites[finalIndex];
    }
}