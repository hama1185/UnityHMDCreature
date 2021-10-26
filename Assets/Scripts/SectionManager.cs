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

    public GameObject _Pusher, _GazeGuide;
    push push;
    Gazeguidance gazeguidance;
    
    void Awake(){
        push = _Pusher.GetComponent<push>();
        gazeguidance = _GazeGuide.GetComponent<Gazeguidance>();
    }

    void Start() {
        quoteManager = this.GetComponent<QuoteManager>();
        SetCurrentState(State.Intro);
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
                DoIntro();
            break;
            // タイマーは並列で動くので時間の管理がだるい
            case State.Guide:
                DoGuide();
            break;

            case State.Foreshadowing:
                DoForeshadowing();
            break;

            case State.Reaction:
                DoReaction();
            break;

            case State.Fin:
                DoFin();
            break;

            default:
            break;
        }
    }

    // それぞれの動作
    void DoIntro(){
        // セリフ
        Observable.Interval(System.TimeSpan.FromSeconds(sceneTime * 2))
            .Take(quoteManager.sectionMaxNumber((int)currentState))
            .DoOnCompleted(() => SetCurrentState(State.Guide))
            .Subscribe(_ =>
            {
                quoteManager.nextQuote((int)currentState);
            }
        ).AddTo(this);
    }

    void DoGuide(){
        Observable.Timer(System.TimeSpan.FromSeconds(sceneTime * 3))
            .Subscribe(_ =>
            {
                // 心音
                Debug.Log("心音");
                // 視線誘導
                Debug.Log("視線誘導");
                gazeguidance.GenerateGuide();

                Observable.Timer(System.TimeSpan.FromSeconds(sceneTime * 2))
                .DoOnCompleted(() => quoteGuide())
                .Subscribe(_ => 
                    {
                        // 人がドアが入ってくる
                        Debug.Log("人が入ってくる");
                        push.pushObject();
                    }
                ).AddTo(this);

                
            }
        ).AddTo(this);
    }

    void DoForeshadowing(){
        Observable.Timer(System.TimeSpan.FromSeconds(sceneTime * 2))
            .Subscribe(_ =>
            {
                // 視線誘導ハンガー
                gazeguidance.GenerateGuide();
                Debug.Log("視線誘導 + Hanger");
                Observable.Interval(System.TimeSpan.FromSeconds(sceneTime))
                    .Take(quoteManager.sectionMaxNumber((int)currentState))
                    .Subscribe(_ =>
                    {
                        quoteManager.nextQuote((int)currentState);
                        
                        Observable.Timer(System.TimeSpan.FromSeconds(sceneTime * 2))
                            .DoOnCompleted(() => SetCurrentState(State.Reaction))
                            .Subscribe(_ =>
                            {
                                // ものを落とす
                                push.pushObject();
                                Debug.Log("物が落ちる");
                            }
                        ).AddTo(this);
                    }
                ).AddTo(this);
            }    
        ).AddTo(this);
    }

    void DoReaction(){
        Observable.Timer(System.TimeSpan.FromSeconds(sceneTime))
            .Subscribe(_ =>
            {
                Observable.Timer(System.TimeSpan.Zero ,System.TimeSpan.FromSeconds(sceneTime * 2))
                    .Take(quoteManager.sectionMaxNumber((int)currentState))
                    .DoOnCompleted(() => SetCurrentState(State.Fin))
                    .Subscribe(x =>
                    {
                        quoteManager.nextQuote((int)currentState);
                        if(x == 0){
                            // ハンガー状態解除
                            Debug.Log("ハンガー解除");
                        }
                        else if(x == 1){
                            // 視線誘導ハンガー
                            gazeguidance.GenerateGuide();
                            Debug.Log("視線誘導 + Hanger");
                            // これが終わった後の時間の間隔を定めたほうがいいかも
                            // 一応2.5秒の間隔を開けている
                        }
                        else{}
                    }
                ).AddTo(this);
            }    
        ).AddTo(this);
    }

    void DoFin(){
        Observable.Interval(System.TimeSpan.FromSeconds(sceneTime * 2))
            .Take(quoteManager.sectionMaxNumber((int)currentState))
            //.DoOnCompleted(() => ハンガー状態解除の関数を書く)
            .Subscribe(_ =>
            {
                quoteManager.nextQuote((int)currentState);
            }
        ).AddTo(this);
    }

    void quoteGuide(){
        Observable.Timer(System.TimeSpan.FromSeconds(sceneTime))
            .Subscribe(_ => 
            {
                Observable.Timer(System.TimeSpan.Zero ,System.TimeSpan.FromSeconds(sceneTime * 2))
                    .Take(quoteManager.sectionMaxNumber((int)currentState))
                    .DoOnCompleted(() => SetCurrentState(State.Foreshadowing))
                    .Subscribe(_ =>
                    {
                        quoteManager.nextQuote((int)currentState);
                    }
                ).AddTo(this);
            }
        ).AddTo(this);
    }
}
