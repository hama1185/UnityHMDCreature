using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// UniRxの参照
using UniRx;

public class Master : MonoBehaviour
{
    // 起動時処理
    void Start()
    {
        Debug.Log("Time : " + Time.time + ", Main ThreadID:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

        // 10秒ごとに実行（Timerかつ第一引数に 0 指定の場合、Subscribe後に即座に1回目の処理が実行される）
        Observable.Timer(System.TimeSpan.Zero, System.TimeSpan.FromSeconds(10))
            .Subscribe(x =>
            {
                Debug.Log("Timer Time : " + Time.time + ", No : " + x.ToString() +
                    ", ThreadID : " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            ).AddTo(this);

        // 10秒ごとに実行（Intervalの場合、Subscribe後に待機時間を待機してから1回目の処理が実行される）
        Observable.Interval(System.TimeSpan.FromSeconds(10))
            .Subscribe(x =>
            {
                Debug.Log("Interval Time : " + Time.time + ", No : " + x.ToString() +
                    ", ThreadID : " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            ).AddTo(this);
    }
}
