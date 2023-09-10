using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class Log
{
    public KeyLog[] keyLogs;
    public Log(int size)
    {
        this.keyLogs = new KeyLog[size];
        for (int i = 0; i < this.keyLogs.Length; i++)
        {
            this.keyLogs[i] = new KeyLog(); // 使用 KeyLog 的預設建構函式初始化每個元素
        }
    }
}

public class GameLog : MonoBehaviour
{
    public List<Log> logList = new();
    public UnityEvent<int, int, int> rollback;
    public UnityEvent<int, int, int> reback;
    public Receive receive;
    public LocalInput localInput;
    public PingBuffer pingBuffer;
    void Start()
    {
        GameData.startGame += OnStartGame;
        localInput.inputKey.AddListener(InputKey);
        pingBuffer.inputKey.AddListener(NetInputKey);
        // receive.inputKey.AddListener(NetInputKey);
    }
    void OnStartGame()
    {

    }

    public void CreatPlayerLogs()
    {
        Log log = new Log(60 * 60);
        log.keyLogs[0].arrowKey = ArrowKey.NONE;
        logList.Add(log);
        //Debug.Log("CreatPlayerLogs");
    }
    void InputKey(FrameLog frameLog)
    {
        var log = logList[frameLog.playerId].keyLogs[frameLog.currentFrame];
        if (log.arrowKey != ArrowKey.NULL)
        {
            if (log.arrowKey != frameLog.keyLog.arrowKey)
            {
                Debug.Log("rollback://" + "pre_key:" + log.arrowKey + "/new_key" + frameLog.keyLog.arrowKey + "/f:" + frameLog.currentFrame + "/gametime:" + GameData.gameTime);
                rollback.Invoke(frameLog.playerId, frameLog.currentFrame, GameData.gameTime - 1);
                log.arrowKey = frameLog.keyLog.arrowKey;
                log.attackKey = frameLog.keyLog.attackKey;
                reback.Invoke(frameLog.playerId, frameLog.currentFrame, GameData.gameTime - 1);
            }
            else
            {
                //Debug.Log("same");
            }
        }
        logList[frameLog.playerId].keyLogs[frameLog.currentFrame].arrowKey = frameLog.keyLog.arrowKey;
    }


    void NetInputKey(FrameLog data)
    {
        InputKey(data);
    }

    public void Predict(int player, int currentFrame)
    {

        int predictRange = 0;
        while (logList[player].keyLogs[currentFrame - predictRange].arrowKey == ArrowKey.NULL)
        {
            predictRange++;
            if (currentFrame - predictRange < 0)
            {
                break;
            }
        }
        if (currentFrame - predictRange >= 0)
        {
            var preLog = logList[player].keyLogs[currentFrame - predictRange];
            for (int i = currentFrame - predictRange + 1; i <= currentFrame; i++)
            {
                Debug.Log("Predict" + player + "/" + i);
                logList[player].keyLogs[i].arrowKey = preLog.arrowKey;
                logList[player].keyLogs[i].attackKey = preLog.attackKey;
            }
        }
    }
    public void RemovePredict(int player, int f)
    {
        logList[player].keyLogs[f].arrowKey = ArrowKey.NULL;
    }

}
