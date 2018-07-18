using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {
	Transform player;
	Rigidbody2D playerRig;
	public float upSpeed = 5;
	private float moveSpeed = 0;
	public float resetSpeed = 10;
	public float gravityScale = 20;
	public float commonScale = 2;
	public float rightSpeed = 7;
	public Transform mainCamera;
	private bool isStart = false;
	private bool isReset = true;
	private Vector3 birthPosition;
	// Use this for initialization
	void Start () {
		player = this.transform;
		playerRig = player.GetComponent<Rigidbody2D> ();
		birthPosition = player.position;
	}

	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (mainCamera.position, new Vector3 (player.position.x + 1, mainCamera.position.y, mainCamera.position.z)) < 3) {
			if (Input.GetKeyDown (KeyCode.Q)) {
				playerRig.gravityScale = gravityScale;
				if (!isStart)
					isStart = true;
			}
		}
		mainCamera.position = Vector3.Lerp (mainCamera.position, new Vector3 (player.position.x + 1, mainCamera.position.y, mainCamera.position.z), Time.deltaTime*resetSpeed);
		player.Translate (Vector3.right * moveSpeed * Time.deltaTime);

		if (!isReset) {
			player.position = birthPosition;
			if (Vector3.Distance (player.position, birthPosition) < 0.1) {
				isReset = true;
				isStart = false;
			}
		}
	}

	void FixedUpdate() {
		
	}
		
	void OnCollisionEnter2D(Collision2D coll) {
		Transform obj = coll.transform;
		if (obj.tag == "step") {
			playerRig.AddForce(Vector2.up * upSpeed);
			playerRig.gravityScale = commonScale;
			if (moveSpeed == 0 && isStart) {
				moveSpeed = rightSpeed;
			} 
			obj.DOPunchPosition (Vector3.down/10, 0.4f, 8, 0.3f, false);
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (coll.transform.tag == "generateWall") {
			StepGenerate.Instance.InstStep (5);
		}
		if (coll.transform.tag == "DeadLine") {
			//player.position = birthPosition;
			isReset = false;
			moveSpeed = 0;
			StepGenerate.Instance.stepPool.DespawnAll ();
			StepGenerate.Instance.wallPool.DespawnAll ();
			StepGenerate.Instance.secondStep.position = GameObject.Find ("step").transform.position;
			StepGenerate.Instance.InstStep (5);
		}
	}
}