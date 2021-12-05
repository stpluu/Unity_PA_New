using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireScript : MonsterScript {

	public float yMoveSpeed_;
	// Use this for initialization
	void Start () {
		zMoveSpeed_ = 0.0f;
		yMoveSpeed_ = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void FixedUpdate()
	{
		if (monsterState_ == Constant.MonsterState.Active)
		{

			Vector3 moveVector = new Vector3();
			moveVector = Vector3.zero;

			moveVector.y = yMoveSpeed_;

		}
		//GetComponent<Rigidbody>().AddForce(moveVector, ForceMode.Impulse);
	}
	/*
	public override void UpdateInterpolatePos(float frameMovedDist)
	{
		playerMoveInterpolatedPos_ += frameMovedDist;
		base.UpdateInterpolatePos(frameMovedDist);
		return;
	}
	*/
	public void setMoveSpeed(Vector3 moveVec)
	{
		
	}	
	public override float CalcYPos()
	{
		return 0.0f;
	}
	public override float CalcZPos()
	{
		return 0.0f;
	}
}
