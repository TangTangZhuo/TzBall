using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderFire : MonoBehaviour {
	public Material targetMaterial;
	public Material defaultMaterial;
	bool isIE = true;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DestroyWithFire(Transform trans){
		float value = 0.26f;
		Material dMaterial = defaultMaterial;
		SpriteRenderer sr = trans.GetComponent<SpriteRenderer> ();
		sr.material = targetMaterial;
		StartCoroutine (IDestroyWithFire(value,sr,dMaterial));
	}

	IEnumerator IDestroyWithFire(float value ,SpriteRenderer sr,Material dMaterial){
			for (float timer = 0; timer < 1; timer += Time.deltaTime /15) {
				value = Mathf.Lerp (value, 0.76f, timer);
				sr.material.SetFloat ("_value", value);
				if (Mathf.Abs (value - 0.76f) < 0.0001f) {
					sr.material = dMaterial;
					continue;
				}
				yield return 0;
			}
	}
		
}
