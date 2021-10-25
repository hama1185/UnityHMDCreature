using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public enum State{
    Intro,
    Guide,
    Foreshadowing,
    Reaction,
    Fin
}

public class SectionManager : MonoBehaviour
{
    QuoteManager quoteManager;
    private State currentState;
    float sceneTime = 2.5f;
    
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
        // セリフの番号をリセット
        quoteManager.resetQuoteNumber();
        switch(state) {
            case State.Intro:
                // セリフ
                Observable.Interval(System.TimeSpan.FromSeconds(sceneTime * 2))
                    .Take(quoteManager.sectionMaxNumber((int)currentState))
                    .Subscribe(_ =>
                    {
                        quoteManager.nextQuote((int)currentState);
                    }
                ).AddTo(this);
                
            break;

            case State.Guide:
                // 心音
                // 視線誘導
                // 人がドアが入ってくる
                // セリフ
            break;

            case State.Foreshadowing:
                // 視線誘導
                // セリフ
                // 物を落とす
            break;

            case State.Reaction:
                // セリフ
                // 視線誘導
            break;

            case State.Fin:
                // セリフ
            break;

            default:
            break;
        }
    } 
}
