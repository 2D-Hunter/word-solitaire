using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkService 
{
    void Connect(string url, Action onConnected);
    void Send<T>(T message);
    void Close();
    void Subscribe<T>(BaseMessage.MsgType type, Action<T> callback);
    event Action OnDisconnected;
    event Action<Exception> OnError;
    void GetGameData(string url, Action<bool, string> callBack);


}
