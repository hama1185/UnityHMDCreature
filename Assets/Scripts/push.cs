using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class push : MonoBehaviour
{
    private Rigidbody rb1;
    private Rigidbody rb2;
    private Rigidbody rb3;
    private int current_no = 1;

    public GameObject book;
    public GameObject plant;
    public GameObject woman;

    // Start is called before the first frame update
    void Start()
    {
        rb1 = book.GetComponent<Rigidbody>();    
        rb2 = plant.GetComponent<Rigidbody>();    

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pushObject(){
        if(current_no == 1){
            woman.GetComponent<Move>().OpeningDoor();
            current_no++;
        }
        else if(current_no == 2){
            Vector3 f = new Vector3(0.5f, 0.0f, 0.0f);
            rb1.AddForce(f, ForceMode.Impulse);
            current_no++;
        } else if (current_no == 3){
            Vector3 f = new Vector3(-0.5f, 0.0f, 0.0f);
            rb2.AddForce(f, ForceMode.Impulse);
            current_no++;
        } 
    }
}
