using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using System;
using System.Threading.Tasks;

public class Client : MonoBehaviour
{
    #region Network Settings //----------追記
    public string ip;
    public int port;
	#endregion //----------追記
    string address;
    string id = "VP2";

    void Awake() {
        IpGetter ipGetter = new IpGetter();
        string myIP = ipGetter.GetIp();
        //ここにServer(PC)のIPアドレス
        ip = "192.168.11.7";
        port = 8000;
        Debug.Log("client IP : " + ip + "   port : " + port);

        OSCHandler.Instance.clientInit(id, ip,port);//ipには接続先のipアドレスの文字列を入れる。
    }

    void Start()
    {
        address = "/VP2";
    }

    // Update is called once per frame
    public void SendStart(float boise){
        address = "/Start";
        List<float> sendAudioList = new List<float>();
        sendAudioList.Add(boise);
        OSCHandler.Instance.SendMessageToClient(id, address, sendAudioList);
    }

    public void SendStop(){
        address = "/Stop";
        List<float> sendAudioList = new List<float>();
        sendAudioList.Add(0);
        OSCHandler.Instance.SendMessageToClient(id, address, sendAudioList);
    }

    // sendSpaceで重複しないようにする
    public void SendUp(){
        SendSpace(0);

        address = "/Change";
        List<float> sendAudioList = new List<float>();
        sendAudioList.Add(1);
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後に処理
            OSCHandler.Instance.SendMessageToClient(id, address, sendAudioList);
        }));
    }

    public void SendDown(){
        SendSpace(0);

        address = "/Change";
        List<float> sendAudioList = new List<float>();
        sendAudioList.Add(-1);
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後に処理
            OSCHandler.Instance.SendMessageToClient(id, address, sendAudioList);
        }));
    }

    public void SendSpace(int add){
        address = "/Space";
        List<float> sendAudioList = new List<float>();
        sendAudioList.Add(add);
        OSCHandler.Instance.SendMessageToClient(id, address, sendAudioList);
    }

    private IEnumerator DelayCoroutine(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }
}
