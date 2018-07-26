using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomTest : MonoBehaviour {
	public Text text;
	// Use this for initialization
	void Start ()
	{

		for (int i = 0; i < 10; i++) {
			System.Random random = new System.Random ();
			int j = random.Next (1, 7);
			Debug.Log (j);
			text.text += j+"+";

		}

	}
}

