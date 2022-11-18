using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPolyAnimationScript : MonoBehaviour
{
    public enum State
    {
        IDLE,
        WALK,
        RUN,
        SIT,
        DANCING,
        CARRY
    };


    public Animator animator;

    [HideInInspector]
    public float speed;

    [SerializeField]
    float maxWalkSpeed = 2.0f;
    
    [SerializeField]
    float maxRunSpeed = 4.0f;

    State currentState;

    [HideInInspector]
    public bool isRunning;
    
    [HideInInspector]
    bool isStanding;
    
    [HideInInspector]
    public bool isCarrying;
    
    void ChangeState(State state)
    {
        if(currentState == state)
            return;

        currentState = state;
        switch(state)
        {
            case State.IDLE:
                animator.CrossFade("Male_Idle", 0.3f);
                break;

            case State.WALK:
                animator.CrossFade("Male_Walk", 0.3f);
                break;

            case State.RUN:
                animator.CrossFade("Male_Run", 0.3f);
                break;

            case State.SIT:
                animator.CrossFade("Male_Sit", 0.3f);
                break;

            case State.DANCING:
                animator.CrossFade("Male_Dancing", 0.3f);
                break;

            case State.CARRY:
                animator.CrossFade("Male_Carry", 0.3f);
                break;
        }
    }

    public void SetMotion(int keyCode)
    {
        switch(keyCode)
        {
            case 1:
                ChangeState(State.IDLE);
                break;
            case 2:
                ChangeState(State.DANCING);
                break;
            case 3:
                ChangeState(State.SIT);
                break;
        }
    }

    void Animate()
    {
        animator.speed = 1;
        if(speed == 0)
        {
            if(isStanding == false)
            {
                isStanding = true;
                ChangeState(State.IDLE);
            }
        }
        else
        {
            isStanding = false;
            if(isRunning == true)
            {
                ChangeState(State.RUN);
                animator.speed = (speed / maxRunSpeed) * 0.5f + 0.5f;
            }
            else
            {
                ChangeState(State.WALK);
                animator.speed = (speed / maxWalkSpeed) * 0.5f + 0.5f;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }
}
