using UnityEngine;
using UnityEngine.UI;

namespace UISwitcher {
	public enum ToggleType { Music, SFX }
	//[SerializeField] private ToggleType toggleType1;
	public class UISwitcher : UINullableToggle {
		// 🎯 Add this field to differentiate toggles
		public enum ToggleType { Music, SFX }
		[SerializeField] private ToggleType toggleType;
		public GameObject obg;
		private readonly Vector2 _min = new(0, 0.5f);
		private readonly Vector2 _max = new(1, 0.5f);
		private readonly Vector2 _middle = new(0.5f, 0.5f);

		[SerializeField] private Graphic backgroundGraphic;
		[SerializeField] private Color onColor, offColor, nullColor;
		[SerializeField] private RectTransform tipRect;

		

		private Color backgroundColor {
			set {
				if (backgroundGraphic == null) return;
				backgroundGraphic.color = value;
			}
		}
		protected override void OnChanged(bool? obj) {
			Debug.Log($"OnChanged: {obj} | HasValue: {obj.HasValue} | Value: {obj.GetValueOrDefault()} | Toggle: {toggleType}");

			if (obj.HasValue)
            {
                if (obj.Value)
                    SetOn();
                else
                    SetOff();
            }
            else
            {
                SetNull();
            }
            base.OnChanged(obj);

			if (SoundManager.instance == null) return;

			if (obj.HasValue)
			{
				if (toggleType == ToggleType.Music)
				{
					SoundManager.instance.ToggleMuteBGMFromUI(obj.Value); // Toggle Music
				}
				else if (toggleType == ToggleType.SFX)
				{
					SoundManager.instance.ToggleMuteSFXFromUI(obj.Value); // Toggle SFX
				}
			}
		}
		private void Start()
		{
			if (SoundManager.instance != null)
			{
				if (toggleType == ToggleType.Music)
				{
					if (SoundManager.instance.IsBGMMuted())
						SetOff();
					else
						SetOn();
				}
				else if (toggleType == ToggleType.SFX)
				{
					if (SoundManager.instance.IsSFXMuted())
						SetOff();
					else
						SetOn();
				}
			}
		}

		private void SetOn() {
			SetAnchors(_max);
			backgroundColor = onColor;
		}

		private void SetOff() {
			SetAnchors(_min);
			backgroundColor = offColor;
		}

		private void SetNull() {
			SetAnchors(_middle);
			backgroundColor = nullColor;
		}

		private void SetAnchors(Vector2 anchor) {
			tipRect.anchorMin = anchor;
			tipRect.anchorMax = anchor;
			tipRect.pivot = anchor;
		}
	}
}