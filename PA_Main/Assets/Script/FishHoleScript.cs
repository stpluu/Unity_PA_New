using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Constant
{
    public const float Distance_FishCreate = 18.0f;
}
public class FishHoleScript : MonoBehaviour {
    bool bFishCreated;
	// Use this for initialization
	void Start () {
        
    }
    private void Awake()
    {
       
    }
    private void OnEnable()
    {
        //Debug.Log("FishHole - Enabled");
        GetComponent<BoxCollider>().enabled = true;
        bFishCreated = false;
    }
    // Update is called once per frame
    void Update () {
        if (bFishCreated == false)
        {
            if (transform.position.z < Constant.Distance_FishCreate)
            {
                //Debug.Log("FishHole - FishCreate");
                GameObject obj = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().GetFishInstance();
                bFishCreated = true;
                obj.transform.position = transform.position;
                obj.SetActive(true);
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //Debug.Log("Fish Hole : Player col ");
            GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideCrack(gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }

}
