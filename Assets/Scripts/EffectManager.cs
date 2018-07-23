using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {
	public Transform smokeParticle;
	public Transform player;

	private static EffectManager _instance;
	public static EffectManager Instance{
		get{ return _instance;}
	}
	void Awake(){
		_instance = this;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		smokeParticle.position = player.position + Vector3.left / 2;
	}
}
