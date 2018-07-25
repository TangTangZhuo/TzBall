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
	public SpawnPool step1Pool;
	[HideInInspector]
	public SpawnPool step2Pool;
	[HideInInspector]
	public SpawnPool step3Pool;
	[HideInInspector]
	public SpawnPool step4Pool;
	[HideInInspector]
	public SpawnPool step5Pool;
	[HideInInspector]
	public SpawnPool step6Pool;
	[HideInInspector]
	public SpawnPool[] stepPools;
	public Transform tempStep;
	Vector3 localScale;

	void Awake(){
		_instance = this;
	}

	public static StepGenerate Instance{
		get{ return _instance;}
	}

	// Use this for initialization
	void Start () {
		SpawnObject ("Step1", "step1", 10, out step1Pool);
		SpawnObject ("Step2", "step2", 10, out step2Pool);
		SpawnObject ("Step3", "step3", 10, out step3Pool);
		SpawnObject ("Step4", "step4", 10, out step4Pool);
		SpawnObject ("Step5", "step5", 10, out step5Pool);
		SpawnObject ("Step6", "step6", 10, out step6Pool);
		stepPools = new SpawnPool[]{ step1Pool, step2Pool, step3Pool, step4Pool, step5Pool, step6Pool };

		SpawnObject ("End", "End", 1, out endPool);
		firstStep = this.transform;
		secondStep = step4Pool.Spawn("step4");
		secondStep.position = firstStep.position + Vector3.right * (stepDistance-1);
		//localScale = Resources.Load<Transform> ("step6").Find ("step").localScale;
		//secondStep.Find("step").localScale = new Vector3 (localScale.x * 6, localScale.y, localScale.z);
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
		int currentLevel = UIManager.Instance.currentLevel;
		//print (currentLevel);
		System.Random rd = new System.Random ();
		for (int i = 0; i < number; i++) {
			


			//距离,高度,宽度
			int offsetX = rd.Next (-1, 1);
			float offsetY = rd.Next (-1, 2);
			int offsetW = 6;

			if (currentLevel == 1) {
				offsetX = 0;
				offsetY = 0;
			} else if (currentLevel <= 10) {
				if (offsetY == -1) {
					offsetW = rd.Next (4, 6);
				}else if (offsetY == 1) {
					offsetY = 2;
					offsetW = 6;
				}

			} else if (currentLevel <= 20) {
				if (offsetY == -1) {
					offsetW = rd.Next (4, 6);
				} else if (offsetY == 1) {
					offsetW = 6;
					offsetY = 2;
				} else if (offsetY == 0) {
					offsetW = rd.Next (2, 4);
				}
			} else {
				if (offsetY == -1) {
					offsetW = rd.Next (4, 6);
				} else if (offsetY == 1) {
					offsetW = 6;
					offsetY = 1.5f;
				} else if (offsetY == 0) {
					offsetW = rd.Next (1, 4);
				}
			}

			secondStep = stepPools [offsetW - 1].Spawn ("step" + offsetW);

			//Vector3 offset = new Vector3 (offsetX, 0, 0);
			secondStep.position = new Vector3(firstStep.position.x,0,0) + Vector3.right * (stepDistance+offsetX) + Vector3.up * offsetY;
			//secondStep.Find ("step").localScale = new Vector3 (localScale.x * offsetW, localScale.y, localScale.z);
		
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
