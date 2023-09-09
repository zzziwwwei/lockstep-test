using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class PingBuffer : MonoBehaviour
{
    public UnityEvent<FrameLog> inputKey = new();
    public LocalInput localInput;
    int fakePing = 0;
    int timer = 0;
    Queue<FrameLog> buffer = new();
    void Start()
    {
        localInput.inputKey.AddListener(ReceiveKey);
        GameData.fakePing += (fakePing) => this.fakePing = fakePing;
    }

    void ReceiveKey(FrameLog frameLog)
    {
        FrameLog Log = new();
        Log.playerId = 1;
        Log.currentFrame = frameLog.currentFrame;
        Log.currentFrame =Log.currentFrame;
        Log.keyLog.arrowKey = frameLog.keyLog.arrowKey;
        Log.keyLog.attackKey = frameLog.keyLog.attackKey;
        buffer.Enqueue(Log);
    }
    void FixedUpdate()
    {    
        if (timer >= fakePing)
        {
            while (buffer.Count > 0)
            {
                inputKey.Invoke(buffer.Dequeue());
            }
            timer = 0;
        }
        timer++;
    }

}
