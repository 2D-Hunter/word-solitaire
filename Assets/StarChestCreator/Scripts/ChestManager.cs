using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

namespace StarChestCreator         
{
public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;

    [Header("Chest Settings")]
    public List<Chest> chestSOList;  // List of all chest ScriptableObjects
    private Chest currentChestSO;    // Currently selected chest ScriptableObject
    private List<Chest> selectedChests = new List<Chest>(); // List of previously selected chests

    public Transform spawnPoint;
    public Transform rewardContainer;

    [HideInInspector]
    public Transform[] targetPoints;  // Target points where rewards will move to

    [Header("Speed Settings")]
    public float speed = 1f;  // Movement speed for rewards
    public float scaleSpeed = 1f;  // Scale speed for rewards

    public GameObject chestGameObject;  // Reference to the chest GameObject

    private List<GameObject> spawnedRewards = new List<GameObject>();  // List of spawned reward objects

    void Start()
    {
        // Subscribe to ChestAnimator events
        ChestAnimator.instance.OnOpenChestAnimationComplete += OnOpenChestAnimationComplete;
        ChestAnimator.instance.OnClaimChestAnimationComplete += OnClaimChestAnimationComplete;

        // Load the saved chest index
        if (PlayerPrefs.HasKey("SelectedChestIndex"))
        {
            int index = PlayerPrefs.GetInt("SelectedChestIndex");
            if (chestSOList != null && index >= 0 && index < chestSOList.Count)
            {
                currentChestSO = chestSOList[index];

                // Set sprite frames for ChestAnimator
                SetChestSpriteFrames();

                // Start idle animation
                ChestAnimator.instance.PlayIdleAnimation();
            }
            else
            {
                Debug.LogError("Invalid saved chest index or chest not found in list. Selecting a new chest.");
                SelectRandomChestSO();
            }
        }
        else
        {
            // If no chest is saved, select a random one
            SelectRandomChestSO();
        }
    }

    // Set the sprite frames for the chest animation
    private void SetChestSpriteFrames()
    {
        if (currentChestSO != null && currentChestSO.chestSprites != null && currentChestSO.chestSprites.Length >= 15)
        {
            // Ensure sprites are in the correct order
            List<Sprite> frames = new List<Sprite>(currentChestSO.chestSprites);

            // Sort the sprites by their name (if needed)
            frames.Sort(CompareSpriteNames);

            // Set sprite frames in ChestAnimator
            ChestAnimator.instance.SetSpriteFrames(frames);
        }
        else
        {
            Debug.LogError("Current chest SO or sprites are invalid, or not enough sprites are assigned.");
        }
    }

    // Compare sprite names to sort them in order
    private int CompareSpriteNames(Sprite x, Sprite y)
    {
        int xIndex = ExtractFrameNumber(x.name);
        int yIndex = ExtractFrameNumber(y.name);
        return xIndex.CompareTo(yIndex);
    }

