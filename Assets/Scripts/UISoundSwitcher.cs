using UnityEngine;
using UnityEngine.UI;
public class UISoundSwitcher : MonoBehaviour
{
    public enum ToggleType { Music, SFX }

    [SerializeField] private ToggleType toggleType; // Enum selection in Inspector
    [SerializeField] private Graphic backgroundGraphic;
    [SerializeField] private Color onColor, offColor, nullColor;
    [SerializeField] private RectTransform tipRect;
    [SerializeField] private bool isOn; // Tracks toggle state

    public System.Action<bool, ToggleType> OnValueChanged; // Event for external listeners

    private void Start()
    {
        UpdateVisuals();
    }

    public void Toggle()
    {
        isOn = !isOn;
        UpdateVisuals();

        // Use enum to determine behavior
        if (toggleType == ToggleType.Music)
        {
            Debug.Log("Toggling Music: " + isOn);
        }
        else if (toggleType == ToggleType.SFX)
        {
            Debug.Log("Toggling SFX: " + isOn);
        }

        OnValueChanged?.Invoke(isOn, toggleType); // Notify listeners
    }

    private void UpdateVisuals()
    {
        if (isOn)
            SetOn();
        else
            SetOff();
    }

    private void SetOn()
    {
        tipRect.anchorMin = new Vector2(1, 0.5f);
        tipRect.anchorMax = new Vector2(1, 0.5f);
        tipRect.pivot = new Vector2(1, 0.5f);
        backgroundGraphic.color = onColor;
    }

    private void SetOff()
    {
        tipRect.anchorMin = new Vector2(0, 0.5f);
        tipRect.anchorMax = new Vector2(0, 0.5f);
        tipRect.pivot = new Vector2(0, 0.5f);
        backgroundGraphic.color = offColor;
    }

    public void OnToggleClick()
    {
        Toggle();
    }
}