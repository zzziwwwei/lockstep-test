using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {

        }
        timer = createTimer();
    }
    void OnStartGame()
    {
        startGame = true;
    }
    void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    Func<int> createTimer()
    {
        int time = 30;
        Func<int> reduce = () =>
            {
                time--;
                return time;
            };
        return reduce;
    }
    Func<int> timer;
    int reloadScene;
    void Update()
    {
        if (Input.gyro.userAcceleration.y > 0.15f)
        {
            arrowKey |= ArrowKey.UP;
        }
        if (Input.gyro.userAcceleration.y > 1.2f)
        {
            reloadScene++;
        }
        if (reloadScene > 3)
        {
            ReloadScene();
        }
        if (Input.gyro.gravity.x > 0.05f)
        {
            arrowKey |= ArrowKey.RIGHT;
        }
        if (Input.gyro.gravity.x < -0.05f)
        {
            arrowKey |= ArrowKey.LEFT;
        }
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
        if (timer() > 0)
        {
            timer();
        }
        else
        {
            reloadScene = 0;
            timer = createTimer();
        }
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
