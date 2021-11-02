using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityOSC;
using System.Text;
using System.Threading.Tasks;
using UniRx;
public class ServoServer : MonoBehaviour {
    // Start is called before the first frame update
    #region Network Settings //----------追記
    public string serverName;
    public int inComingPort; //----------追記
    #endregion //----------追記
    public bool playFlag = false;
    
    private Dictionary<string, ServerLog> servers;
    
    void Awake() {
        serverName = "VP2";
        inComingPort = 8005;
        // Debug.Log("server IP : " + serverName + "   port : " + inComingPort);
        OSCHandler.Instance.serverInit(serverName,inComingPort); //init OSC　//----------変更
        servers = new Dictionary<string, ServerLog>();
        
    }
    // Update is called once per frame
    void Update() {
        OSCHandler.Instance.UpdateLogs();
        servers = OSCHandler.Instance.Servers;
    }
    void LateUpdate(){
        foreach( KeyValuePair<string, ServerLog> item in servers ){
            if(item.Value.log.Count > 0){
                int lastPacketIndex = item.Value.packets.Count - 1;
                var address = item.Value.packets[lastPacketIndex].Address.ToString();
                if(address.Contains("/Start")){
                    if(!playFlag){
                        playFlag = true;
                    }
                }
            }
        }
        // Debug.Log(Time.deltaTime);
    }
}