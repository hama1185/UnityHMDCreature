using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Timer : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(3.0f))
        .TakeWhile(count => count < 5)
        .Subscribe(_ =>
        {
            // フラグを起動
            Debug.Log("b");
        }
        ).AddTo(this);
        
        Observable.Interval(System.TimeSpan.FromSeconds(3.0f))
        .Take(5)
        .Subscribe(_ =>
        {
            // フラグを起動
            Debug.Log("c");
        }
        ).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
