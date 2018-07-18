using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepGenerate : MonoBehaviour {
	private static StepGenerate _instance = null;
	public GameObject step;
	private GameObject firstStep;
	private GameObject secondStep;
	public GameObject generateWall;
	public float stepDistance = 10;

	void Awake(){
		_instance = this;
	}

	public static StepGenerate Instance{
		get{ return _instance;}
	}

	// Use this for initialization
	void Start () {
		if (step == null || generateWall == null)
			Debug.Log ("null gameobject");
		firstStep = this.gameObject;
		secondStep = Instantiate (step, GameObject.Find ("Steps").transform);
		secondStep.transform.position = firstStep.transform.position + Vector3.right * stepDistance;
		InstStep (5);
	}
		
	// Update is called once per frame
	void Update () {
		
	}

	//生成台阶
	public void InstStep(int number){
		GameObject obj = step;
		firstStep = secondStep;
		for (int i = 0; i < number; i++) {
			if (i == 0) {
				Instantiate (generateWall, firstStep.transform.position, firstStep.transform.rotation);
			}
			secondStep = Instantiate (obj, GameObject.Find ("Steps").transform);
			System.Random rd = new System.Random ();
			int offset = rd.Next (-1, 2);
			Vector3 offsetX = new Vector3 (offset, 0, 0);
			secondStep.transform.position = new Vector3(firstStep.transform.position.x,0,0) + Vector3.right * stepDistance + Vector3.up * offset;
			firstStep = secondStep;
		}
	}
		
}
