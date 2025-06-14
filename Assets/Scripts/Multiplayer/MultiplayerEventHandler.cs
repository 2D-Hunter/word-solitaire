using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;

public class MultiplayerEventHandler : MonoBehaviour
{
    public string localPlayerID;
    private static MultiplayerEventHandler instance;
    public static MultiplayerEventHandler Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
    }

    public void SubscribeMultiplayerEvents()
    {
        Debug.Log("____SubscribeMultiplayerEvents");
        WordServiceContainer.NetworkService.Subscribe<PlayerDetails>(BaseMessage.MsgType.playerdetails, (PlayerDetails msg) =>
        {
            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>" + msg.playerID);
            localPlayerID = msg.playerID;
            Initiate.Fade("MultiplayerSelection", Color.black, 1f);
            MatchMakingRequest matchMakingRequest = new MatchMakingRequest();
            matchMakingRequest.playerId = msg.playerID;
            matchMakingRequest.Type = BaseMessage.MsgType.MatchMaking;
            var makingRequest = Newtonsoft.Json.JsonConvert.SerializeObject(matchMakingRequest);
            NetworkMessege message = new NetworkMessege();
            message.Msg = makingRequest;
            message.Type = BaseMessage.MsgType.MatchMaking;
            WordServiceContainer.NetworkService.Send<NetworkMessege>(message);

            
        });
        
    }
}
