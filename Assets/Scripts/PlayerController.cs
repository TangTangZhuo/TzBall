﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	Transform player;
	Rigidbody2D playerRig;
	public float upSpeed = 5;
	private float moveSpeed = 0;
	public float resetSpeed = 10;
	//public float gravityScale = 20;
	public float forceDown = 20;
	public float commonScale = 2;
	public float rightSpeed = 7;
	public float rotateSpeed = 10;
	public float resetRotate = 10;
	public Transform mainCamera;
	private bool isStart = false;
	private bool isReset = true;
	private Vector3 birthPosition;
	private Slider progess;

	private int score = 0;
	private int currentLevel;
	private int combo=1;
	bool isPerfect = false;
	bool isWin = false;
	bool isStep = false;
	// Use this for initialization
	void Start () {
		player = this.transform;
		playerRig = player.GetComponent<Rigidbody2D> ();
		birthPosition = player.position;
		currentLevel = PlayerPrefs.GetInt ("currentLevel", 1);
		progess = UIManager.Instance.progress;
	}

	// Update is called once per frame
	void Update () {
		progess.value = player.position.x;
		if (Vector3.Distance (mainCamera.position, new Vector3 (player.position.x + 1, mainCamera.position.y, mainCamera.position.z)) < 3) {

			if (Input.GetKeyDown (KeyCode.Q)) {
				if (!isWin) {
					//playerRig.gravityScale = gravityScale;
					playerRig.AddForce(Vector2.down*forceDown);
					if (!isStart)
						isStart = true;
				}
			}

			#if UNITY_ANDROID || UNITY_IOS
			if(Input.touchCount==1)
			{
				if(Input.touches[0].phase==TouchPhase.Began)
				{
					if(!isWin){
						//playerRig.gravityScale = gravityScale;
						playerRig.AddForce(Vector2.down*forceDown);
						if (!isStart)
							isStart = true;
					}
				}
			}
			#endif
		}
		mainCamera.position = Vector3.Lerp (mainCamera.position, new Vector3 (player.position.x + 1, mainCamera.position.y, mainCamera.position.z), Time.deltaTime*resetSpeed);
		player.Translate (Vector3.right * moveSpeed * Time.deltaTime,Space.World);
		player.Rotate (0, 0, rotateSpeed * Time.deltaTime,Space.World);	

		if (!isReset) {
			player.position = birthPosition;
			if (Vector3.Distance (player.position, birthPosition) < 0.1) {
				isReset = true;
				isStart = false;
			}
		}

		if (!isStart) {
			if (Vector3.Distance (player.position, birthPosition) < 0.6) {
				//playerRig.gravityScale = 5;
				playerRig.AddForce(Vector2.down*forceDown/2);
				UpdataProgress ();
			}
		}


	}
		
	void OnCollisionEnter2D(Collision2D coll) {
		Transform obj = coll.transform;

		if (obj.tag == "step") {
			RaycastHit2D[] hitArray = Physics2D.RaycastAll (transform.position, Vector2.down);
			foreach (RaycastHit2D hit in hitArray) {
				if (hit.collider.tag == "perfect") {
					isPerfect = true;
				}
				if (hit.collider.tag == "step") {
					isStep = true;
				} 
			}

			if (isStep) {
				if (moveSpeed == 0 && isStart) {
					moveSpeed = rightSpeed;
				} 
			
				playerRig.AddForce (Vector2.up * upSpeed);
				//playerRig.gravityScale = commonScale;

				obj.DOPunchPosition (Vector3.down / 10, 0.4f, 8, 0.3f, false);
				obj.parent.Find ("perfect").DOPunchPosition (Vector3.down / 10, 0.4f, 8, 0.3f, false);
				//获取当前位置生成加分UI
				StepGenerate.Instance.tempStep = obj;

				if (isStart) {
					if (isPerfect) {
						combo += 1;
						isPerfect = false;
					} else {
						combo = 1;
					}

					score = currentLevel * combo;
					UIManager.Instance.currentScore = score;
					UIManager.Instance.ScoreAdd (score);
					UIManager.Instance.SpawnJumpScore ();
				}

				isStep = false;

				if (combo >= 3 && combo < 5) {
					EffectManager.Instance.smokeParticle.Play();
				} else if (combo >= 5) {
					EffectManager.Instance.smokeParticle.Stop();
					EffectManager.Instance.smokeRed.Play ();
				} else {
					EffectManager.Instance.smokeParticle.Stop();
					EffectManager.Instance.smokeRed.Stop ();
				}
			} else {
				player.GetComponent<CircleCollider2D> ().isTrigger = true;
				moveSpeed = 1;
				//playerRig.gravityScale = gravityScale;
				//playerRig.AddForce(Vector2.down*forceDown);
			}
		}
	}
		
	void OnTriggerEnter2D(Collider2D coll){
		if (coll.transform.tag == "End") {
			Time.timeScale = 0.5f;
			isWin = true;
			Invoke ("GameWin", 0.5f);
		}
		if (coll.transform.tag == "DeadLine") {
			UIManager.Instance.showGameOver (true);
			UIManager.Instance.Complete ();
			moveSpeed = 0;
			playerRig.gravityScale = 0;
		}
	}

	public void GameOver(){
		player.GetComponent<CircleCollider2D> ().isTrigger = false;
		UIManager.Instance.showGameOver (false);
		playerRig.gravityScale = 2;
		isReset = false;
		combo = 1;
		StepGenerate.Instance.stepPool.DespawnAll ();
		StepGenerate.Instance.endPool.DespawnAll();
		StepGenerate.Instance.secondStep.position = GameObject.Find ("step").transform.position;
		StepGenerate.Instance.InstLevel (UIManager.Instance.currentLevel);
		UIManager.Instance.UpdateBestScore ();
		UIManager.Instance.ClearScore ();
	}

	public void GameWin(){
		Time.timeScale = 1;
		moveSpeed = 0;
		//playerRig.gravityScale = 1;
		currentLevel += 1;
		UIManager.Instance.currentLevel = currentLevel;
		UIManager.Instance.UpdateText ();
		UIManager.Instance.progress.maxValue = StepGenerate.Instance.distanceEnd;
		UIManager.Instance.UpdateBestScore ();
		PlayerPrefs.SetInt ("currentLevel", currentLevel);
		isReset = false;
		isWin = false;
		combo = 1;
		StepGenerate.Instance.stepPool.DespawnAll ();
		StepGenerate.Instance.endPool.DespawnAll();
		StepGenerate.Instance.secondStep.position = GameObject.Find ("step").transform.position;
		StepGenerate.Instance.InstLevel (currentLevel);
		UIManager.Instance.UpdateBestScore ();
	}

	void UpdataProgress(){
		UIManager.Instance.progress.maxValue = StepGenerate.Instance.distanceEnd;
		UIManager.Instance.progress.minValue = StepGenerate.Instance.startPos;

	}
}