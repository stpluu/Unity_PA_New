using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constant
{
	public enum HeartColor
	{
		Purple, //Score
		Green,	//Time
		Sky,	//Cloud
		Yellow, //Invincible
		Max,
	};
	public const float HeartMoveSpeedX = 1.0f;
	public const float HeartInitialPosX = -7.0f;
	public const float HeartCosMoveY = 1.3f;
};

public class HeartScript : MonoBehaviour {
	public Constant.HeartColor currentHeartColor;
	bool isMoveLeft_;
	public float createTime_;
	public Vector3 startPosition_;

	private Sprite[] heartSprites_;
	// Use this for initialization

	private void Awake()
	{
		heartSprites_ = new Sprite[(int)Constant.HeartColor.Max];
		for (int i = 0; i < (int)Constant.HeartColor.Max; ++i)
		{
			string spriteName = string.Format("Sprites/Item/Heart_{0:D1}", i);
			heartSprites_[i] = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
		}
	}
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		float xPos = (Time.time - createTime_) * Constant.HeartMoveSpeedX;
		if (isMoveLeft_)
			xPos *= -1.0f;
		transform.position = startPosition_ + new Vector3(xPos
			, Mathf.Sin((Time.time - createTime_) * 2) * Constant.HeartCosMoveY, 0.0f);
		if (transform.position.x < -8.0f
			|| transform.position.x > 8.0f)
			gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		currentHeartColor = Constant.HeartColor.Purple;
		gameObject.GetComponent<SpriteRenderer>().sprite = heartSprites_[(int)currentHeartColor];
		createTime_ = Time.time;
		GetComponent<BoxCollider>().enabled = true;
		if (GameObject.Find("Player").GetComponent<PlayerScript>().characterPosition.x > 0.0f)
		{
			isMoveLeft_ = false;
		}
		else
		{
			isMoveLeft_ = true;
		}
		Vector3 pos = new Vector3();
		pos.x = Constant.HeartInitialPosX;
		if (isMoveLeft_)
		{
			pos.x *= -1.0f;
		}
		pos.y = 1.0f;
		pos.z = 0.0f;

		transform.position = pos;
		startPosition_ = pos;
		Debug.Log("Heart Enabled : isMoveLeft : " + isMoveLeft_.ToString());
	}
	private void OnDisable()
	{
		//GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.Force);
		GetComponent<BoxCollider>().enabled = false;
		Debug.Log("Heart Disabled / Last Pos : " + transform.position.ToString());
		transform.position = Vector3.zero;
		
	}
	public void ChangeToNextColor()
	{
		currentHeartColor++;
		WorldScript.StageStyle style = GameObject.Find("GameManager").GetComponent<WorldScript>().GetStageStyle();
		if (currentHeartColor == Constant.HeartColor.Sky
			&& (style == WorldScript.StageStyle.Swimming
				|| style == WorldScript.StageStyle.UnderTheSea)
			)
		{
			currentHeartColor++;
		}
		if (currentHeartColor == Constant.HeartColor.Yellow
			&& GameObject.Find("Player").GetComponent<PlayerScript>().IsInvincible())
		{
			currentHeartColor++;
		}
		
		if (currentHeartColor == Constant.HeartColor.Max)
			currentHeartColor = Constant.HeartColor.Purple;
		gameObject.GetComponent<SpriteRenderer>().sprite = heartSprites_[(int)currentHeartColor];
		Debug.Log("Heart colorChange : " + currentHeartColor.ToString());
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			//Debug.Log("Fish : Player col ");
			//GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideLargeHole(gameObject);
			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().OnCollideHeart(gameObject, currentHeartColor);
			GetComponent<BoxCollider>().enabled = false;
			Debug.Log("Heart collide");
		}
	}
}
