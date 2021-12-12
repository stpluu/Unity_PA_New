using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireScript : MonsterScript {
	public enum fireState
	{
		Disable,
		Ready,
		Move,
		Disapear,

	};
	private fireState state_;
	public float yMoveSpeed_;
	public Sprite[] fireImgs_;
	// Use this for initialization
	float stateTime_;
	const int spriteIndex_ready0 = 42;
	const int spriteIndex_ready1 = 43;
	const int spriteIndex_move0 = 44;
	const int spriteIndex_move1 = 40;
	const int spriteIndex_move2 = 41;
	void Start () {
		zMoveSpeed_ = 0.0f;
		yMoveSpeed_ = 0.0f;
		fireImgs_ = Resources.LoadAll<Sprite>("Sprites/Boss/sprite_set");

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (state_ == fireState.Ready)
		{
			GetComponent<SpriteRenderer>().sprite
				= fireImgs_[Mathf.RoundToInt(Time.time) > Time.time ? spriteIndex_ready0 : spriteIndex_ready1];
		}
		else if (state_ == fireState.Move)
		{
			if (transform.position.y < 0.05f)
			{
				gameObject.SetActive(false);
				
			}
			else if (transform.position.y < 2.0f)
			{
				GetComponent<SpriteRenderer>().sprite = fireImgs_[spriteIndex_move2];
			}
			else if (transform.position.y < 2.5f)
			{
				GetComponent<SpriteRenderer>().sprite = fireImgs_[spriteIndex_move1];
				GetComponent<BoxCollider>().enabled = true;
			}
			
		}
	}
	private void OnEnable()
	{
		GetComponent<BoxCollider>().enabled = false;
		state_ = fireState.Ready;
		stateTime_ = Time.time;
	}

	private void OnDisable()
	{
		state_ = fireState.Disable;
		stateTime_ = 0.0f;
		GetComponent<BoxCollider>().enabled = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.position = Vector3.zero;
	}
	private void FixedUpdate()
	{

	}
	/*
	public override void UpdateInterpolatePos(float frameMovedDist)
	{
		playerMoveInterpolatedPos_ += frameMovedDist;
		base.UpdateInterpolatePos(frameMovedDist);
		return;
	}
	*/
	public void OnCollideBullet(Collider other)
	{
	}
	public void startMove(Vector3 moveVec)
	{
		GetComponent<Rigidbody>().AddForce(moveVec, ForceMode.Impulse);
		GetComponent<SpriteRenderer>().sprite = fireImgs_[spriteIndex_move0];
		stateTime_ = Time.time;
		state_ = fireState.Move;
		
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
