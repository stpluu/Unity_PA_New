using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static partial class Constant
{
    //public const float Width_INTO_LargeHole = 2.5f;
	public const int needDepth = 3;
}
public class BossPinScript : MonoBehaviour {

    public int currentDepth_;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnEnable()
    {
        currentDepth_ = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        
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
    public void OnCharacterLanding()
    {
        if (currentDepth_ < Constant.needDepth)
        {
            currentDepth_++;
            
        }
    }
}
