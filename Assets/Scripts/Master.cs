using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// UniRxの参照
using UniRx;
using System;

// このスクリプトでこのプロジェクトを管理する

public class Master : MonoBehaviour
{
    // ブラックアウトするまでの時間
    [SerializeField]
    int blackoutTime = 7;

    // 寄生されるまでの時間
    [SerializeField]
    int parasiteTime = 10;

    // 音が大きくなっていく間隔時間
    [SerializeField]
    float volUpDeltaTime = 0.5f;

    // テンポが速くなっていく間隔時間
    [SerializeField]
    float pitchUpDeltaTime = 0.5f;

    // 色が変わる間隔時間
    [SerializeField]
    float changeColorDeltaTime = 0.5f;

    // ハンガーが始まる時間
    [SerializeField]
    int hangerTime = 50;

    // モノが落ちる時間間隔
    [SerializeField]
    int fallObjTime = 20;

    // 視線誘導オーブ生成時間間隔
    [SerializeField]
    int generateTime = 20;

    // 体験が終了するまでの時間
    [SerializeField]
    int finishTime = 25;

    public bool audioSendFlag = false;
    public bool finishFlag = false;

    public GameObject _Client, _Hanger, _ViewHack, _Head, _Pusher, _GazeGuide;
    Client client;
    HangerController hanger;
    ViewHacking view;
    headAngleControl headAngle;
    push push;
    Gazeguidance gazeguidance;

    void Awake(){
        client = _Client.GetComponent<Client>();
        hanger = _Hanger.GetComponent<HangerController>();
        try{
            view = _ViewHack.GetComponent<ViewHacking>();
        }catch(UnassignedReferenceException e){
            Debug.Log(e);
        }
        headAngle = _Head.GetComponent<headAngleControl>();
        push = _Pusher.GetComponent<push>();
        gazeguidance = _GazeGuide.GetComponent<Gazeguidance>();
    }

    // 起動時処理
    void Start()
    {
        // 序盤の設定
        //設定時間後にブラックアウト
        Observable.Timer(System.TimeSpan.FromSeconds(blackoutTime))
            .Subscribe(_ =>
            {
                view.blackOut();
            }
        ).AddTo(this);

        // 設定時間後に起動
        Observable.Timer(System.TimeSpan.FromSeconds(parasiteTime))
            .Subscribe(_ =>
            {
                
                // 音の再生
                client.SendStart(1);

                // 終了条件の設定
                var endFlag = Observable.EveryUpdate()
                .Where(_ => audioSendFlag);

                // 音のアップ設定 
                Observable.Timer(System.TimeSpan.Zero,System.TimeSpan.FromSeconds(volUpDeltaTime))
                .TakeUntil(endFlag)
                .Subscribe(_ =>
                {
                    client.SendUp();
                }
                ).AddTo(this);

                // 色の変更
                
                Observable.Timer(System.TimeSpan.Zero,System.TimeSpan.FromSeconds(changeColorDeltaTime))
                .TakeUntil(endFlag)
                .Subscribe(x =>
                {
                    // 色が徐々に変わっていく処理
                    try{
                        view.viewHack(x);
                    }catch(NullReferenceException){
                        
                    }
                }
                ).AddTo(this);
                

                // ハンガースタートする設定
                Observable.Timer(System.TimeSpan.FromSeconds(hangerTime))
                .Subscribe(_ =>
                {
                    // 音楽の変化をやめる
                    audioSendFlag = true;
                    // ハンガー反射デバイス起動
                    hanger.act();
                    // フラグを起動
                    headAngle.hangerLeftFlag = true;

                    // 生物を倒すことのコールバックが来たら終盤の設定を起動する
                }
                ).AddTo(this);

                //一定の間隔でモノが落ち始める
                Observable.Timer(System.TimeSpan.Zero,System.TimeSpan.FromSeconds(fallObjTime))
                .TakeUntil(endFlag)
                .DelaySubscription(System.TimeSpan.FromSeconds(10.0f)) //視線誘導オーブがターゲットにたどり着いてからモノを落下させるために購入を遅延
                .Subscribe(_ =>
                {
                    push.pushObject();
                }
                ).AddTo(this);

                //一定の間隔で視線誘導オーブ生成
                Observable.Timer(System.TimeSpan.Zero,System.TimeSpan.FromSeconds(generateTime))
                .TakeUntil(endFlag)
                .Subscribe(_ =>
                {
                    gazeguidance.GenerateGuide();
                }
                ).AddTo(this);
            }
        ).AddTo(this);
    }

    void Update(){
        
        if(headAngle.firstHangerFlag){
                hanger.act();
                Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
                .Subscribe(_ =>
                {
                    hanger.act();
                    headAngle.hangerRightFlag= true;
                }
                ).AddTo(this);

                headAngle.firstHangerFlag = false;
            }
        

        if(headAngle.secondHangerFlag){
            hanger.act();

            //消えていく処理
            vanish();
            headAngle.secondHangerFlag = false;
        }
        
    }

    void vanish(){

        var isFinish = Observable.EveryUpdate()
                .Where(_ => audioSendFlag);

        
        Observable.Timer(System.TimeSpan.FromSeconds(finishTime))
            .Subscribe(_ =>
            {
                finishFlag = true;
            }
        ).AddTo(this);

        Observable.Timer(System.TimeSpan.Zero,System.TimeSpan.FromSeconds(changeColorDeltaTime))
            .TakeUntil(isFinish)
            .Subscribe(x =>
            {
                // 色が徐々に変わっていく処理
                try{
                    view.viewHack(100 - 2* (int)(object)x);
                }catch(NullReferenceException){
                    
                }
            }
        ).AddTo(this);
    }
}
