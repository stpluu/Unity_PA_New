using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBallScript : MonsterScript {

	// Use this for initialization
	void Start () {
		zMoveSpeed_ = -3.0f;
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}

	private void FixedUpdate()
	{
		if (monsterState_ == Constant.MonsterState.Active)
		{

			Vector3 moveVector = new Vector3();
			moveVector = Vector3.zero;

			moveVector.z = zMoveSpeed_;

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

	public override float CalcYPos()
	{
		return CalcZPos() * 0.3f * 0.25f;
	}
	public override float CalcZPos()
	{
		return (Constant.Distance_ObjectAppear_          //interpolate world scale
					+ zMoveSpeed_ * (Time.time - enabledTime_)) * 4.0f; // monster move speed, character move
	}
}
