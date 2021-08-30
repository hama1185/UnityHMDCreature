using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Timer : MonoBehaviour
{
    public bool firstFlag = false;
    public bool testFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(firstFlag){
            // if(!testFlag){
                Debug.Log("a");
                Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
                .Subscribe(_ =>
                {
                    // フラグを起動
                    Debug.Log("b");
                    // headAngle.hangerLeftFlag = false;ここで呼び出すのはいけない
                }
                ).AddTo(this);
                // testFlag = true;
            // }
            firstFlag = false;
        }
    }
}
