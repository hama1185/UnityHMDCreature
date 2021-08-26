using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using System.Text;
using System.Threading.Tasks;

public class Client : MonoBehaviour
{
    #region Network Settings //----------追記
    public string ip;
    public int port;
	#endregion //----------追記
    string address;
    string id = "Audio";

    void Awake() {
        IpGetter ipGetter = new IpGetter();
        string myIP = ipGetter.GetIp();

        ip = "192.168.1.21";
        port = 8000;
        Debug.Log("client IP : " + ip + "   port : " + port);

        OSCHandler.Instance.clientInit(id, ip,port);//ipには接続先のipアドレスの文字列を入れる。
    }

    void Start()
    {
        address = "/VP2";
    }

    // Update is called once per frame
    void SendStart(int boise){
        List<float> sendAudioList = new List<float>();
        sendAudioList.Add(1);
        sendAudioList.Add(boise);
        OSCHandler.Instance.SendMessageToClient(id, address, sendAudioList);
    }

    void SendStop(){
        List<float> sendAudioList = new List<float>();
        sendAudioList.Add(0);
        OSCHandler.Instance.SendMessageToClient(id, address, sendAudioList);
    }
}
