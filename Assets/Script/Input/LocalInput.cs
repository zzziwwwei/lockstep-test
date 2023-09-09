using UnityEngine;
using UnityEngine.Events;

public class LocalInput : MonoBehaviour
{
    public UnityEvent<FrameLog> inputKey = new();
    ArrowKey arrowKey = new();
    AttackKey attackKey = new();
    FrameLog frameLog = new();
    bool startGame = false;
    void Start()
    {
        frameLog.playerId = 0;
        frameLog.currentFrame = 0;
        arrowKey = ArrowKey.NONE;
        attackKey = AttackKey.NONE;
        GameData.startGame += OnStartGame;
    }
    void OnStartGame()
    {
        startGame = true;
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            
        }
        //test
       
        if (Input.GetKey(KeyCode.UpArrow))
        {
            arrowKey |= ArrowKey.UP;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            arrowKey |= ArrowKey.DOWN;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            arrowKey |= ArrowKey.RIGHT;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
           arrowKey |= ArrowKey.LEFT;
        }
        //GetKeyUp;
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            arrowKey |= ArrowKey.UP_Up;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            arrowKey |= ArrowKey.DOWN_Up;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            arrowKey |= ArrowKey.RIGHT_Up;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            arrowKey |= ArrowKey.LEFT_Up;
        }
    }
    int f = 0;
    void FixedUpdate()
    {
        if (arrowKey.HasFlag(ArrowKey.UP) && arrowKey.HasFlag(ArrowKey.DOWN))
        {
            arrowKey ^= ArrowKey.UP;
            arrowKey ^= ArrowKey.DOWN;
            arrowKey |= ArrowKey.UP_Up;
            arrowKey |= ArrowKey.DOWN_Up;
        }
        if (arrowKey.HasFlag(ArrowKey.RIGHT) && arrowKey.HasFlag(ArrowKey.LEFT))
        {
            arrowKey ^= ArrowKey.RIGHT;
            arrowKey ^= ArrowKey.LEFT;
            arrowKey |= ArrowKey.RIGHT_Up;
            arrowKey |= ArrowKey.LEFT_Up;
        }
        frameLog.keyLog.arrowKey = arrowKey;
        frameLog.keyLog.attackKey = attackKey;
        if (startGame)
        {
            frameLog.currentFrame = f;
            inputKey.Invoke(frameLog);
            f++;
        }
        
        arrowKey &= ArrowKey.NONE;
        attackKey &= AttackKey.NONE;
    }


}
