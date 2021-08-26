using System;
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
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public static class TimeUtil
{

    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    // DateTimeからUnixTimeへ変換
    public static long GetUnixTime(DateTime dateTime)
    {
        return (long)(dateTime - UnixEpoch).TotalSeconds;
    }

    // UnixTimeからDateTimeへ変換
    public static DateTime GetDateTime(long unixTime)
    {
        return UnixEpoch.AddSeconds(unixTime);
    }
}

public class IVRC : MonoBehaviour
{

    //int LOCAL_PORT = 22222; 
    //int LOCAL_PORT = 22221; //
    int LOCAL_PORT = 22221; //myPort
    //string host = "192.168.1.177";
    string host = "192.168.1.134"; //myIP
    UdpClient udp;
    Thread thread;

    //int LOCAL_PORT2 = 33333; 
    int LOCAL_PORT2 = 10000; // espのPort
    //string host2 = "192.168.11.32";
    string host2 = "192.168.11.31"; //espのIP
    //UdpClient udp2;
    //Thread thread2;

    //受信するESP32情報
    int[] SensorValue = { 0, 0, 0, 0 };
    int[] MotorPWM = { 0, 0, 0, 0 };
    int[] ValvePWM = { 0, 0, 0, 0 };
    float Yaw = 0f;
    float Pitch = 0f;
    float Roll = 0f;
    string recvData;

    //Air
    int inAbsGoalValue;

    //送信するESP32情報
    int SendPWM = 0;
    int SendGoalSensor = 0;
    int CountSendPWM = 5;
    int CountSendGoalSensor = 5;
    string strSendPWM;
    string strSendGoalSensor;

    //時間変数
    private int starttime;
    private int now;
    public int currenttime;

    bool flagMeasurement = false;

    private GameObject messageText;


    private bool blInvoke = false;
    private bool blTrial = false;
    private bool blFinish = false;

    private IPEndPoint remoteEP,remoteEP2; //UnityとESP
    private byte[] data;
    private byte[] received;

    private List<string> recvDatas = new List<string>();

    private bool Writing = true;

    public AudioClip audioClip1;
    private AudioSource audioSource;

    //Vection変数
    private Rigidbody User;
    private float WorldAngleX, WorldAngleY, WorldAngleZ;


    //Flag of Vection, Hanger, GVS
    private Boolean blDriveWave = false;

    [SerializeField] private Text txt;

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
        
        MainThreadExecutor.Initialize();

        //udp2 = new UdpClient(LOCAL_PORT2);
        //thread2 = null;
        //thread2 = new Thread(new ThreadStart(ThreadMethod2));
        //thread2.IsBackground = true;
        //thread2.Start(); //スレッド開始

        //flagMeasurement = true;
        //blDriveWave = true;
        StartTime();

    }



    private void StartTime()
    {
        //時間計測開始
        #region 時間計測開始
        starttime = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        #endregion
    }

    bool oneshot_R = false;
    bool oneshot_L = false;
    bool blDrive = false; //バルーン動作させるか？

    private string textstr="";
    private void updateText(){
        txt.text=textstr;
    }

