using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Send : MonoBehaviour
{
    
    Packet packet = new();
    public LocalInput localInput;
    public Socket socket;

    FrameLog data = new();
    void Start()
    {
        GameData.startGame += OnStartGame;
        localInput.inputKey.AddListener(SendKeyLog);
    }
    void OnStartGame()
    {
       
    }
    void SendKeyLog(FrameLog frameLog){
        data.playerId = 1;//test
        data.keyLog = frameLog.keyLog;//test
        data.currentFrame = frameLog.currentFrame;//test
        packet.type = Type.Game;
        packet.title = Title.SendKeyLog;
        packet.message = socket.Serialization(data);
        socket.SendAsync(socket.Serialization(packet));
    }
    void CheckPing(){
        Packet packet = new(Type.System,Title.CheckPing,"");
        socket.SendAsync(socket.Serialization(packet));
    }
}
