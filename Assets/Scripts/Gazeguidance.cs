using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gazeguidance : MonoBehaviour
{
    public GameObject book;
    public GameObject plant;
    public GameObject door;
    public GameObject prefab;
    float Num;
    Quaternion RandomQ;
    private int current_no = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateGuide(){
        if(current_no == 1){
            for(int i = 0; i < 10; i++){
                //落下物に対して向かっていく光のようなものを生成
                Vector3 pos = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-2.0f, 2.0f), Random.Range(-1.0f, 3.0f));
                Num = Random.Range(-180.0f, 180.0f);
                RandomQ = Quaternion.Euler(0,0,Num);
                GameObject ball = Instantiate(prefab, pos, RandomQ);
                MoveSphere ms = ball.GetComponent<MoveSphere>();
                ms.setTarget(door);
            }
            current_no++;
        } else if (current_no == 2){
            for(int i = 0; i < 15; i++){
                //落下物に対して向かっていく光のようなものを生成
                Vector3 pos = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-2.0f, 2.0f), Random.Range(-1.0f, 3.0f));
                Num = Random.Range(-180.0f, 180.0f);
                RandomQ = Quaternion.Euler(0,0,Num);
                GameObject ball = Instantiate(prefab, pos, RandomQ);
                MoveSphere ms = ball.GetComponent<MoveSphere>();
                ms.setTarget(book);
            }
            current_no++;
        } else if (current_no == 3){
            for(int i = 0; i < 15; i++){
                //落下物に対して向かっていく光のようなものを生成
                Vector3 pos = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-2.0f, 2.0f), Random.Range(-1.0f, 3.0f));
                Num = Random.Range(-180.0f, 180.0f);
                RandomQ = Quaternion.Euler(0,0,Num);
                GameObject ball = Instantiate(prefab, pos, RandomQ);
                MoveSphere ms = ball.GetComponent<MoveSphere>();
                ms.setTarget(plant);
            }
            current_no++;
        }
    }
}
