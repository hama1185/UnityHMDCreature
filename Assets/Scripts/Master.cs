using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// UniRxの参照
using UniRx;

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

    // ハンガーが始まる時間
    [SerializeField]
    int hangerTime = 50;

    public bool audioSendFlag = false;

    public GameObject _Client,_Hanger;
    Client client;
    HangerController hanger;
    void Awake(){
        client = _Client.GetComponent<Client>();
        hanger = _Hanger.GetComponent<HangerController>();
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

                // 音のボリューム設定 
                Observable.Timer(System.TimeSpan.Zero,System.TimeSpan.FromSeconds(volUpDeltaTime))
                .TakeUntil(endFlag)
                .Subscribe(_ =>
                {
                    client.SendVolUp();
                }
                ).AddTo(this);

                // 音のピッチ設定
                Observable.Timer(System.TimeSpan.Zero,System.TimeSpan.FromSeconds(pitchUpDeltaTime))
                .TakeUntil(endFlag)
                .Subscribe(_ =>
                {
                    client.SendPitchUp();
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
                }
                ).AddTo(this);
            }
            ).AddTo(this);    
    }
}
