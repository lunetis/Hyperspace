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
        switch(state)
        {
            case State.IDLE:
                if(currentState == state) return;
                animator.SetTrigger("Idle");
                break;

            case State.WALK:
                animator.SetBool("IsRunning", false);
                break;

            case State.RUN:
                animator.SetBool("IsRunning", true);
                break;

            case State.SIT:
                if(currentState == state) return;
                animator.SetTrigger("Sit");
                break;

            case State.DANCING:
                if(currentState == state) return;
                animator.SetTrigger("Dancing");
                break;

            case State.CARRY:
                // animator.CrossFade("Male_Carry", 0.3f);
                break;
        }

        currentState = state;
    }


    public void SetCarry(bool isCarrying)
    {
        animator.SetBool("IsCarrying", isCarrying);
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

    public void Animate(float speed, bool isRunning)
    {
        animator.speed = 1;
        this.isRunning = isRunning;

        animator.SetFloat("Speed", speed);
        if(speed == 0)
        {
            if(isStanding == false)
            {
                isStanding = true;
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
        isStanding = true;
        isRunning = false;
    }
}
