using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour
{
    private GameObject target;
    private Transform _transform;
	private Vector3 _startPos;
	private Vector3 _targetPos;
	private float _startTime;
	private Vector3 _velocity = Vector3.zero;
	
	public float SmoothTime = 15.0f;
	public float Speed = 0.3f;
	public float JourneyLength = 10f;
    public float deleteTime = 4.0f;
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _startPos = _transform.position;
        _startTime = Time.time;
        Destroy(gameObject, deleteTime);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //あるターゲットにむかってふわふわしながら向かい、着いたら消える処理
        transform.position = Vector3.SmoothDamp(_transform.position, target.transform.position, ref _velocity, SmoothTime);
    }

    //ターゲットを設定
    public void setTarget(GameObject obj){
        target = obj;
    }

	float CalcMoveRatio()
	{
		var distCovered = (Time.time - _startTime) * Speed;
		return distCovered / JourneyLength;
	}
}
