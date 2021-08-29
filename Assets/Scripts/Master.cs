using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// UniRxの参照
using UniRx;
using System;

// このスクリプトでこのプロジェクトを管理する

public class Master : MonoBehaviour
{
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

    public bool audioSendFlag = false;

    public GameObject _Client, _Hanger, _ViewHack, _Head;
    Client client;
    HangerController hanger;
    ViewHacking view;
    headAngleControl headAngle;

    bool testFlag = false;
    void Awake(){
        client = _Client.GetComponent<Client>();
        hanger = _Hanger.GetComponent<HangerController>();
        try{
            view = _ViewHack.GetComponent<ViewHacking>();
        }catch(UnassignedReferenceException e){
            Debug.Log(e);
        }
        headAngle = _Head.GetComponent<headAngleControl>();
    }

    // 起動時処理
    void Start()
    {
        // 序盤の設定

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

                    // headangle取得するスクリプトのflagを変える
                    // イベントを起動させる
                    // 生物を倒すことのコールバックが来たら終盤の設定を起動する
                }
                ).AddTo(this);
            }
            ).AddTo(this);
    }

    void Update(){
        
        if(headAngle.firstHangerFlag){
            if(!testFlag){
                hanger.act();
                Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
                .Subscribe(_ =>
                {
                    // フラグを起動
                    headAngle.hangerLeftFlag = false;
                    headAngle.hangerRightFlag = true;
                    // ハンガー反射デバイス起動
                    hanger.act();
                }
                ).AddTo(this);
                testFlag = true;
            }
            headAngle.firstHangerFlag = false;
            // 後で3にする
        }

        if(headAngle.secondHangerFlag){
            hanger.act();
            headAngle.hangerRightFlag = false;
            headAngle.secondHangerFlag = false;
        }
        
    }
}
