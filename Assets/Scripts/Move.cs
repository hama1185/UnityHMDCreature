using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public GameObject firstDoor;
    public GameObject secondDoor;
    bool isWalking = false;
    Animator animator;

    [SerializeField] private AudioSource a;//AudioSource型の変数aを宣言 使用するAudioSourceコンポーネントをアタッチ必要

    [SerializeField] private AudioClip b1;//AudioClip型の変数b1を宣言 使用するAudioClipをアタッチ必要
    [SerializeField] private AudioClip b2;//AudioClip型の変数b2を宣言 使用するAudioClipをアタッチ必要 
    
    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponent<Animator>();
        //OpeningDoor();
    }

    // Update is called once per frame
    void Update()
    {
        if(isWalking){
            this.transform.position += new Vector3(0.0f, 0.0f, -0.01f);
            if(this.transform.position.z < -1.8f){
                StartCoroutine("closeDoor");
                isWalking = false;
                animator.SetTrigger("CloseDoor");

            }
        }
    }

    //これを呼び出せば人が入ってくるアニメーション開始
    public void OpeningDoor(){
        StartCoroutine("openDoor");
    }

    IEnumerator openDoor(){
        var currentRotation = firstDoor.transform.localRotation; // localEulerAnglesの代わりにlocalRotationを取得
        var currentRotation2 = secondDoor.transform.localRotation; // localEulerAnglesの代わりにlocalRotationを取得
        var newRotation = currentRotation * Quaternion.AngleAxis(90, Vector3.up); // currentRotationを(1, 0, 0)軸周り90°回転したものをnewRotationとする
        var newRotation2 = currentRotation * Quaternion.AngleAxis(0, Vector3.up); // currentRotationを(1, 0, 0)軸周り90°回転したものをnewRotationとする

        //Debug.Log(currentRotation.eulerAngles);
        //Debug.Log(newRotation.eulerAngles);

        //入口のドアが開く
        a.PlayOneShot(b1);
        for (float t=0;t<0.5f;t+=0.025f)
        {
            Quaternion rotation = Quaternion.Slerp(currentRotation, newRotation, t * 2); // 中間の回転を求めるのにSlerpを使いましたが、より高速なLerpを使ってもほとんど違和感はないかと思います

            //rotatingAxisは回転させるオブジェクト
            firstDoor.transform.localRotation = rotation;

            yield return new WaitForSeconds(0.05f);
        }

        animator.SetTrigger("OpenDoor");
        isWalking = true;

        yield return new WaitForSeconds(1.5f);
        currentRotation = firstDoor.transform.localRotation;
        for (float t=0;t<0.5f;t+=0.025f)
        {
            Quaternion rotation = Quaternion.Slerp(currentRotation, newRotation2, t * 2); // 中間の回転を求めるのにSlerpを使いましたが、より高速なLerpを使ってもほとんど違和感はないかと思います

            //rotatingAxisは回転させるオブジェクト
            firstDoor.transform.localRotation = rotation;

            yield return new WaitForSeconds(0.05f);
        }
        //入り口のドアが閉まる
        a.PlayOneShot(b2);

        //出口のドアが開く
        a.PlayOneShot(b1);
        for (float t=0;t<0.5f;t+=0.025f)
        {
            Quaternion rotation = Quaternion.Slerp(currentRotation2, newRotation, t * 2); // 中間の回転を求めるのにSlerpを使いましたが、より高速なLerpを使ってもほとんど違和感はないかと思います

            //rotatingAxisは回転させるオブジェクト
            secondDoor.transform.localRotation = rotation;

            yield return new WaitForSeconds(0.05f);
        }

        
    }

    IEnumerator closeDoor(){
        var Rotation2 = secondDoor.transform.localRotation;
        var newRotation2 = Rotation2 * Quaternion.AngleAxis(-90, Vector3.up); // currentRotationを(1, 0, 0)軸周り90°回転したものをnewRotationとする
        Debug.Log("closing");
        for (float t=0;t<0.5f;t+=0.025f)
        {
            Quaternion rotation = Quaternion.Slerp(Rotation2, newRotation2, t * 2); // 中間の回転を求めるのにSlerpを使いましたが、より高速なLerpを使ってもほとんど違和感はないかと思います

            //rotatingAxisは回転させるオブジェクト
            secondDoor.transform.localRotation = rotation;

            yield return new WaitForSeconds(0.05f);
        }
        //出口のドアが閉まる
        a.PlayOneShot(b2);
    }
}
