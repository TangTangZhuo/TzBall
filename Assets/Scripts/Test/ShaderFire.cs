using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderFire : MonoBehaviour {
	Shader shader;
	Shader defaultShader;

	void Start () {
		shader = Shader.Find ("Shader Forge/Dissolve2D");
		defaultShader = Shader.Find ("Shader Forge/defautShader");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DestroyWithFire(Transform trans){
		float value = 0.26f;
		Material material;
		material = trans.GetComponent<SpriteRenderer> ().material;
		material.shader = shader;
		StartCoroutine (IDestroyWithFire(value,material));
	}

	IEnumerator IDestroyWithFire(float value ,Material material){
		for (float timer = 0; timer < 1; timer += Time.deltaTime/15) {
			value = Mathf.Lerp (value, 0.76f, timer);
			material.SetFloat ("_value", value);
			if (Mathf.Abs (value - 0.76f) < 0.0001f) {
				material.shader = defaultShader;
				continue;
			}
			yield return 0;
		}
	}
		
}
