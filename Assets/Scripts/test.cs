using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class test : MonoBehaviour
{
    // Start is called before the first frame update
    Client client;
    public GameObject manager;
    void Start()
    {
        client = manager.GetComponent<Client>();
    }

    // Update is called once per frame
    public void onStopbuttonClick(){
        client.SendStop();
    }

    public void onSendbuttonClick(){
        client.SendStart(1);
    }

    public void onVolupButtonClick(){
        client.SendUp();
    }
    public void onVoldownButtonClick(){
        client.SendDown();
    }

    public void onPitchupButtonClick(){
        client.SendUp();
    }
    public void onPitchdownButtonClick(){
        client.SendDown();
    }
}
