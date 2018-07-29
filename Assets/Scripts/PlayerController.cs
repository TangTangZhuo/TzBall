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
	public Transform bg1;
	public Transform bg2;
	private Vector3 bg1Pos;
	private Vector3 bg2Pos;
	private float bgDistance;
	private Color[] colors;

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

	ShaderFire shaderFire;
	// Use this for initialization
	void Start () {
		player = this.transform;
		playerRig = player.GetComponent<Rigidbody2D> ();
		birthPosition = player.position;
		currentLevel = PlayerPrefs.GetInt ("currentLevel", 1);
		progess = UIManager.Instance.progress;
		bg1Pos = bg1.position;
		bg2Pos = bg2.position;
		bgDistance = bg2Pos.x - bg1Pos.x;
		shaderFire =  gameObject.GetComponent<ShaderFire> ();
		colors = new Color[] {new Color (255/255f, 201/255f, 201/255f), new Color (255/255f, 201/255f, 237/255f), 
			new Color (222/255f, 201/255f, 255/255f),new Color (201/255f, 234/255f, 255/255f), 
			new Color (201/255f, 255/255f, 231/255f), new Color (246/255f, 255/255f, 201/255f), 
			new Color (167/255f, 167/255f, 167/255f),new Color (255/255f, 255/255f, 255/255f)
		};
	}

//	void FixedUpdate(){
//		
//	}

	// Update is called once per frame
	void Update () {
		progess.value = player.position.x;
		if (Vector3.Distance (mainCamera.position, new Vector3 (player.position.x + 1, mainCamera.position.y, mainCamera.position.z)) < 3) {

			if (Input.GetKeyDown (KeyCode.Q)) {
				if (!isWin) {
					//playerRig.gravityScale = gravityScale;
					playerRig.AddForce(Vector2.down*forceDown);
					if (!isStart) {
						isStart = true;
						UIManager.Instance.bestScoreText.gameObject.SetActive (false);
					}
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
						if (!isStart){
							isStart = true;
							UIManager.Instance.bestScoreText.gameObject.SetActive (false);
						}
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
				//playerRig.AddForce(Vector2.down*forceDown/5);
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
					if (isStart)
						hit.transform.Find ("Particle").GetComponent<ParticleSystem> ().Play ();
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

				//obj.DOPunchPosition (Vector3.down / 10, 0.4f, 8, 0.3f, false);
				obj.parent.DOPunchPosition (Vector3.down / 10, 0.4f, 8, 0.3f, false);
				//GameObject.FindWithTag("bg").transform.DOPunchPosition (Vector3.down / 10, 0.4f, 8, 0.3f, false);
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
					EffectManager.Instance.smokeParticle.Play();
					EffectManager.Instance.smokeRed.Play ();
					shaderFire.DestroyWithFire (obj.parent.Find("stepIma"));

				} else {
					EffectManager.Instance.smokeParticle.Stop();
					EffectManager.Instance.smokeRed.Stop ();

				}
			} else {
				player.GetComponent<CircleCollider2D> ().isTrigger = true;
				moveSpeed = 0;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.transform.tag == "End") {
			Time.timeScale = 0.5f;
			isWin = true;
			EffectManager.Instance.fra.ExplodeChunks(30,(Vector3.left*30),0);
			Invoke ("GameWin", 0.5f);
		}
		if (coll.transform.tag == "DeadLine") {
			UIManager.Instance.showGameOver (true);
			UIManager.Instance.Complete ();
			moveSpeed = 0;
			playerRig.gravityScale = 0;
		}
		if (coll.name == "bg") {
			coll.transform.position += new Vector3 (bgDistance, 0, 0) * 2;
		}
	}

	public void GameOver(){
		player.GetComponent<CircleCollider2D> ().isTrigger = false;
		ResetBackground ();
		UIManager.Instance.showGameOver (false);
		playerRig.gravityScale = 2;
		isReset = false;
		combo = 1;
		for (int i = 0; i < StepGenerate.Instance.stepPools.Length; i++) {
			StepGenerate.Instance.stepPools [i].DespawnAll ();
		}
		for (int i = 0; i < StepGenerate.Instance.draftPools.Length; i++) {
			StepGenerate.Instance.draftPools [i].DespawnAll ();
		}
		StepGenerate.Instance.secondStep.position = GameObject.Find ("step").transform.position;
		StepGenerate.Instance.InstLevel (UIManager.Instance.currentLevel);
		UIManager.Instance.UpdateBestScore ();
		UIManager.Instance.ClearScore ();
		UIManager.Instance.bestScoreText.gameObject.SetActive (true);
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
		ResetBackground ();
		ChangeBGColor ();
		for (int i = 0; i < StepGenerate.Instance.stepPools.Length; i++) {
			StepGenerate.Instance.stepPools [i].DespawnAll ();
		}
		for (int i = 0; i < StepGenerate.Instance.draftPools.Length; i++) {
			StepGenerate.Instance.draftPools [i].DespawnAll ();
		}
		StepGenerate.Instance.secondStep.position = GameObject.Find ("step").transform.position;
		StepGenerate.Instance.InstLevel (currentLevel);
		UIManager.Instance.UpdateBestScore ();
		EffectManager.Instance.fra.ResetChunks ();
		UIManager.Instance.bestScoreText.gameObject.SetActive (true);
		player.position = birthPosition;
		mainCamera.position = new Vector3 (birthPosition.x + 1, mainCamera.position.y, mainCamera.position.z);

	}

	void UpdataProgress(){
		UIManager.Instance.progress.maxValue = StepGenerate.Instance.distanceEnd;
		UIManager.Instance.progress.minValue = StepGenerate.Instance.startPos;

	}

	void ResetBackground(){
		bg1.position = bg1Pos;
		bg2.position = bg2Pos;
	}

	void ChangeBGColor(){
		System.Random rand = new System.Random ();
		SpriteRenderer spriteR = bg1.GetComponent<SpriteRenderer> ();
		int i = 0;
		while (i == 0) {
			int index = rand.Next (0, 8);
			Color color = colors [index];
			if (color != spriteR.color) {
				spriteR.DOColor(colors [index], 0.5f);
				bg2.GetComponent<SpriteRenderer> ().DOColor(colors [index], 0.5f);;
				i = 1;
			}
		}
	}

}