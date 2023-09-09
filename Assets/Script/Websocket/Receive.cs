
using UnityEngine;
using UnityEngine.Events;
public class Receive : MonoBehaviour
{
    public Socket socket;
    public UnityEvent<FrameLog> inputKey = new();
    void Start()
    {
        socket.receiveMessage.AddListener(OnMessage);

    }
    void FixedUpdate()
    {

    }
    void OnMessage(string message)
    {
        try
        {
            Packet packet = socket.Deserialization<Packet>(message);
            Unpack(packet);

        }
        catch
        {
            Debug.Log("反序列失敗");
        }

    }
    void Unpack(Packet packet)
    {
        switch (packet.type)
        {
            case Type.System:
                switch (packet.title)
                {
                    case Title.CreatThisPlayer:
                        break;
                    case Title.DeleteThisPlayer:
                        break;
                    case Title.CheckPing:
                        CheckPing();
                        break;
                }
                break;
            case Type.Game:
                switch (packet.title)
                {
                    case Title.SendKeyLog:
                        ReceiveKeyLog(packet.message);
                        break;
                }
                break;
        }

    }
    void ReceiveKeyLog(string message)
    {
        FrameLog data = socket.Deserialization<FrameLog>(message);
        //Debug.Log("rec"+data.keyLog.arrowKey+"/f"+data.currentFrame);
        inputKey.Invoke(data);
    }
    void CheckPing(){
        
    }

}
