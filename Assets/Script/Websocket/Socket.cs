using UnityEngine;
using System;
using UnityWebSocket;
using UnityEngine.Events;
using System.Collections.Generic;
public class Socket : MonoBehaviour
{
    WebSocket socket = new WebSocket("");
    List<object> players= new();
    public UnityEvent<string> receiveMessage;
    void Start()
    {
        CreatSocket();
        ConnectAsync();
    }
    public int CheckPing(int receiveTime){
        int time = GameData.gameTime - receiveTime;
        return time;
    }
    public void CreatSocket()
    {
        string address = "ws://localhost:8080";
        socket = new WebSocket(address);
        socket.OnOpen += OnOpen;
        socket.OnClose += OnClose;
        socket.OnMessage += OnMessage;
        socket.OnError += OnError;
    }
    void OnOpen(object sender, EventArgs e)
    {
        Debug.Log("open");
    }
    void OnClose(object sender, EventArgs e)
    {

    }
    void OnMessage(object sender, MessageEventArgs e)
    {
        receiveMessage.Invoke(e.Data);
    }
    void OnError(object sender, EventArgs e)
    {

    }
    public void ConnectAsync()
    {
        socket.ConnectAsync();
    }
    public void SendAsync(string str)
    {
        socket.SendAsync(str);
    }
    public void CloseAsync()
    {
        socket.CloseAsync();
    }
    public string Serialization<T>(T Data)
    {
        string serializedData = JsonUtility.ToJson(Data);
        return serializedData;
    }
    public T Deserialization<T>(string data)
    {
        T deserializedData = JsonUtility.FromJson<T>(data);
        return deserializedData;
    }
}
public class Packet
{
    public Type type;
    public Title title;
    public string message;

    public Packet()
    {
    }

    public Packet(Type type, Title title, string message)
    {
        this.type = type;
        this.title = title;
        this.message = message;
    }
}
[Flags]
public enum Type
{
    System = 0,
    Game = 1,
}
[Flags]
public enum Title
{
    CreatThisPlayer = 0,
    DeleteThisPlayer = 1,
    CheckPing = 2,
    SendKeyLog = 0
}