bool tmp=true;
    // Update is called once per frame
    void Update()
    {
        Task.Run(() => {
            MainThreadExecutor.Instance.Post(() => {
                updateText();
            });
        });
        now = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

        currenttime = now - starttime;

        //Debug.Log(currenttime);

        if (true==false)
        {
            if (true)
            {
                tmp=false;
                if (tmp)
                {
                    //文字列からバイト型配列に変換
                    //data = Encoding.UTF8.GetBytes("642,642,000,000,255,000,000,000,000,000,000,000");
                    data = Encoding.UTF8.GetBytes("250,250,128,128,064,064,000,000,000,000,000,000");
                    //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
                    udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
                    //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
                    //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
                    oneshot_R = true;
                    oneshot_L = false;
                    tmp=false;
                }
            }
            else // > 2000
            {
                if (oneshot_L == false)
                {
                    data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
                    //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
                    udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
                    //data = Encoding.UTF8.GetBytes("642,642,000,000,255,000,000,000,000,000,000,000");
                    //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
                    oneshot_R = false;
                    oneshot_L = true;
                }

            }
        }

        if (blDriveWave == true)
        {
            Hanger();
        }
        else
        {
            //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
            //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        }


        if (Input.GetKeyDown("joystick button 5"))
        {
            Debug.Log("R1");
            data = Encoding.UTF8.GetBytes("642,642,000,000,255,000,000,000,000,000,000,000");
            //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
            udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
            //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
            //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
           

        }
        else if (Input.GetKeyDown("joystick button 3"))             //Release
        {

            Debug.Log("△");
            data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
            //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
            udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
            //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);

        }
        else if (Input.GetKeyDown("joystick button 13"))            //Neutral
        {

            Debug.Log("TouchPad");
            data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,000,000,000,000");
            udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
            //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
            //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
            blDrive = false;

        }
        else if (Input.GetKeyDown("joystick button 9"))
        {
            Debug.Log("option");
            //            UnityEditor.EditorApplication.isPlaying = false;

        }
        else if (Input.GetKeyDown("joystick button 0"))
        {
            Debug.Log("□");
            blDrive = true;
            //flagMeasurement = true;
            //blDriveWave = true;
            //StartTime();

        }
        else if (Input.GetKeyDown("joystick button 4"))
        {
            Debug.Log("L1");
            data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
            //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
            udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
            //data = Encoding.UTF8.GetBytes("642,642,000,000,255,000,000,000,000,000,000,000");
            //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);

        }
        else
        {

        }
       

    }

    public void pump(){
        data = Encoding.UTF8.GetBytes("350,350,128,128,000,000,000,000,000,000,000,000,");
        //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
        //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
        //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
        oneshot_R = true;
        oneshot_L = false;
    }

    public void emit(){
        data = Encoding.UTF8.GetBytes("999,999,256,256,256,256,000,000,000,000,000,000,");
        //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
        //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
        //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
        oneshot_R = true;
        oneshot_L = false;
    }

    public void pump_bal1_emit_bal2(){
        data = Encoding.UTF8.GetBytes("350,999,128,255,000,255,000,000,000,000,000,000,");
        //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
        //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
        //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
        oneshot_R = true;
        oneshot_L = false;
    }

    public void pump_bal2_emit_bal1(){
        data = Encoding.UTF8.GetBytes("999,350,255,128,255,000,000,000,000,000,000,000,");
        //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
        //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
        //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
        oneshot_R = true;
        oneshot_L = false;
    }

    public void stop_motor(){
        data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,000,000,000,000,");
        //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
        //data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,255,255,255,255");
        //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send2), udp2);
        oneshot_R = true;
        oneshot_L = false;
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
        //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        //udp2.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp2);


        try
        {
            udp.Close();
            thread.Abort();
        }
        catch
        {
            //Debug.Log("Thread Abording Failed");
        }
        try
        {
            //udp2.Close();
            //thread2.Abort();
        }
        catch
        {
            //Debug.Log("Thread Abording Failed");
        }
        //Debug.Log(thread);
        //Debug.Log(thread.IsAlive);
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
            //host("192.168.11.32", 22221からorへのデータ?)
            //remoteEP = new IPEndPoint(IPAddress.Parse(host), LOCAL_PORT); 
        }
        catch
        {
            Debug.Log("UDP Connection Error");
        }

    }
    //スレッド2で実行される関数
    /*
    private void ThreadMethod2()
    {
        //Thread.Sleep(10);

        try
        {
            udp2.BeginReceive(new AsyncCallback(recv), udp2);
            Debug.Log("Begin Receiving UDP Packets");
            //host("192.168.11.33", 10000からorへのデータ?)
            remoteEP2 = new IPEndPoint(IPAddress.Parse(host2), LOCAL_PORT2);
        }
        catch
        {
            Debug.Log("UDP Connection Error");
        }

    }
    */
    void send(IAsyncResult result)
    {
        udp.EndSend(result);
    }
    /*
    void send2(IAsyncResult result)
    {
        udp2.EndSend(result);
    }
    */
    void recv(IAsyncResult result)
    {
        //Debug.Log("receive");
        //resultはudpクライアント？
        received = udp.EndReceive(result, ref remoteEP);
        //22221へのデータ受信したら再び待ち.常に受信できる状態？
        udp.BeginReceive(new AsyncCallback(recv), null);
        recvData = Encoding.UTF8.GetString(received);
        Debug.Log(recvData);

        string[] arr = recvData.Split(',');

        SensorValue[0] = int.Parse(arr[0]);
        MotorPWM[0] = int.Parse(arr[1]);
        ValvePWM[0] = int.Parse(arr[2]);
        ValvePWM[1] = int.Parse(arr[3]);
        ValvePWM[2] = int.Parse(arr[4]);
        ValvePWM[3] = int.Parse(arr[5]);
        textstr="[0]\nSensor:"+arr[0]
                +"\nCurrent:"+arr[2]
                +"\nPump:"+arr[4]
                +"\nValve:"+arr[6]
                +"\n\n[1]\nSensor:"+arr[1]
                +"\nCurrent:"+arr[3]
                +"\nPump:"+arr[5]
                +"\nValve:"+arr[7];

        if (flagMeasurement == true)
        {
            now = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

            currenttime = now - starttime;
            //Debug.Log(currenttime);

            if (currenttime > 30000)
            {
                currenttime = 0;
                flagMeasurement = false;
                blDriveWave = false;
                data = Encoding.UTF8.GetBytes("000,000,000,000,000,000,000,000,000,000,000,000");
                //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
                udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);

                
                recvDatas.Clear();
            }
        }
    }

    float time = 0f;
    int inGoalValue = 0;
    int inPreGoalValue = 0;
    int inDerivation;
    float fGoalValue = 600f;
    float fPreGoalValue = 0f;
    float fDerivation = 0f;
    [SerializeField]
    int TargetValue = 535;
    [SerializeField]
    int TargetPumpPWM = 255;
    [SerializeField]
    int TargetValvePWM = 255;
    [SerializeField]
    float fPeriodHanger = 10f;
    [SerializeField]
    int iThetaHangerDenominator = 2;
    [SerializeField]
    int iThetaHangerNumerator = 0;
    double fThetaHanger = 0;
    [SerializeField]
    Boolean[] blHangerSelectedAxis = { false, false, false };
    float[] fHangerSelectedAxis = { 0f, 0f, 0f };
    String strGoalValue = "";
    String strValve = "";
    String strTargetValvePWM;
    String strTargetPumpPWM;

    void Hanger()
    {
        time = time + Time.deltaTime;
        //InspectorからThetaを生成
        fThetaHanger = System.Math.PI * ((double)iThetaHangerNumerator / iThetaHangerDenominator);

        //Inspectorから軸を選択
        for (int i = 0; i < blHangerSelectedAxis.Length; i++)
        {
            if (blHangerSelectedAxis[i] == true)
            {
                fHangerSelectedAxis[i] = 1f;
            }
            else
            {
                fHangerSelectedAxis[i] = 0f;
            }
        }

        //Pumpの出力値について，ESP32への送信のためにフォーマットを揃える
        if (TargetPumpPWM < 10)
        {
            strTargetPumpPWM = "00" + TargetPumpPWM.ToString();
        }
        else if (TargetPumpPWM < 100)
        {
            strTargetPumpPWM = "0" + TargetPumpPWM.ToString();
        }
        else
        {
            strTargetPumpPWM = TargetPumpPWM.ToString();
        }

        //Valveの出力値について，ESP32への送信のためにフォーマットを揃える
        if (TargetValvePWM < 10)
        {
            strTargetValvePWM = "00" + TargetValvePWM.ToString();
        }
        else if (TargetValvePWM < 100)
        {
            strTargetValvePWM = "0" + TargetValvePWM.ToString();
        }
        else
        {
            strTargetValvePWM = TargetValvePWM.ToString();
        }


        //傾きを生成するために，過去値を格納
        //inPreGoalValue = inGoalValue;
        fPreGoalValue = fGoalValue;
        //Waveをy軸方向に振幅分だけオフセットし，正領域でのみ変動させる
        fGoalValue = (float)(TargetValue * System.Math.Sin(2 * System.Math.PI * (time / fPeriodHanger) + fThetaHanger)) + TargetValue;

        //微分して，増加区間or減少区間を判別
        //Derivation = next - pre
        //inDerivation = inGoalValue - inPreGoalValue;
        fDerivation = fGoalValue - fPreGoalValue;
        //Debug.Log(inGoalValue + ", " + inPreGoalValue + ", " + inDerivation);

        if (fDerivation > 0)   //増加区間
        {
            //Pitch Down
            if(blHangerSelectedAxis[0] == true)
            {
                strValve = strTargetValvePWM + ",000," + strTargetValvePWM + ",000";
            }
            //Yaw Right
            else if(blHangerSelectedAxis[1] == true)
            {
                strValve = "000," + strTargetValvePWM + "," + strTargetValvePWM + ",000";
            }
            //Roll Left
            else if(blHangerSelectedAxis[2] == true)
            {
                strValve = strTargetValvePWM + "," + strTargetValvePWM + ",000,000";
            }
            else { }         
        }
        else if(fDerivation < 0)   //減少区間
        {
            //Pitch Up
            if (blHangerSelectedAxis[0] == true)
            {
                strValve = "000," + TargetValvePWM + ",000," + TargetValvePWM;
            }
            //Yaw Left
            else if (blHangerSelectedAxis[1] == true)
            {
                strValve = TargetValvePWM + ",000,000," + TargetValvePWM;
            }
            //Roll Right
            else if (blHangerSelectedAxis[2] == true)
            {
                strValve = "000,000," + TargetValvePWM + "," + TargetValvePWM;
            }
            else { }       
        }
        else  //Hold区間
        {
            //直前の動作をHoldする
        }

        //目標値を絶対値に変換　<- 絶対値に変換する必要は無いが，前実装の名残．
        //ESP32との送信フォーマットを揃える
        //y軸方向にTargetValue分だけオフセットしているので，inGoalValue - 600して送信データとしての振幅をあわせる
        inGoalValue = (int)fGoalValue;
        inAbsGoalValue = System.Math.Abs(inGoalValue)/2;    //ex. 1200 -> 600


        if(fDerivation < 0)    //減少区間
        {
            inAbsGoalValue = TargetValue - inAbsGoalValue;  //ex. 600-600 = 0, 600 - 0 = 600;
        }
        else if(fDerivation > 0)   //増加区間
        {

        }
        else
        {

        }

        //Debug.Log((int)(TargetValue * System.Math.Sin(2 * System.Math.PI * (time / fPeriodHanger) + fThetaHanger)) + ", " + TargetValue
        //    + ", " + inAbsGoalValue + ", " + inDerivation);
        //Debug.Log(fGoalValue + ", " + inAbsGoalValue + ", " + fDerivation);


        if (inAbsGoalValue < 10)
        {
            strGoalValue = "00" + inAbsGoalValue.ToString();
        }
        else if (inAbsGoalValue < 100)
        {
            strGoalValue = "0" + inAbsGoalValue.ToString();
        }
        else
        {
            strGoalValue = inAbsGoalValue.ToString();
        }

        string sd = strGoalValue + ",000,000," + strGoalValue + ",255,000,000,000," + strValve;

        //ここのdataを変更して送信する
        data = Encoding.UTF8.GetBytes(sd);
        Debug.Log("send data is: " + data);
        //udp.BeginSend(data, data.Length, remoteEP, new AsyncCallback(send), udp);
        udp.BeginSend(data, data.Length, remoteEP2, new AsyncCallback(send), udp);
    }

    
}

public class MainThreadExecutor : MonoBehaviour
{
    private Queue<Action> actionQueue = new Queue<Action>();

    private static readonly object syncObject = new object();
    private static MainThreadExecutor instance;

    public static MainThreadExecutor Instance
    {
        get
        {
            lock (syncObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<MainThreadExecutor>();
                }

                if (instance == null)
                {
                    instance = new GameObject(typeof(MainThreadExecutor).ToString()).AddComponent<MainThreadExecutor>();
                }
            }

            return instance;
        }
    }

    public static void Initialize()
    {
        var executor = MainThreadExecutor.Instance;
        Debug.Log("initialized :" + executor ?? executor.name);
    }

    public void Post(Action action)
    {
        lock (syncObject)
        {
            actionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        while (actionQueue.Any())
        {
            Action action = null;
            lock (syncObject)
            {
                action = actionQueue.Dequeue();
            }

            if (action != null)
            {
                action.Invoke();
            }
        }
    }
}
