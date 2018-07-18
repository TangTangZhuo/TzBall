﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class PoolTest : MonoBehaviour {
	SpawnPool spawnPool;
	PrefabPool refabPool;

	// Use this for initialization
	void Start () {
		spawnPool = PoolManager.Pools ["Test"];
		refabPool = new PrefabPool (Resources.Load<Transform> ("step"));
		//默认初始化两个Prefab
		refabPool.preloadAmount = 2;
		//开启限制
		refabPool.limitInstances = true;
		//关闭无限取Prefab
		refabPool.limitFIFO = false;
		//限制池子里最大的Prefab数量
		refabPool.limitAmount =5;
		//开启自动清理池子
		refabPool.cullDespawned = true;
		//最终保留
		refabPool.cullAbove = 10;
		//多久清理一次
		refabPool.cullDelay = 5;
		//每次清理几个
		refabPool.cullMaxPerPass =5;
		//初始化内存池
		spawnPool._perPrefabPoolOptions.Add(refabPool);
		spawnPool.CreatePrefabPool(spawnPool._perPrefabPoolOptions[spawnPool.Count]);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			spawnPool.Spawn ("step");
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			spawnPool.DespawnAll ();
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			spawnPool.Clear();
		}
	}
		
}
