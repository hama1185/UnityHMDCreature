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
    Fin,
    Fin2
}

public class SectionManager : MonoBehaviour
{
    QuoteManager quoteManager;
    private State currentState;
    float sceneTime = 3.5f;

    public GameObject _Pusher, _GazeGuide, _Client, _Hanger, _ServoController, _Noise;
    push push;
    Gazeguidance gazeguidance;
    Client client;
    HangerController hanger;
    ServoController servoController;
    Noising noise;
    ServoServer server;

    public bool onceFlag = false;

    // 音が大きくなっていく間隔時間
    [SerializeField]
    float volUpDeltaTime = 0.5f;

    
    void Awake(){
        client = _Client.GetComponent<Client>();
        hanger = _Hanger.GetComponent<HangerController>();
        push = _Pusher.GetComponent<push>();
        gazeguidance = _GazeGuide.GetComponent<Gazeguidance>();
        servoController = _ServoController.GetComponent<ServoController>();
        noise = _Noise.GetComponent<Noising>();
        server = this.GetComponent<ServoServer>();
    }

    void Start() {
        quoteManager = this.GetComponent<QuoteManager>();
    }

    void Update() {
        if(!onceFlag){
            if(server.playFlag){
                SetCurrentState(State.Intro);
                onceFlag = true;
            }
        }
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

            //case State.Fin2:
                //DoFin2();
            //break;
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
                // 音の再生
                client.SendStart(1);
        
                // 視線誘導
                Debug.Log("視線誘導");
                gazeguidance.GenerateGuide();
                // ハンガー
                hanger.act();
                //音誘導
                noise.MakeNoise();

                Observable.Timer(System.TimeSpan.FromSeconds(sceneTime * 2))
                .DoOnCompleted(() => quoteGuide())
                .Subscribe(_ => 
                    {
                        // 人がドアが入ってくる
                        Debug.Log("人が入ってくる");
                        hanger.act();
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
                hanger.act();
                Debug.Log("視線誘導 + Hanger");
                //音誘導
                noise.MakeNoise();

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
                                // 現実でも落とす
                                servoController.sendopendataUDP();
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
                            hanger.act();
                        }
                        else if(x == 1){
                            // 視線誘導ハンガー
                            gazeguidance.GenerateGuide();
                            hanger.act();
                            Debug.Log("視線誘導 + Hanger");
                            //音誘導
                            noise.MakeNoise();
                            // これが終わった後の時間の間隔を定めたほうがいいかも
                            // 一応2.5秒の間隔を開けている
                        }
                        else{}
                    }
                ).AddTo(this);
            }    
        ).AddTo(this);
    }

    /*
    void DoFin(){
        Observable.Interval(System.TimeSpan.FromSeconds(sceneTime * 2))
            .Take(quoteManager.sectionMaxNumber((int)currentState))
            .DoOnCompleted(() => SetCurrentState(State.Fin2))
            .Subscribe(_ =>
            {
                //quoteManager.nextQuote((int)currentState);
                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
                {
                }).AddTo(this);
            }
        ).AddTo(this);
    }
    */

    void DoFin(){
        Observable.Interval(System.TimeSpan.FromSeconds(sceneTime * 2))
            .Take(quoteManager.sectionMaxNumber((int)currentState))
            .DoOnCompleted(() => killHanger())
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

    void killHanger(){
        hanger.act();
        client.SendStop();
    }
}
