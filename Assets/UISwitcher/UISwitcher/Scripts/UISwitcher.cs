using UnityEngine;
using UnityEngine.UI;

namespace UISwitcher
{
	public class UISwitcher : UINullableToggle {

		private readonly Vector2 _min = new(0, 0.5f);
		private readonly Vector2 _max = new(1, 0.5f);
		private readonly Vector2 _middle = new(0.5f, 0.5f);
		[SerializeField] private Graphic backgroundGraphic;
		[SerializeField] private Color onColor, offColor, nullColor;
		[SerializeField] private RectTransform tipRect;
		private Color backgroundColor
		{
			set {
				if (backgroundGraphic == null) return;
				backgroundGraphic.color = value;
			}
		}
		private void Start()
		{
			//if (SoundManager.instance != null)
			//{
			//	if (gameObject.tag == "Music")
			//		SetState(!SoundManager.instance.IsBGMMuted()); // Sync with SoundManager
			//	else if (gameObject.tag == "SFX")
			//		SetState(!SoundManager.instance.IsSFXMuted());
			//}
			Debug.Log("FBPlayerData.instance.GAME_MUSIC: " + FBPlayerData.instance.GAME_MUSIC);
			Debug.Log("FBPlayerData.instance.GAME_SOUND: " + FBPlayerData.instance.GAME_SOUND);
			//FBPlayerData.instance.GAME_MUSIC = false;
			//Debug.Log("gameObject.tag: " + gameObject.tag);
			if (gameObject.tag == "Music")
            {
				if (FBPlayerData.instance.GAME_MUSIC)
				{
					SoundManager.instance.isBgmMuted = false;
					SetOn();
				}
				else
				{
					SoundManager.instance.isBgmMuted = true;
					SetOff();
				}
				//SoundManager.instance.bgmSource.mute = SoundManager.instance.isBgmMuted;
			}
			else if (gameObject.tag == "SFX")
			{
				//SoundManager.instance.SetSFXUI();
			}

			


			//if (FBPlayerData.instance.GAME_SOUND)
			//    SoundManager.instance.ToggleMuteSFXFromUI(true);
			//else
			//    SoundManager.instance.ToggleMuteSFXFromUI(false);

			//bool isBgmMuted = SoundManager.instance.IsBGMMuted();
			//Debug.Log(isBgmMuted);
			//SetState(!isBgmMuted);
			//bool isSfxMuted = SoundManager.instance.IsSFXMuted();
			//Debug.Log(isSfxMuted);
			//SetState(!isSfxMuted);
		}
		protected override void OnChanged(bool? obj)
		{
			Debug.Log("Tage: "+gameObject.tag);
			Debug.Log("OnChanged: " + obj + obj.HasValue);
			if (obj.HasValue)
			{
				Debug.Log("OnChanged: " + obj.Value);
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
				if(gameObject.tag == "Music")
					SoundManager.instance.ToggleMuteBGMFromUI(obj.Value); // Mute/unmute BGM
				else if (gameObject.tag == "SFX")
					SoundManager.instance.ToggleMuteSFXFromUI(obj.Value); // Mute/unmute BGM
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