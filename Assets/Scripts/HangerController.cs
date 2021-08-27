﻿using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
public class HangerController : MonoBehaviour
{

    int LOCAL_PORT = 22221; //myPort
    string host = "192.168.1.134"; //myIP
    UdpClient udp;
    Thread thread;

    int LOCAL_PORT2 = 10000; // espのPort
    string host2 = "192.168.11.31"; //espのIP

    //受信するESP32情報    
    string recvData;

    //送信するESP32情報
    private IPEndPoint remoteEP,remoteEP2; //UnityとESP
    private byte[] data;
    private byte[] received;

    //駆動状態・次の駆動を決定
    private int state=0;

    // Use this for initialization
    void Start()
    {
        //送受信用ポートを開きつつ送受信用スレッドを生成
        udp = new UdpClient(LOCAL_PORT);
        thread = null;
        thread = new Thread(new ThreadStart(ThreadMethod)); //スレッド生成
        thread.IsBackground = true;
        thread.Start(); //スレッド開始
        
        //スレッド内で毎回更新している理由がわからなかったのでこっちに
        remoteEP = new IPEndPoint(IPAddress.Parse(host), LOCAL_PORT); 
        remoteEP2 = new IPEndPoint(IPAddress.Parse(host2), LOCAL_PORT2); 
    }
    // Update is called once per frame
    void Update()
    {
    }

    //呼び出した回数に応じて異なる駆動＝ストーリーラインに準ずる
    public void act(){
        Debug.Log("[Hanger] act"+state);
        switch(state){
            case 0:
                pump_bal1_emit_bal2();
                break;
            case 1:
                emit();
                break;
            case 2:
                pump_bal2_emit_bal1();
                break;
            case 3:
                emit();
                break;
            default:
                stop_motor();
                break;
        }
        state++;
    }

    //任意の指定値での駆動
    public void drive(int tar1,int tar2,int pum1,int pum2,int val1,int val2){
        int[] arr={tar1,tar2,pum1,pum2,val1,val2};
        string str="";
        for(int i=0;i<6;i++){
            string tmp=arr[i].ToString();
            if(tmp.Length>=4){
                Debug.Log("[Hanger] Invalid input value!");
                return;
            }
            for(int j=3;j>tmp.Length;j--){
                str+="0";
            }
            str+=tmp;
            str+=",";
        }
        data=Encoding.UTF8.GetBytes(str);
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
    }

    //バルーンすべて膨らむ
    public void pump(){
        data = Encoding.UTF8.GetBytes("350,350,128,128,000,000,000,000,000,000,000,000,");
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
    }

    //バルーンすべて萎む
    public void emit(){
        data = Encoding.UTF8.GetBytes("999,999,256,256,256,256,000,000,000,000,000,000,");
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
    }

    //バルーン1のペア膨らむ　バルーン2のペア萎む
    public void pump_bal1_emit_bal2(){
        data = Encoding.UTF8.GetBytes("350,999,128,255,000,255,000,000,000,000,000,000,");
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
    }

    //バルーン2のペア膨らむ　バルーン1のペア萎む
    public void pump_bal2_emit_bal1(){
        data = Encoding.UTF8.GetBytes("999,350,255,128,255,000,000,000,000,000,000,000,");
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
    }

    //モータを停止する
    public void stop_motor(){
        data = Encoding.UTF8.GetBytes("000,000,000,000,255,255,000,000,000,000,000,000,");
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
    }

    //==========================================
    //
    //              UDP関連
    //
    //==========================================

    void OnApplicationQuit()
    {
        data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,000,000,000,000");
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);

        try
        {
            udp.Close();
            thread.Abort();
        }
        catch
        {
            Debug.Log("Thread Abording Failed");
        }
    }

    //スレッド1で実行される関数
    private void ThreadMethod()
    {
        //Thread.Sleep(10);
        Debug.Log("aaaaa");
        try
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.net.sockets.udpclient.beginreceive?view=net-5.0
            //データを受信したらrecv()関数を呼び出す。そのときudpをrecvに渡す(?)
            udp.BeginReceive(new AsyncCallback(recv), udp);
            Debug.Log("Begin Receiving UDP Packets");
        }
        catch
        {
            Debug.Log("UDP Connection Error");
        }

    }
    void send(IAsyncResult result)
    {
        udp.EndSend(result);
    }
    void recv(IAsyncResult result)
    {
        received = udp.EndReceive(result, ref remoteEP);
        //22221へのデータ受信したら再び待ち.常に受信できる状態？
        udp.BeginReceive(new AsyncCallback(recv), null);
        recvData = Encoding.UTF8.GetString(received);
        //Debug.Log(recvData);
    }
}