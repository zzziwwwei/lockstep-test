using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public static class GameData
{
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        SceneManager.sceneLoaded += (_, _) => Reset();
    }
    public static int gameTime = 0;
    public static int inputTimeFrame = 0;
    public static int inputDelay = 0;
    public static Action<int> fakePing;
    public static Action startGame;
    public static void Reset()
    {
        gameTime = 0;
        inputTimeFrame = 0;
        inputDelay = 0;
        fakePing = null;
        startGame = null;
    }
}
public class Command
{
    public bool isRollback;

    public int command_frame;
    public KeyLog keyLog;
    public Command(bool isRollback, ArrowKey arrowKey, AttackKey attackKey, int command_frame)
    {
        this.isRollback = isRollback;
        this.keyLog = new KeyLog();
        keyLog.arrowKey = arrowKey;
        keyLog.attackKey = attackKey;
        this.command_frame = command_frame;

    }
}

public class GameHandler : MonoBehaviour
{


    public PlayerHandler playerHandler;
    public GameLog gameLog;
    public InGame inGame;
    bool gameStart;
    void Start()
    {
        GameData.startGame += OnStartGame;
        gameLog.rollback.AddListener(OnRollback);
        gameLog.reback.AddListener(OnReback);
        testCreatplayer(0);
        testCreatplayer(1);
        inGame.Init();
    }

    void FixedUpdate()
    {
        if (gameStart)
        {
            ReadCommand();
        }
    }
    void OnStartGame()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        AssignChannel();                //gameLog連接對應player
        while (GameData.inputDelay > 0)
        {
            yield return new WaitForFixedUpdate();
            GameData.inputDelay--;
        }
        gameStart = true;

    }
    public List<UnityEvent<Command>> channels = new();   //對應player頻道
    public List<UnityEvent<string>> rollback_channels = new();
    void AssignChannel()
    {
        foreach (var character in playerHandler.characters)
        {
            channels.Add(new UnityEvent<Command>());
            channels[channels.Count - 1] = character.GetComponent<Controller>().command_channel;
            rollback_channels.Add(new UnityEvent<string>());
            rollback_channels[rollback_channels.Count - 1] = character.GetComponent<Controller>().rollback_channel;
        }
    }
    void ReadCommand()
    {

        for (int player = 0; player < gameLog.logList.Count; player++)
        {

            var log = gameLog.logList[player].keyLogs[GameData.gameTime];

            if (log.arrowKey == ArrowKey.NULL)
            {
                gameLog.Predict(player, GameData.gameTime);
            }
            channels[player].Invoke(new Command(false, log.arrowKey, log.attackKey, GameData.gameTime));
        }
        Debug.Log("ReadCommand:" + GameData.gameTime);
        GameData.gameTime++;
    }

    void OnRollback(int player, int toRollbackTime, int gameTime)
    {
        for (int rollbackTime = gameTime; rollbackTime >= toRollbackTime; rollbackTime--)
        {
            var log = gameLog.logList[player].keyLogs[rollbackTime];
            channels[player].Invoke(new Command(true, log.arrowKey, log.attackKey, rollbackTime));
            rollback_channels[player].Invoke("OnRollback");
            gameLog.RemovePredict(player, rollbackTime);
            Debug.Log("OnRollback:" + rollbackTime);
        }

    }
    void OnReback(int player, int reRollbackTime, int gameTime)
    {
        gameLog.Predict(player, gameTime);
        for (int rollbackTime = reRollbackTime; rollbackTime <= gameTime; rollbackTime++)
        {
            var log = gameLog.logList[player].keyLogs[rollbackTime];
            channels[player].Invoke(new Command(false, log.arrowKey, log.attackKey, rollbackTime));
            rollback_channels[player].Invoke("OnReback");
            Debug.Log("OnReback:" + rollbackTime);
        }
    }
    void testCreatplayer(int playerID)
    {
        PlayerData playerData = new();
        playerData.playerID = playerID;
        playerData.playerName = "player1";
        playerData.character = "Enchanter";
        playerHandler.CreatPlayer(playerData);
        gameLog.CreatPlayerLogs();
    }

}