using UnityEngine;
using System.Collections;

namespace TapToBlast
{
    public class AutoDestruct : MonoBehaviour 
    {

        public float duration = 1;
        public bool OnlyDeactivate;

        void OnEnable()
        {
            StartCoroutine("DestroyObj");
        }

        IEnumerator DestroyObj()
        {
            yield return new WaitForSeconds(duration);

            if (OnlyDeactivate)
            {
#if UNITY_3_5
				this.gameObject.SetActiveRecursively(false);
#else
                this.gameObject.SetActive(false);
#endif
            }
            else
                GameObject.Destroy(this.gameObject);
        }
    }
}