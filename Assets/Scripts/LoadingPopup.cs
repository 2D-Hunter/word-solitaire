using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPopup : MonoBehaviour
{
    private void Start()
    {
        if(InitManager.instance.deleteData)
        {
            Invoke("DataDeleted", 3f);
        }
    }
    void DataDeleted()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.loading);
        PopupManager.instance.TogglePopup(PopupManager.instance.accountDeletedPopup);
    }
}
