using System;
using System.Text;
using System.Threading;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class NetworkService : INetworkService
{
    private WebSocket _webSocket;
    private string _url;
    private Action _onConnected;
    private Timer _pingTimer;
    private bool _isManuallyClosed;

    public event Action<string> OnRawMessageReceived;
    public event Action OnDisconnected;
    public event Action<Exception> OnError;
    private readonly Dictionary<BaseMessage.MsgType, List<Delegate>> _typeHandlers = new();
    public void Connect(string url, Action onConnected)
    {
        _url = url;
        _onConnected = onConnected;
        _isManuallyClosed = false;

        InitWebSocket();
    }

    private void InitWebSocket()
    {
        _webSocket = new WebSocket(_url);

        _webSocket.OnOpen += (sender, e) =>
        {
            _onConnected?.Invoke();
            _pingTimer = new Timer(SendPing, null, 5000, 5000); // ping every 5 sec
        };

        _webSocket.OnMessage += (sender, e) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                Debug.Log("Main Thread Received: " + e.Data);
                HandleIncomingMessage(e.Data);
            });
           
        };

        _webSocket.OnClose += (sender, e) =>
        {
            _pingTimer?.Dispose();
            OnDisconnected?.Invoke();

            if (!_isManuallyClosed)
            {
                // attempt reconnect after short delay
                Thread.Sleep(2000);
                InitWebSocket();
                _webSocket.ConnectAsync();
            }
        };

        _webSocket.OnError += (sender, e) =>
        {
            OnError?.Invoke(e.Exception);
        };

        _webSocket.ConnectAsync();
    }

    private void SendPing(object state)
    {
        if (_webSocket != null && _webSocket.IsAlive)
        {
            _webSocket.Ping();
        }
    }

    public void Send<T>(T message)
    {
        if (_webSocket != null && _webSocket.IsAlive)
        {
            string json = JsonConvert.SerializeObject(message);
            Debug.Log("sending Msg "+json);
            _webSocket.Send(json);
        }
    }
    public void Subscribe<T>(BaseMessage.MsgType type, Action<T> callback)
    {
        if (!_typeHandlers.ContainsKey(type))
        {
            _typeHandlers[type] = new List<Delegate>();
        }

        _typeHandlers[type].Add(callback);
    }

    private void HandleIncomingMessage(string json)
    {
        try
        {
            var baseMsg = JsonConvert.DeserializeObject<NetworkMessege>(json);

            if (baseMsg != null)
            {
                Debug.Log("Main Thread Received: 2" + json);
                if (_typeHandlers.TryGetValue(baseMsg.Type, out var handlers))
                {
                    Debug.Log("Main Thread Received: 3" + json);
                    foreach (var handler in handlers)
                    {
                        var targetType = handler.Method.GetParameters()[0].ParameterType;
                        Debug.Log("Main Thread Received: 4" + baseMsg.Msg);
                        Debug.Log("Main Thread Received: 5 " + targetType);
                        var typedObj = JsonConvert.DeserializeObject(baseMsg.Msg, targetType);
                        Debug.Log("Main Thread Received: 5 " + typedObj);
                        handler.DynamicInvoke(typedObj);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }
    public void Close()
    {
        _isManuallyClosed = true;
        _pingTimer?.Dispose();
        _webSocket?.Close();
    }
}
