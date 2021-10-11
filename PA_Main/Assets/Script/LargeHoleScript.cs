using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static partial class Constant
{
    public const float Width_INTO_LargeHole = 2.5f;
	
}
public class LargeHoleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnEnable()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Large Hole : col ");
        if (other.CompareTag("Player"))
        {
           // Debug.Log("Large Hole : Player col ");
            GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideLargeHole(gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
   /*
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Large Hole : col ");
        if (col.CompareTag("Player"))
        {
            Debug.Log("Large Hole : Player col ");
            GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideLargeHole(gameObject);
        }
    }
    */
}
