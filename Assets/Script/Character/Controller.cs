using UnityEngine.Events;
using UnityEngine;
using System;
public class OnHitEventData
{
    public int onHitID;
    public GameObject hitter;
    public Trigger trigger;
    public OnHitEventData(int onHitID, Trigger trigger)
    {
        this.onHitID = onHitID;
        this.trigger = trigger;
    }
    public enum Trigger
    {
        Enter = 0,
        Stay = 1,
        Leave = 2
    }
}
public abstract class Controller : MonoBehaviour
{
    public UnityEvent<Command> command_channel;
    public UnityEvent<string> rollback_channel;
    public UnityEvent<OnHitEventData> onHit_channel;

    public delegate void Method(int t);
    public class ActionEvent
    {
        public int ACT; // action current runtime
        public int ATT; //actions take time
        public Method update_action;
        public bool Event;
        public bool exist = true;
        public ActionEvent(Method update_action, int ACT, int ATT, bool Event)
        {
            this.update_action = update_action;
            this.ACT = ACT;
            this.ATT = ATT;
            this.Event = Event;
        }
    }


}

