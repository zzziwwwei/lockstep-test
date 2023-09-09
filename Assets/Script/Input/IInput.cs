using System;
using UnityEngine.Events;
[Flags]
[Serializable]
public enum ArrowKey
{
    NULL = 0,
    UP = 1,
    DOWN = 2,
    LEFT = 4,
    RIGHT = 8,
    UP_Up = 16,
    DOWN_Up = 32,
    LEFT_Up = 64,
    RIGHT_Up = 128,
    NONE = 256,
}
[Flags]
[Serializable]
public enum AttackKey
{
    NONE = 0
}
[Serializable]
public class KeyLog
{
    public ArrowKey arrowKey;
    public AttackKey attackKey;
    public KeyLog()
    {
        this.arrowKey = new();
        this.attackKey = new();
    }
}
[Serializable]
public class FrameLog
{
    public int playerId;
    public int currentFrame;
    public KeyLog keyLog;
    public FrameLog()
    {
        this.keyLog = new KeyLog();
    }

}
