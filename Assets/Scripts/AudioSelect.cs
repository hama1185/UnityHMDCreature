using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSelect : MonoBehaviour
{
    // このスクリプトと同じ場所にaudiosourceをアタッチする
    public GameObject Server;
    Server server;

    AudioSource[] sounds;

    [SerializeField] private AudioMixer Main;

    float beforetype = 0;
    bool changeFlag = false;
    
    void Start()
    {
        sounds = GetComponents<AudioSource>();
        server = Server.GetComponent<Server>();

    }

    void Update()
    {
        
        switch(server.type){
            case 0:
            if(beforetype != 0){
                sounds[(int)beforetype - 1].Stop();
                beforetype = 0;
            }
            break;

            case 1:
            if(beforetype != 1){
                sounds[0].Play();
                beforetype = 1;
                Debug.Log("1再生");
            }
            break;

            case 2:
            if(beforetype != 2){
                sounds[1].Play();
                beforetype = 2;
                Debug.Log("2再生");
            }
            break;

            default:
            
            break;
        }

        Main.SetFloat("vol",ConvertVolume2dB(server.volume));
        Main.SetFloat("pitch", server.pitch);
    }

    // デシベルに直す
    float ConvertVolume2dB(float volume) => Mathf.Clamp(20f * Mathf.Log10(Mathf.Clamp(volume, 0f, 2f)), -80f, 20f);
}
