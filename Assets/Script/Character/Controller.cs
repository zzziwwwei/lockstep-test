using UnityEngine.Events;
using UnityEngine;
using System;
public class OnHit
{
    public int onHitID;
    public GameObject hitter;

    public bool on;
    public OnHit(int onHitID,GameObject hitter,bool on){
        this.onHitID = onHitID;
        this.hitter = hitter;
        this.on = on;
    }
}
public abstract class Controller : MonoBehaviour
{
    public UnityEvent<Command> command_channel;
    public UnityEvent<string> rollback_channel;
    public UnityEvent<OnHit> onHit_channel;

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

