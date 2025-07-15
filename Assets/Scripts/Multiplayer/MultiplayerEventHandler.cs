using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Word;

public class MultiplayerEventHandler : MonoBehaviour
{
    public bool isMultiplayer = false;
    public string localPlayerID;
    public MatchFound matchFound;
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
        WordServiceContainer.NetworkService.Subscribe<MatchFound>(BaseMessage.MsgType.MatchFound, OnMatchFound);
        WordServiceContainer.NetworkService.Subscribe<PlayerDetails>(BaseMessage.MsgType.playerdetails, (PlayerDetails msg) =>
        {
            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>" + msg.playerID);
            localPlayerID = msg.playerID;
            //Initiate.Fade("MultiplayerSelection", Color.black, 1f);
            isMultiplayer = true;
            SceneManager.LoadScene("Game");
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

    private void OnMatchFound(MatchFound found)
    {
        Debug.Log(found.players.Count);
        matchFound = found;
        isMultiplayer = true;
        SceneManager.LoadScene("Game");
    }
}
