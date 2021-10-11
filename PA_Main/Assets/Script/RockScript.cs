using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnEnable()
	{
		//Debug.Log("Rock - Enabled");
		GetComponent<BoxCollider>().enabled = true;
	}
	private void OnDisable()
	{
		GetComponent<BoxCollider>().enabled = false;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			//Debug.Log("Crack: Player col ");
			GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideRock(gameObject);
			GetComponent<BoxCollider>().enabled = false;
		}
	}
}
