using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constant
{
	public enum BossState
	{
		Deactive,
		Meet,
		Moving,
		AttackStart,
		AttackDelay,
		Die,
		Max,
	};
	
	public const float Pattern_MeetTime = 1.0f;
	public const float Pattern_MovingTerm = 2.0f;
	public const float Pattern_AttackTerm = 1.5f;
	public const float Pattern_AttackDelay = 1.0f;
	public const float Pattern_DieStart = 3.0f;
	public const float Pattern_DieDrop = 1.0f;
}
public class BossScript : MonoBehaviour {

	public float stateTime_;
	public float dieTime_;
	public int currentHp_;
	public Constant.BossState bossState_;
	// Use this for initialization
	void Start () {

		stateTime_ = Time.time;

	}
	
	// Update is called once per frame
	void Update () {
		//UpdatePosition();
		if (bossState_ == Constant.BossState.Die
			&& Time.time - dieTime_ > 8.0f)
		{
			gameObject.SetActive(false);
		}
	}
	
	public void changeState(Constant.BossState nextState)
	{
		// before change State
		switch(bossState_)
		{
			case Constant.BossState.Deactive:
			break;
			default:
			break;
		}
		
		switch(nextState)
		{
			case Constant.BossState.Meet:
			OnMeet();
			break;
			default:
			break;
		}
		bossState_ = nextState;
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
		GetComponent<BoxCollider>().enabled = false;
		currentHp_ = 30;
		dieTime_ = 0.0f;
		bossState_ = Constant.BossState.Meet;
	}
	private void OnDisable()
	{
		GetComponent<BoxCollider>().enabled = false;

		bossState_ = Constant.BossState.Deactive;
	}

	private void OnStateExit()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Bullet"))
		{
			Debug.Log("Boss : Bullet col ");
			OnCollideBullet(other);
		}
	}

	public void OnMeet()
	{

	}
	public void OnCollideBullet(Collider other)
	{
		currentHp_--;
		if (currentHp_ <= 0)
		{
			OnHPZero();
		}
		other.GetComponent<BulletScript>().OnHitObject();
	}

	public void OnHPZero()
	{
		bossState_ = Constant.BossState.Die;
		dieTime_ = Time.time;
	}
}
