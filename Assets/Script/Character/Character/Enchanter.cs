using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enchanter : Controller
{
    public GameObject upperbody;
    public GameObject lowerbody;

    List<ActionEvent> actionList = new();
    int jumping;
    void Start()
    {
        command_channel.AddListener(OnCommand);
        //rollback_channel.AddListener(OnRollback);
        Set_jump_ac(30, 0.05f);

    }
    void FixedUpdate()
    {

        for (int i = 0; i < actionList.Count; i++)
        {
            var action = actionList[i];
            if (!action.exist)
            {
                actionList.Remove(action);
            }
            else
            {
                action.update_action(action.ACT);
                if (action.ACT == action.ATT)
                {
                    action.exist = false;
                }
            }
            action.ACT++;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D");
    }

    //跳躍回滾判定_尚未實現
    void RollBack_Event(bool rollback)
    {
        for (int i = 0; i < actionList.Count; i++)
        {
            var action = actionList[i];
            if (action.Event == true)
            {
                if (rollback)
                {
                    action.ACT--;
                    if (action.ACT == 0)
                    {
                        action.exist = true;
                    }
                }
                else
                {
                    action.ACT++;
                    if (action.ACT >= action.ATT)
                    {
                        action.ACT = action.ATT;
                        action.exist = true;
                    }
                }

            }
        }
    }
    void OnRollback(string rollback)
    {
        switch (rollback)
        {
            case "OnRollback":
                RollBack_Event(true);
                //Debug.Log("OnRollback");
                break;
            case "OnReback":
                RollBack_Event(false);
                //Debug.Log("OnReback");
                break;
        }
    }
    //--------------------------------------------------


    void OnCommand(Command command)
    {
        ArrowKey arrowKey = command.keyLog.arrowKey;
        bool isRollBack = command.isRollback;
        int command_frame = command.command_frame;
        if (arrowKey.HasFlag(ArrowKey.DOWN))
        {
            Crouch(true);
        }
        else
        {
            if (arrowKey.HasFlag(ArrowKey.RIGHT))
            {
                Move(isRollBack, "RIGHT");
            }
            if (arrowKey.HasFlag(ArrowKey.LEFT))
            {
                Move(isRollBack, "LEFT");
            }
            if (arrowKey.HasFlag(ArrowKey.UP))
            {
                if (!isJump)
                {
                    isJump = true;
                    Jump(isRollBack, GameData.gameTime - command_frame);
                }
            }
        }
        if (arrowKey.HasFlag(ArrowKey.DOWN_Up))
        {
            Crouch(false);
        }
    }
    float[] jump_ac;
    bool isJump = false;
    void Set_jump_ac(int high, float speed)
    {
        jump_ac = new float[high];
        float o = this.transform.position.y;
        int height = jump_ac.Length / 2;
        for (int i = 0; i < height; i++)
        {
            jump_ac[i] = o;
            jump_ac[jump_ac.Length - 1 - i] = o;
            o += 0.1f * (height - i) * (height - i) * speed;
        }

    }
    void Jump_ac(int t)
    {
        this.transform.position = new Vector3(this.transform.position.x, jump_ac[t], this.transform.position.z);

        if (t == (jump_ac.Length - 1))
        {
            onHit_channel.Invoke(new OnHit(0,this.gameObject,false));
            isJump = false;
        }


    }
    void Jump(bool isRollBack, int o)
    {
        onHit_channel.Invoke(new OnHit(0,this.gameObject,true));
        if (!isRollBack)
        {
            actionList.Add(new ActionEvent(Jump_ac, o, jump_ac.Length - 1, true));
        }

    }
    void Move(bool isRollBack, string direction)
    {

        Method move;
        switch (direction)
        {

            case "RIGHT":
                move = (!isRollBack) ? ActionRight : ActionLeft;
                actionList.Add(new ActionEvent(move, 0, 0, false));
                break;
            case "LEFT":
                move = (!isRollBack) ? ActionLeft : ActionRight;
                actionList.Add(new ActionEvent(move, 0, 0, false));
                break;
        }
    }
    void ActionRight(int t)
    {
        this.transform.position = this.transform.position + new Vector3(0.1f, 0, 0);
    }
    void ActionLeft(int t)
    {
        this.transform.position = this.transform.position + new Vector3(-0.1f, 0, 0);
    }

    void Crouch(bool isCrouch)
    {
        if (isCrouch)
        {
            upperbody.GetComponent<BoxCollider2D>().enabled = false;
            upperbody.transform.localPosition = new Vector3(0, -1, 0);

        }
        else
        {
            upperbody.transform.localPosition = new Vector3(0, 0, 0);
            upperbody.GetComponent<BoxCollider2D>().enabled = true;
        }

    }
}