using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State{
    Intro,
    Guide,
    Foreshadowing,
    Reaction,
    Fin
}

public class SectionManager : MonoBehaviour
{
    private State currentState;
    
    void Start() {
        
    }

    void Update() {
        
    }

    // 他スクリプトから参照する時
    public State CurrentState
    {
        get
        {
            return currentState;
        }
    }

    // ステートの値を変更するとき
    public void SetCurrentState(State state) {
        currentState = state;
        OnStateChanged(currentState);
    }
    
    // ステートの値が変わったとき
    void OnStateChanged(State state) {
        switch(state) {
            case State.Intro:
            break;

            case State.Guide:
            break;

            case State.Foreshadowing:
            break;

            case State.Reaction:
            break;

            case State.Fin:
            break;

            default:
            break;
        }
    } 
}
