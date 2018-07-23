using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using System;

public class StepGenerate : MonoBehaviour {
	private static StepGenerate _instance = null;
	private Transform firstStep;
	public float distanceEnd;
	public float startPos;
	[HideInInspector]
	public Transform secondStep;
	public float stepDistance = 10;
	[HideInInspector]
	public SpawnPool endPool;
	[HideInInspector]
	public SpawnPool stepPool;
	public Transform tempStep;

	void Awake(){
		_instance = this;
	}

	public static StepGenerate Instance{
		get{ return _instance;}
	}

	// Use this for initialization
	void Start () {
		SpawnObject ("Step", "step", 20, out stepPool);
		SpawnObject ("End", "End", 1, out endPool);
		firstStep = this.transform;
		secondStep = stepPool.Spawn("step");
		secondStep.position = firstStep.position + Vector3.right * stepDistance;
		InstLevel (UIManager.Instance.currentLevel);
		startPos = tempStep.position.x;
	}
		
	// Update is called once per frame
	void Update () {
		
	}

	//生成台阶
	public void InstStep(int number){
		//GameObject obj = step;
		firstStep = secondStep;
		for (int i = 0; i < number; i++) {
			secondStep = stepPool.Spawn("step");
			System.Random rd = new System.Random ();
			int offset = rd.Next (-1, 2);
			Vector3 offsetX = new Vector3 (offset, 0, 0);
			secondStep.position = new Vector3(firstStep.position.x,0,0) + Vector3.right * stepDistance + Vector3.up * offset;

			firstStep = secondStep;
			if (i == number-1) {
				Transform end = endPool.Spawn("End");
				end.position = firstStep.position + Vector3.right * stepDistance / 2;
				distanceEnd = end.position.x - startPos;
			}
		}
	}

	//生成关卡
	public void InstLevel(int level){
		InstStep (18 + (int)Math.Ceiling (level / 5.0));
	}

	//将对象放入对象池	
	public void SpawnObject(string poolName,string objName,int objNumber,out SpawnPool spawnPool){
		spawnPool = PoolManager.Pools [poolName];
		PrefabPool prefabPool = new PrefabPool (Resources.Load<Transform> (objName));
		prefabPool.preloadAmount = objNumber;
		spawnPool._perPrefabPoolOptions.Add(prefabPool);
		spawnPool.CreatePrefabPool(spawnPool._perPrefabPoolOptions[spawnPool.Count]);
	}
}
