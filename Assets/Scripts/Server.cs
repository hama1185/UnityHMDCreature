using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityOSC;
using System.Text;
using System.Threading.Tasks;
using UniRx;
public class Server : MonoBehaviour {
    // Start is called before the first frame update
    #region Network Settings //----------追記
    public string serverName;
    public int inComingPort; //----------追記
    #endregion //----------追記
    public bool playFlag = false;
    // public bool volFlag = false;
    // public bool pitchFlag = false;
    public bool changeFlag = false;  
    public int type = 0;
    private Dictionary<string, ServerLog> servers;
    public float volume;
    public float pitch;
    float maxVol = 1.5f;
    float maxPitch = 1.5f;
    float minVol = 0.0f;
    float minPitch = 0.5f;
    void Awake() {
        serverName = "VP2";
        inComingPort = 8000;
        // Debug.Log("server IP : " + serverName + "   port : " + inComingPort);
        OSCHandler.Instance.serverInit(serverName,inComingPort); //init OSC　//----------変更
        servers = new Dictionary<string, ServerLog>();
        volume = 0.2f;
        pitch = 0.7f;
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
                        float data = (float)item.Value.packets[lastPacketIndex].Data[0];
                        Debug.Log(data);
                        switch(data){
                            case 1:
                            type = 1;
                            break;
                            case 2:
                            type = 2;
                            break;
                            case 3:
                            type = 3;
                            break;
                            default:
                            Debug.Log("なんすかそれ");
                            type = 0;
                            break;
                        }
                        playFlag = true;
                    }
                }
                else if(address.Contains("/Stop")){
                    if(playFlag){
                        Debug.Log("音を止める");
                        playFlag = false;
                        type = 0;
                    }
                }
                else if(address.Contains("/Space")){
                    changeFlag = false;
                    // float add = (float)item.Value.packets[lastPacketIndex].Data[0];
                    // // 0ならvol 1ならpitch
                    // if(add == 0){
                    //     volFlag = false;
                    // }
                    // else{
                    //     pitchFlag = false;
                    // }
                }
                // 上限の設定
                // else if(address.Contains("/Vol")){
                //     if(!volFlag){
                //         float value = (float)item.Value.packets[lastPacketIndex].Data[0];
                //         if(value > 0 && volume < maxVol){
                //             volume += 0.01f;
                //         }
                //         else if(value < 0 && pitch > minVol){
                //             volume -= 0.01f;
                //         }
                //         volFlag = true;
                //     }
                // }
                // else if(address.Contains("/Pitch")){
                //     if(!pitchFlag){
                //         float value = (float)item.Value.packets[lastPacketIndex].Data[0];
                //         if(value > 0 && pitch < maxPitch){
                //             pitch += 0.01f;
                //         }
                //         else if(value < 0 && pitch > minPitch){
                //             pitch -= 0.01f;
                //         }
                //         pitchFlag = true;
                //     }
                // }
                else if(address.Contains("/Change")){
                    if(!changeFlag){
                        float value = (float)item.Value.packets[lastPacketIndex].Data[0];
                        if(value > 0 && pitch < maxPitch){
                            pitch += 0.01f;
                            volume += 0.01f;
                        }
                        else if(value < 0 && pitch > minPitch){
                            pitch -= 0.01f;
                            volume -= 0.01f;
                        }
                        changeFlag = true;
                    }
                }
            }
        }
        // Debug.Log(Time.deltaTime);
    }
}