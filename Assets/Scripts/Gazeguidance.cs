using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gazeguidance : MonoBehaviour
{
    public GameObject book;
    public GameObject plant;
    public GameObject tissue;
    public GameObject prefab;

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
                GameObject ball = Instantiate(prefab, pos, Quaternion.identity);
                MoveSphere ms = ball.GetComponent<MoveSphere>();
                ms.setTarget(book);
            }
            current_no++;
        } else if (current_no == 2){
            for(int i = 0; i < 10; i++){
                //落下物に対して向かっていく光のようなものを生成
                Vector3 pos = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-2.0f, 2.0f), Random.Range(-1.0f, 3.0f));
                GameObject ball = Instantiate(prefab, pos, Quaternion.identity);
                MoveSphere ms = ball.GetComponent<MoveSphere>();
                ms.setTarget(plant);
            }
            current_no++;
        } else if (current_no == 3){
            for(int i = 0; i < 10; i++){
                //落下物に対して向かっていく光のようなものを生成
                Vector3 pos = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-2.0f, 2.0f), Random.Range(-1.0f, 3.0f));
                GameObject ball = Instantiate(prefab, pos, Quaternion.identity);
                MoveSphere ms = ball.GetComponent<MoveSphere>();
                ms.setTarget(tissue);
            }
            current_no++;
        }
    }
}
