using System.Collections;
//using DynamicCurrencyUI;
using UnityEngine;

namespace StarChestCreator         
{
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public GameObject gameScreen;
    public GameObject menuScreen;

    private bool allCurrenciesMoved = false;

    private void Awake()
    {
        instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // Prevent the screen from going to sleep
        Application.targetFrameRate = 60; // Set the frame rate to 60 FPS

        //if (gameScreen != null)
        //    gameScreen.SetActive(true);
        //if (menuScreen != null)
        //    menuScreen.SetActive(false);
    }

    private void OnEnable()
    {
        SC_CurrencySpawner.OnAllCurrenciesMoved += OnAllCurrenciesMoved;
    }

    private void OnDisable()
    {
        SC_CurrencySpawner.OnAllCurrenciesMoved -= OnAllCurrenciesMoved;
    }
    private void Start()
    {
        StartCoroutine(HandleCurrencyFlyAndChest());
    }

        // Transition to the menu screen while stars are flying
        public IEnumerator HandleCurrencyFlyAndChest()
    {
        SwitchToMenuScreen();
        yield return new WaitForSeconds(0.25f);

        // Calculate collected stars
        int starsCollected = StarManager.instance.GetCollectedStars(false);

        // Start currency animations
        allCurrenciesMoved = false;
        yield return StartCoroutine(SpawnCurrenciesWithDelay(starsCollected));

        // Wait until all currencies have finished moving
        yield return new WaitUntil(() => allCurrenciesMoved == true);

        // Once the stars have fully moved, update the UI
        yield return StartCoroutine(UpdateStarUIAndCheckTarget());
    }

    // Coroutine to spawn currencies with delay
    private IEnumerator SpawnCurrenciesWithDelay(int starsCollected)
    {
        // Start spawning coins
        SC_CurrencySpawner.instance.StartCurrencySpawn("Coin", 7); // Spawns 7 coins
        yield return new WaitForSeconds(0.5f); // Delay between different currency spawns

        // Start spawning stars (based on how many were collected)
        SC_CurrencySpawner.instance.StartCurrencySpawn("Star", starsCollected);
    }

    // Triggered when all currencies have completed their movement
    private void OnAllCurrenciesMoved()
    {
        // Set the flag to true when all currencies have moved
        allCurrenciesMoved = true;
    }

    // Update the UI and check the target once stars have finished flying
    private IEnumerator UpdateStarUIAndCheckTarget()
    {
        // Update the star bar and text
        StarManager.instance.UpdateUI(true, () =>
        {
            // Check if the star target is achieved
            StarManager.instance.CheckTarget();
        });

        // Wait for the UI update to complete
        yield return new WaitForSeconds(0.5f);
    }

    // Switch to the menu screen
    public void SwitchToMenuScreen()
    {
        //if (gameScreen != null)
        //    gameScreen.SetActive(false);
        //if (menuScreen != null)
        //    menuScreen.SetActive(true);
    }

    // Switch to the game screen
    public void SwitchToGameScreen()
    {
        //if (gameScreen != null)
        //    gameScreen.SetActive(true);
        //if (menuScreen != null)
        //    menuScreen.SetActive(false);
    }
}
}