using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Constant
{
	public const float BulletMoveSpeed = 6.0f;
	public const float BulletMaxDistance = 5.0f;
}
;
public class BulletScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}


	private void OnEnable()
	{
		GetComponent<BoxCollider>().enabled = true;

		Vector3 moveVector = new Vector3();
		moveVector = Vector3.zero;

		moveVector.z = Constant.BulletMoveSpeed;
		// if ()

		GetComponent<Rigidbody>().AddForce(moveVector, ForceMode.Impulse);

	}
	private void OnDisable()
	{
		//GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.Force);
		GetComponent<BoxCollider>().enabled = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.position = Vector3.zero;
	}
	// Update is called once per frame
	void Update()
	{
		//if (transform.position.y < -1.0f)
		// {
		//    StopCoroutine("UpdateFishPosition");
		//}
		if (transform.position.z > Constant.BulletMaxDistance)
		{
			//Debug.Log("Fish - deactivated");
			gameObject.SetActive(false);
		}

	}

	private void FixedUpdate()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Monster"))
		{
			//Debug.Log("Bullet - Monster Trigger");
			//GameObject.Find("Monster").GetComponent<MonsterScript>().OnCollideBullet(gameObject);
			//GetComponent<BoxCollider>().enabled = false;
			
		}
	}

	public void OnHitObject()
	{
		gameObject.SetActive(false);
	}
}
