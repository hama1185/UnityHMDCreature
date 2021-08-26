using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hunger : MonoBehaviour
{
    public Text hungerText;
    bool state = false;

    public void OnButtonClick(){
        state = !state;
        if(state){
            this.hungerText.text = "Hunger ON";
        } else {
            this.hungerText.text = "Hunger OFF";
        }
    }
}
