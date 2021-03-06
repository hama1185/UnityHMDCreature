using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UniRx;
public class AudioSelect : MonoBehaviour
{
    // このスクリプトと同じ場所にaudiosourceをアタッチする
    public GameObject Server;
    Server server;
    AudioSource[] sounds;
    [SerializeField] private AudioMixer Main;
    float beforetype = 0;
    bool changeFlag = false;
    float basevol;
    float basepitch;
    void Start()
    {
        sounds = GetComponents<AudioSource>();
        server = Server.GetComponent<Server>();
        // awakeで値を代入してるので大丈夫
        basevol = server.volume;
        basepitch = server.pitch;
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
                UpAudio();
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
        // 止めるときはまた別の処理
        // if(basevol != server.volume && basepitch != server.pitch && server.type == 0){
        //     server.type = 1;
        // }
        Main.SetFloat("vol",ConvertVolume2dB(server.volume));
        Main.SetFloat("pitch", server.pitch);
    }
    void UpAudio(){
        Observable.Interval(System.TimeSpan.FromSeconds(0.5))
            .Take(80)
            .Subscribe(_ =>
            {
                server.pitch += 0.01f;
                server.volume += 0.01f;
            }
        ).AddTo(this);
    }
    // デシベルに直す
    float ConvertVolume2dB(float volume) => Mathf.Clamp(20f * Mathf.Log10(Mathf.Clamp(volume, 0f, 2f)), -80f, 20f);
}