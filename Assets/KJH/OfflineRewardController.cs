using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class OfflineRewardController : MonoBehaviour
{
    public Button btn;

    private void Start()
    {
        btn.onClick.AddListener(GetReward);
    }

    public void GetReward()
    {
        var req = new OfflineDataRequest();
        ServerManager.instance.Post<OfflineDataRequest, OfflineDataResponse>(HttpPath.Offline, req, (connect, res) =>
        {
            if (!connect) { return;}
            if (!res.isSuccess) { return; }




        });
    }
    
}
