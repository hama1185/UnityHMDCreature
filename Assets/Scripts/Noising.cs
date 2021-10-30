using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noising : MonoBehaviour
{
    int state = 0;
    public GameObject door;
    public GameObject plant;
    public GameObject book;
    AudioSource a1;
    AudioSource a2;
    AudioSource a3;
    public AudioClip audioClip;
    // Start is called before the first frame update
    void Start()
    {
        a1 = door.GetComponent<AudioSource>();
        a2 = plant.GetComponent<AudioSource>();
        a3 = book.GetComponent<AudioSource>();
        a1.clip = audioClip;
        a2.clip = audioClip;
        a3.clip = audioClip;
        a1.volume = 0.0f;
        a2.volume = 0.0f;
        a3.volume = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeNoise(){
        if(state == 0){
            a1.time = 298.0f;
            a1.Play();
            StartCoroutine("VolumeUp", a1);
            Debug.Log("1st noise");
            state++;
        } else if(state == 1){
            a2.time = 5.0f;
            a2.Play();
            StartCoroutine("VolumeUp", a2);
            Debug.Log("2nd noise");
            state++;
        } else if(state == 2){
            a3.time = 5.0f;
            a3.Play();
            StartCoroutine("VolumeUp", a3);
            Debug.Log("3rd noise");
            state++;
        } else {

        }
    }

    

    IEnumerator VolumeUp(AudioSource aud)
    {
        while(aud.volume < 0.15f)
        {
            aud.volume +=0.005f;
            yield return new WaitForSeconds(0.2f);
        }
        aud.Stop();
    }
}
