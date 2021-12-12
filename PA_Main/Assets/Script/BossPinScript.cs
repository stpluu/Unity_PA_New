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
        if (transform.position.y <= 0.0f)
		{
            
            Vector3 pos = new Vector3(transform.position.x, 0.0f, transform.position.z);
            transform.position = pos;
            GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, 0.0f, 0.0f), ForceMode.VelocityChange);
		}
	}
    private void OnEnable()
    {
        currentDepth_ = 0;
        GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, -6.0f, 0.0f), ForceMode.VelocityChange);
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
            string spriteName = string.Format("Sprites/Boss/boss_point_{0:D1}", (Constant.needDepth - currentDepth_));
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
        }
    }
}
