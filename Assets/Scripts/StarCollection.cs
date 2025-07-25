using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarCollection : MonoBehaviour
{
    public SpriteRenderer star1;
    public SpriteRenderer star2;
    public SpriteRenderer star3;

    public GameObject[] star1Effects;
    public GameObject[] star2Effects;
    public GameObject[] star3Effects;

    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public ParticleSystem ps3;

    private void Start()
    {
        UpdateStarVisuals(GameManager.instance.earnedStarsInTheLevel);
    }

    private void UpdateStarVisuals(int earnedStars)
    {
        // Deactivate stars based on how many were NOT earned
        if (earnedStars < 3) DisableStar(star3, ps3, star3Effects);
        if (earnedStars < 2) DisableStar(star2, ps2, star2Effects);
        if (earnedStars < 1) DisableStar(star1, ps1, star1Effects);
    }

    private void DisableStar(SpriteRenderer starImage, ParticleSystem particleSystem, GameObject[] effects)
    {
        // Set star to the "off" state
        starImage.sprite = Resources.Load<Sprite>("Star_Off 1");

        // Stop and clear particles if assigned
        if (particleSystem != null)
        {
            particleSystem.loop = false;
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Deactivate all associated effects
        foreach (var effect in effects)
        {
            effect.SetActive(false);
        }
    }
}
