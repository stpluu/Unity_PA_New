using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constant
{
	public enum MonsterState
	{
		Deactive,
		Active,
		Die,
		Max,
	};
	public const float Monster_Die_Speed = 7.0f;
}
public class MonsterScript : MonoBehaviour {

	public float zMoveSpeed_;
	public float enabledTime_;
	public float dieTime_;
	public float playerMoveInterpolatedPos_;
	public Constant.MonsterState monsterState_;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//UpdatePosition();
		if (monsterState_ == Constant.MonsterState.Die
			&& Time.time - dieTime_ > 2.0f)
		{
			gameObject.SetActive(false);
		}
	}
	public virtual void UpdateInterpolatePos(float frameMovedDist)
	{
		//playerMoveInterpolatedPos_ = 0.0f;
		//Debug.Log("Monster - Update InterpolatePos :" + playerMoveInterpolatedPos_.ToString());
	}
	public virtual float CalcYPos()
	{
		return 0.0f;
	}
	public virtual float CalcZPos()
	{
		return 0.0f;
	}
	
	private void OnEnable()
	{
		//Debug.Log("Monster - Enabled");
		GetComponent<BoxCollider>().enabled = true;
		playerMoveInterpolatedPos_ = 0.0f;
		enabledTime_ = Time.time;
		dieTime_ = 0.0f;
		monsterState_ = Constant.MonsterState.Active;
	}
	private void OnDisable()
	{
		GetComponent<BoxCollider>().enabled = false;
		enabledTime_ = 0.0f;
		monsterState_ = Constant.MonsterState.Deactive;
	}

	private void OnStateExit()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Monster : Player col ");
			GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideRock(gameObject);
			GetComponent<BoxCollider>().enabled = false;
		}
		else if (other.CompareTag("Bullet"))
		{
			Debug.Log("Monster : Bullet col ");
			OnCollideBullet(other);
		}
	}

	public void OnCollideBullet(Collider other)
	{
		// DIE MOTION
		if (gameObject.GetComponent<Animator>())
		{
			gameObject.GetComponent<Animator>().SetTrigger("trDamaged");
		}

		GetComponent<BoxCollider>().enabled = false;
		monsterState_ = Constant.MonsterState.Die;
		dieTime_ = Time.time;
		Vector3 moveVector = new Vector3();
		bool isLeft = false;
		if (other.transform.position.x > gameObject.transform.position.x)
		{
			isLeft = true;
		}

		if (isLeft)
			moveVector.x = Constant.Monster_Die_Speed * -1.0f;
		else
			moveVector.x = Constant.Monster_Die_Speed;
		moveVector.y = Constant.Monster_Die_Speed;
		//moveVector = //Quaternion.AngleAxis(Random.Range(10.0f, 70.0f), Vector3.forward) * moveVector;
		// if ()

		GetComponent<Rigidbody>().AddForce(moveVector, ForceMode.Impulse);
		other.GetComponent<BulletScript>().OnHitObject();
	}
}
