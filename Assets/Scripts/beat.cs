using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class beat : MonoBehaviour
{
    public Text beatText;
    bool state = false;

    public void OnButtonClick(){
        state = !state;
        if(state){
            this.beatText.text = "Beat ON";
        } else {
            this.beatText.text = "Beat OFF";
        }
    }
}
