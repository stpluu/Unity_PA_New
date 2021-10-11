using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        //Debug.Log("Crack - Enabled");
        GetComponent<BoxCollider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Crack: Player col ");
            GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideCrack(gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
