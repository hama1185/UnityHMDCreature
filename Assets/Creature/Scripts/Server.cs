using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityOSC;
using System.Text;
using System.Threading.Tasks;

public class Server : MonoBehaviour {
    // Start is called before the first frame update
    #region Network Settings //----------追記
	public string serverName;
	public int inComingPort; //----------追記
	#endregion //----------追記

	private Dictionary<string, ServerLog> servers;
    
    void Awake() {
        serverName = "Audio";
        inComingPort = 8000;
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
                Debug.Log("get");

                if(address.Contains("/VP2")){
                    float onFlag = (float)item.Value.packets[lastPacketIndex].Data[0];
                    if(onFlag != 0){
                        int type = (int)item.Value.packets[lastPacketIndex].Data[1];
                        switch(type){
                            case 1:
                            Debug.Log("1を再生");
                            break;
                            case 2:
                            Debug.Log("2を再生");
                            break;
                            case 3:
                            Debug.Log("3を再生");
                            break;
                            default:
                            Debug.Log("なんすかそれ");
                            break;
                        }
                    }
                    else{
                        Debug.Log("音を止める");
                        // 音止める
                    }
                }
			}
		}
        // Debug.Log(Time.deltaTime);
    }
}