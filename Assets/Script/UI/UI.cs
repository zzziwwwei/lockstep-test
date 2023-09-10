
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
public class UI : MonoBehaviour
{

    public class State_Equal<T>
    {
        public T oldState;

        public bool Equals(T state)
        {
            bool equals = EqualityComparer<T>.Default.Equals(state, this.oldState);
            this.oldState = state;
            return equals;
        }
    }
    public LocalInput localInput;
    public GameObject _InputLog;
    public TMP_Text[] InputLogs;
    public TMP_Text InputLog;
    public TMP_InputField inputdelay;
    public Button gameStart;
    public Slider fakePing;
    public GameObject hide;
    public TMP_Text ping;
    public State_Equal<ArrowKey> stateEqual = new();
    void Start()
    {
        CreatInputLog();
        localInput.inputKey.AddListener(OnInputKey);
        gameStart.onClick.AddListener(GameStart);
        fakePing.minValue = 0;
        fakePing.maxValue = 40;
        fakePing.onValueChanged.AddListener(FakePing);
    }
 
    void FakePing(float value)
    {
        ping.text = "inputDelay:"+(int)(o)+"\n"+"ping:" +(int)(value*3.3f)+"ms";
        GameData.fakePing.Invoke((int)value);
    }
    int o;
    void GameStart()
    {
        Debug.Log("button");
        string inputText = inputdelay.text; // 获取输入字段的文本内容
        
        bool success = int.TryParse(inputText, out o);
        if (success)
        {
            
            GameData.inputDelay = (int)(o/3.3f);
        }
        hide.SetActive(false);

        GameData.startGame.Invoke();
    }
    void CreatInputLog()
    {
        InputLogs = new TMP_Text[25];
        Vector2 offset = new Vector2(InputLog.rectTransform.anchoredPosition.x, InputLog.rectTransform.anchoredPosition.y);

        for (int i = 0; i < 25; i++)
        {
            InputLogs[i] = Instantiate(InputLog, _InputLog.transform);
            InputLogs[i].rectTransform.anchoredPosition = offset + new Vector2(0, -(i * InputLog.rectTransform.rect.height + 1));
        }
    }
    int c = 0;
    void OnInputKey(FrameLog frameLog)
    {
        ArrowKey inputkey = frameLog.keyLog.arrowKey;
        ArrowKey mask = (ArrowKey.UP | ArrowKey.DOWN | ArrowKey.RIGHT | ArrowKey.LEFT);
        ArrowKey key = inputkey & mask;
        if (stateEqual.Equals(key) == false)
        {
            c = 1;
            for (int i = 24; i > 0; i--)
            {
                InputLogs[i].text = InputLogs[i - 1].text;
            }
            InputLogs[0].text = "f:" + c + "  Key:" + key.ToString();

        }
        else
        {
            c++;
            InputLogs[0].text = "f:" + c + "  Key:" + key.ToString();
        }

    }




}
