using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GIE
{

    public class Example : MonoBehaviour
    {
        public GetItemEffectType mGetItemEffectType = GetItemEffectType.Explostion_First;
        public string mItemName = "coin";
        public int mItemNumber = 10;
        public Text mItemNumberText;
        public Canvas rootCanvas;
        public float yOffsetPx = -200f;
        public int coinCntr = 0;

        private void Start()
        {
            StartCoroutine(ShowAnim());
        }
        IEnumerator ShowAnim()
        {
            yield return new WaitForSeconds(0.5f);
            AnimateCoinsFromCanvasCenter();
        }

        public void OnSetNumber( float number )
        {
            mItemNumber = (int)number;
            mItemNumberText.text = "Number:" + mItemNumber.ToString();
        }

        public void OnSetExplostion( bool set_value )
        {
            if( set_value == true ) mGetItemEffectType = GetItemEffectType.Explostion_First;
        }

        public void OnSetJump(bool set_value)
        {
            if (set_value == true) mGetItemEffectType = GetItemEffectType.JumpAway_First;
        }

        public void OnSetFly(bool set_value)
        {
            if (set_value == true) mGetItemEffectType = GetItemEffectType.FlyAway;
        }



        public void OnSetCoin(bool set_value)
        {
            if (set_value == true) mItemName = "coin";
        }

        public void OnSetDiamond(bool set_value)
        {
            if (set_value == true) mItemName = "diamond";
        }

        public void OnSetEquipment(bool set_value)
        {
            if (set_value == true) mItemName = "equipment";
        }



        public void OnClickMoney( RectTransform from_where )
        {
            GetItemEffect.mInstance.GetItem(mItemName, mItemNumber, new Vector3(540, 960, 0),null, mGetItemEffectType);
        }
        //public void AnimateCoinsFromCanvasCenter()
        //{
        //    mItemName = "coin";
        //    mItemNumber = 25;
        //    mGetItemEffectType = GetItemEffectType.Explostion_First;
        //    //mGetItemEffectType = GetItemEffectType.JumpAway_First;
        //    //mGetItemEffectType = GetItemEffectType.FlyAway;
        //    var canvasRect = (RectTransform)rootCanvas.transform;

        //    // local center, then push down
        //    Vector3 localCenter = canvasRect.rect.center + new Vector2(0f, -yOffsetPx);

        //    // convert to world for UI objects
        //    Vector3 worldCenter = canvasRect.TransformPoint(localCenter);

        //    GetItemEffect.mInstance.GetItem(mItemName, mItemNumber, worldCenter, null, mGetItemEffectType);
        //}
        public void AnimateCoinsFromCanvasCenter()
        {
            if(InitManager.instance.CurrentScene =="Levelup")
            {
                yOffsetPx = -650f;
            }
            mItemName = "coin";
            mItemNumber = 25;
            mGetItemEffectType = GetItemEffectType.Explostion_First;

            var canvasRect = (RectTransform)rootCanvas.transform;

            // Start at the canvas local center, then shift by yOffsetPx ( + up, - down )
            Vector3 localCenter = canvasRect.rect.center;
            localCenter.y += yOffsetPx;

            // Convert to world for UI objects
            Vector3 worldCenter = canvasRect.TransformPoint(localCenter);

            GetItemEffect.mInstance.GetItem(mItemName, mItemNumber, worldCenter, null, mGetItemEffectType);
        }


        public void OnClick3DObject( BaseEventData eventData )
        {
            //Debug.Log("OnClick3DObject:" +  ((PointerEventData)eventData).position);
            Vector2 position = ((PointerEventData)eventData).position;
            
            GetItemEffect.mInstance.GetItem(mItemName, mItemNumber, new Vector3(position.x, position.y, 0), null, mGetItemEffectType);
        }

    }

}

