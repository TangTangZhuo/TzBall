using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PathologicalGames;
using DG.Tweening;

public class UIManager : MonoBehaviour {
	public Text scoreText;
	public Text currentLevelText;
	public Text targetLevelText;
	public Text bestScoreText;
	public Transform jumpScoretrans;
	public GameObject gameOver;
	public Slider progress;
	[HideInInspector]
	public int currentLevel;
	private int score = 0;
	private int bestScore = 0;
	private Transform player;
	private string[] comboWords;
	private int comboWordsIndex = 0;
	[HideInInspector]
	public SpawnPool jumpScorePool;
	[HideInInspector]
	public int currentScore;

	private static UIManager _instance;

	public static UIManager Instance{
		get{ return _instance;}
	}
	void Awake(){
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		GetDate ();
		UpdateText ();
		StepGenerate.Instance.SpawnObject ("JumpScore", "JumpScore", 20, out jumpScorePool);
		player = GameObject.Find ("player").transform;
		comboWords = new string[]{"perfect","beautiful","super","cool"};
	}

	public void GetDate(){
		currentLevel = PlayerPrefs.GetInt ("currentLevel", 1);
		bestScore = PlayerPrefs.GetInt ("bestScore", 0);
	}

	public void UpdateText(){
		currentLevelText.text = currentLevel.ToString();
		targetLevelText.text = (currentLevel + 1).ToString();
		bestScoreText.text = "BEST:"+bestScore.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ScoreAdd(int number){
		score += number;
		scoreText.text = "Score:"+score.ToString();
	}

	public void ClearScore(){
		score = 0;
		scoreText.text = "0";
	}

	public void UpdateBestScore(){
		if (score > bestScore) {
			bestScore = score;
			bestScoreText.text = "BEST:"+bestScore.ToString ();
			PlayerPrefs.SetInt ("bestScore", bestScore);
		}
	}

	public void SpawnJumpScore(){
		Transform jumpscore = jumpScorePool.Spawn ("JumpScore");
		Text jumpscoreText = jumpscore.GetComponent<Text> ();
		if (currentScore == currentLevel) {
			jumpscoreText.text = "+" + currentLevel;
		} else {
			jumpscoreText.text = comboWords [comboWordsIndex] + "\n+" + currentScore;
			comboWordsIndex++;
		}
		if (comboWordsIndex == comboWords.Length - 1)
			comboWordsIndex = 0;
		jumpscore.SetParent (jumpScoretrans);
		Vector2 targetPostion = Camera.main.WorldToScreenPoint (StepGenerate.Instance.tempStep.position)+Vector3.left*40;
		jumpscore.GetComponent<RectTransform> ().position = targetPostion;
		jumpscore.DOMoveY (targetPostion.y+100, 1, false).onComplete = delegate() {
			jumpScorePool.Despawn (jumpscore);
			jumpscoreText.DOFade (1, 0.1f);
		};
		jumpscoreText.DOFade (0.5f, 1);
	}

	public void showGameOver(bool show){
		gameOver.SetActive (show);
	}
}
