using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;

public class GameModeSelection : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject _modeSelectionUI;
    [SerializeField]
    private GameObject _matchMakingUI;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickMultiMode()
    {
        WordServiceContainer.NetworkService.Subscribe<PlayerDetails>(BaseMessage.MsgType.playerdetails, (PlayerDetails msg) =>
        {
            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>"+  msg.playerID);
            MatchMakingRequest matchMakingRequest = new MatchMakingRequest();
            matchMakingRequest.playerId = msg.playerID;
            matchMakingRequest.Type = BaseMessage.MsgType.MatchMaking;
            var makingRequest = JsonConvert.SerializeObject(matchMakingRequest);
            NetworkMessege message = new NetworkMessege();
            message.Msg = makingRequest;
            message.Type = BaseMessage.MsgType.MatchMaking;
            WordServiceContainer.NetworkService.Send<NetworkMessege>(message);
        });

        string url = $"ws://ec2-52-43-3-186.us-west-2.compute.amazonaws.com:8770/word";
        WordServiceContainer.NetworkService.Connect(url, () =>
        {
            Debug.Log("Onconeect to server >>>>>>>");
        });
    }
}