    // Extract frame number from sprite name (assuming the number is at the end)
    private int ExtractFrameNumber(string spriteName)
    {
        string[] parts = spriteName.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[parts.Length - 1], out int frameNumber))
        {
            return frameNumber;
        }
        else
        {
            Debug.LogWarning($"Unable to extract frame number from sprite name: {spriteName}");
            return 0;
        }
    }

    // Event handler for when the chest open animation completes
    private void OnOpenChestAnimationComplete()
    {
        // Perform any actions needed after the chest open animation
    }

    // Event handler for when the claim chest animation completes
    private void OnClaimChestAnimationComplete()
    {
        // Perform any actions needed after the claim chest animation
        //ChestAnimator.instance.ResetChest();
    }

    // Select a random chest ScriptableObject (SO) that has not been selected before
    public void SelectRandomChestSO()
    {
        if (chestSOList != null && chestSOList.Count > 0)
        {
            // Reset the selected chests list if all chests have been selected
            if (selectedChests.Count >= chestSOList.Count)
            {
                selectedChests.Clear();
                Debug.Log("All chests have been selected. Resetting the list.");
            }

            // Find chests that have not been selected yet
            List<Chest> remainingChests = new List<Chest>(chestSOList);
            foreach (var selected in selectedChests)
            {
                remainingChests.Remove(selected); // Remove previously selected chests
            }

            // Select a random chest from the remaining ones
            int randomIndex = Random.Range(0, remainingChests.Count);
            currentChestSO = remainingChests[randomIndex];

            // Add the selected chest to the list of selected chests
            selectedChests.Add(currentChestSO);

            // Save the selected chest index
            PlayerPrefs.SetInt("SelectedChestIndex", chestSOList.IndexOf(currentChestSO));
            PlayerPrefs.Save();

            // Set the sprite frames for the selected chest
            SetChestSpriteFrames();
        }
        else
        {
            Debug.LogError("Chest SO List is empty or null.");
        }
    }

    // Calculate the target points for rewards to move to
    private void CalculateTargetPoints(int rewardCount)
    {
        float horizontalSpacing = 600f;
        float verticalSpacing = 600f;
        float arcHeight = 200f;

        RectTransform containerRect = rewardContainer.GetComponent<RectTransform>();

        // Determine number of rows based on reward count
        int numRows = (rewardCount <= 3) ? 1 : 2;
        int rewardsPerRow = Mathf.CeilToInt(rewardCount / (float)numRows);

        targetPoints = new Transform[rewardCount];
        int rewardIndex = 0;

        // Loop through rows to place target points
        for (int i = 0; i < numRows; i++)
        {
            int rewardsInThisRow = (i < numRows - 1) ? rewardsPerRow : rewardCount - rewardIndex;
            float rowWidth = (rewardsInThisRow - 1) * horizontalSpacing;
            Vector3 startPosition = new Vector3(-rowWidth / 2, containerRect.rect.height / 4 - i * verticalSpacing, 0);

            // Loop through each row to place target points horizontally
            for (int j = 0; j < rewardsInThisRow; j++)
            {
                float x = startPosition.x + j * horizontalSpacing;
                float y = startPosition.y;
                float arcOffset = (j == 0 || j == rewardsInThisRow - 1) ? 0 : arcHeight;

                // Create a target point for reward placement
                GameObject targetPoint = new GameObject("TargetPoint");
                targetPoint.transform.SetParent(rewardContainer, false);
                targetPoint.transform.localPosition = new Vector3(x, y + arcOffset, 0);
                targetPoints[rewardIndex++] = targetPoint.transform;
            }
        }
    }

    // Trigger the chest opening and reward spawning process
    public void GiveRewards()
    {
        // Start the Open Chest animation
        ChestAnimator.instance.PlayOpenChestAnimation();

        if (currentChestSO != null)
        {
            // Calculate target points for rewards
            CalculateTargetPoints(currentChestSO.chestRewards.Count);
            StartCoroutine(SpawnAndMoveRewards());
        }
        else
        {
            Debug.LogError("Invalid chest or not enough target points.");
        }
    }

    // Coroutine to spawn and move rewards to target positions
    private IEnumerator SpawnAndMoveRewards()
    {
        yield return new WaitForSeconds(0.1f);  // Small delay before rewards start moving

        int rewardCount = currentChestSO.chestRewards.Count;
        spawnedRewards.Clear();

        // Spawn rewards and move them to their target points
        for (int i = 0; i < rewardCount; i++)
        {
            if (targetPoints[i] == null)
            {
                Debug.LogError($"Target point at index {i} is null.");
                continue;
            }

            var reward = currentChestSO.chestRewards[i];
            var instance = Instantiate(reward.reward.rewardPrefab, spawnPoint.position, Quaternion.identity, rewardContainer);
            instance.transform.localScale = Vector3.zero;  // Start at zero scale
            spawnedRewards.Add(instance);

            StartCoroutine(ScaleAndMove(instance.transform, targetPoints[i].position, reward.amount));
            if (i < rewardCount - 1)
            {
                yield return new WaitForSeconds(0.1f);  // Delay between spawning rewards
            }
        }
    }

    // Coroutine to scale and move rewards to target position
    private IEnumerator ScaleAndMove(Transform objTransform, Vector3 targetPosition, int amount)
    {
        float time = 0f;
        Vector3 initialPosition = objTransform.position;
        Vector3 initialScale = Vector3.zero;  // Start with zero scale
        Vector3 targetScale = Vector3.one;   // Target scale is the normal size

        // Simultaneously scale and move the object
        while (time < 1f)
        {
            time += Time.deltaTime * speed;

            // SmoothStep for smooth movement and scaling
            float t = Mathf.SmoothStep(0f, 1f, time);
            objTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);  // Scale up
            objTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);  // Move to target position

            yield return null;
        }

        // Ensure final position and scale
        objTransform.localScale = targetScale;
        objTransform.position = targetPosition;

        // Update the TextMesh with reward amount
        TextMeshProUGUI textMesh = objTransform.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = amount.ToString();
            StartCoroutine(ScaleTextWithBounce(textMesh.transform));
        }
        else
        {
            Debug.LogWarning("No TextMeshPro component found in the reward prefab's children.");
        }
    }

    // Coroutine for scaling the text with a bounce effect
    private IEnumerator ScaleTextWithBounce(Transform textTransform)
    {
        float time = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;
        float overshoot = 1.4f;  // Amount to overshoot for bounce effect
        float duration = 0.3f;

        // Scale with bounce effect
        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            float scaleValue = Mathf.Lerp(0f, overshoot, progress);
            scaleValue -= (scaleValue - 1f) * progress;
            textTransform.localScale = initialScale + (targetScale - initialScale) * scaleValue;
            yield return null;
        }

        textTransform.localScale = targetScale;
    }

    // Claim the rewards
    public void ClaimRewards()
    {
        StartCoroutine(ClaimAndDestroyRewards());
    }

    // Coroutine to claim and destroy the rewards
    private IEnumerator ClaimAndDestroyRewards()
    {
        float delayBetweenClaims = 0.1f;  // Delay between claiming each reward
        float claimAnimationDuration = 0.15f;

        // Dictionary to summarize claimed rewards
        Dictionary<string, int> rewardSummary = new Dictionary<string, int>();

        // List to store coroutines for claiming rewards
        List<Coroutine> claimCoroutines = new List<Coroutine>();

        // Loop through all spawned rewards and claim them
        for (int i = 0; i < spawnedRewards.Count; i++)
        {
            GameObject reward = spawnedRewards[i];
            ChestReward chestReward = currentChestSO.chestRewards[i];
            Transform targetPoint = targetPoints[i];

            // Summarize the rewards
            string rewardName = chestReward.reward.name;
            if (rewardSummary.ContainsKey(rewardName))
            {
                rewardSummary[rewardName] += chestReward.amount;
            }
            else
            {
                rewardSummary.Add(rewardName, chestReward.amount);
            }

            // Start claim animation coroutine for each reward
            if (reward != null && targetPoint != null)
            {
                claimCoroutines.Add(StartCoroutine(ClaimAnimation(reward.transform, claimAnimationDuration)));
                Destroy(targetPoint.gameObject);  // Destroy the target point
            }

            // Process special reward types
            if (chestReward.reward.rewardType == RewardType.SuperBonus)
            {
                //BlocksManager.instance.AddSuperBonus(chestReward.reward.superBonusType, chestReward.amount);
            }
            else if (chestReward.reward.rewardType == RewardType.ScorePoints)
            {
                //ScoreManager.AddToTotalScore(chestReward.amount);
            }

            // Add a delay between claiming rewards
            yield return new WaitForSeconds(delayBetweenClaims);
        }

        // After all rewards are claimed, reset the current chest and delete the saved index
        currentChestSO = null;
        PlayerPrefs.DeleteKey("SelectedChestIndex");
        PlayerPrefs.Save();

        // Log claimed rewards
        foreach (var entry in rewardSummary)
        {
            Debug.Log($"Claimed {entry.Value} of {entry.Key}");
        }

        // After all claim animations, play chest claim animation
        yield return StartCoroutine(ChestAnimator.instance.PlayClaimChestAnimationCoroutine());  // Wait for claim chest animation

        // Reset chest and select a new chest SO
        yield return new WaitForSeconds(0.1f);  // Optional delay for smoother transition
        ChestAnimator.instance.ResetChest();     // Reset the chest visuals and state
        SelectRandomChestSO();  // Select a new chest SO
    }

    // Coroutine for claiming animation with scaling effect
    private IEnumerator ClaimAnimation(Transform target, float duration)
    {
        float time = 0f;
        float overshoot = 1.7f;
        Vector3 initialScale = target.localScale;
        Vector3 targetScale = Vector3.one * overshoot;

        // Scale up
        while (time < duration / 2)
        {
            time += Time.deltaTime;
            float scaleValue = Mathf.Lerp(1f, overshoot, time / (duration / 2));
            target.localScale = initialScale * scaleValue;
            yield return null;
        }

        time = 0f;

        // Scale down and destroy the object
        while (time < duration / 2)
        {
            time += Time.deltaTime;
            float scaleValue = Mathf.Lerp(overshoot, 0f, time / (duration / 2));
            target.localScale = initialScale * scaleValue;
            yield return null;
        }

        Destroy(target.gameObject);
    }
}
}