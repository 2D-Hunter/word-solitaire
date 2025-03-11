using UnityEngine;
using UnityEngine.UI;

namespace UISwitcher
{
	public class UISwitcherSFX : UINullableToggle
	{
		private readonly Vector2 _min = new(0, 0.5f);
		private readonly Vector2 _max = new(1, 0.5f);
		private readonly Vector2 _middle = new(0.5f, 0.5f);
		[SerializeField] private Graphic backgroundGraphic;
		[SerializeField] private Color onColor, offColor, nullColor;
		[SerializeField] private RectTransform tipRect;
		private Color backgroundColor
		{
			set
			{
				if (backgroundGraphic == null) return;
				backgroundGraphic.color = value;
			}
		}
		private void Start()
		{
			if (SoundManager.instance != null)
			{
				SetState(!SoundManager.instance.IsBGMMuted()); // Sync with SoundManager
			}
		}
		protected override void OnChanged(bool? obj)
		{
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

			if (SoundManager.instance != null && obj.HasValue)
			{
				SoundManager.instance.ToggleMuteBGMFromUI(obj.Value); // Mute/unmute BGM
			}
		}
		private void SetState(bool isOn)
		{
			if (isOn)
			{
				SetOn();
			}
			else
			{
				SetOff();
			}
		}

		private void SetOn()
		{
			SetAnchors(_max);
			backgroundColor = onColor;
		}

		private void SetOff()
		{
			SetAnchors(_min);
			backgroundColor = offColor;
		}

		private void SetNull()
		{
			SetAnchors(_middle);
			backgroundColor = nullColor;
		}

		private void SetAnchors(Vector2 anchor)
		{
			tipRect.anchorMin = anchor;
			tipRect.anchorMax = anchor;
			tipRect.pivot = anchor;
		}
	}
}