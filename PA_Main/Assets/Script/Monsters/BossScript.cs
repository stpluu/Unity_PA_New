using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public partial class Constant
{
	public enum BossState
	{
		Deactive,
		Closing,
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
	private enum BossBodyImage
	{
		MovingLeft = 0,
		Neutral = 1,
		MovingRight = 2,
		Dummy = 3,
	}
	private enum BossHeadImage
	{
		Left = 0,
		Neutral = 1,
		Right = 2,
	}
	public float stateTime_;
	public float dieTime_;
	public int currentHp_;

	public int currentMovingDirection_;
	public int currentHeadDirection_;
	public Constant.BossState bossState_;

	public GameObject currentFire_;

	public Sprite[] bodyImages_;
	public Sprite[] faceImages_;
	public Sprite[] faceDieImages_;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//UpdatePosition();
		if (bossState_ == Constant.BossState.Die
			&& Time.time - dieTime_ > 8.0f)
		{
			gameObject.SetActive(false);
		}
		switch(bossState_)
		{
			case Constant.BossState.Meet:
				procMeet();
			break;
			case Constant.BossState.Moving:
				procMoving();
				break;
			case Constant.BossState.AttackStart:
				procAttackStart();
			break;
			case Constant.BossState.AttackDelay:
				procAttackDelay();
			break;
			case Constant.BossState.Die:
				break;
			default:
			break;
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
			case Constant.BossState.Closing:
				OnClosing();
				break;
			case Constant.BossState.Meet:
				OnMeet();
			break;
			case Constant.BossState.Moving:
				OnMoveStart();
				break;
			case Constant.BossState.Die:
				OnDie();
				break;
			case Constant.BossState.AttackStart:
				OnAttackStart();
			break;
			case Constant.BossState.AttackDelay:
				OnAttackDelay();
			break;
			default:
			break;
		}
		bossState_ = nextState;
		stateTime_ = Time.time;
	}

	public void procMeet()
	{
		if (Time.time - stateTime_ > 2.0f)
		{
			changeState(Constant.BossState.Moving);
			GetComponent<BoxCollider>().enabled = true;
		}
	}
	public void procMoving()
	{
		if (currentMovingDirection_ == 0)
		{
			//0은 이동을 안해서 일정시간후 Attack 바로 시작
			if (Time.time - stateTime_ > 1.0f)
			{
				changeBodyImage(BossBodyImage.Neutral);
				changeState(Constant.BossState.AttackStart);
			}
		}
		else if (currentMovingDirection_ == 1)
		{
			if (Time.time - stateTime_ > 1.0f)
			{
				changeBodyImage(BossBodyImage.MovingRight);
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + 0.6f, gameObject.transform.position.y, gameObject.transform.position.z);
				currentMovingDirection_ = 0;
				stateTime_ = Time.time;
			}
		}
		else if (currentMovingDirection_ == -1)
		{
			if (Time.time - stateTime_ > 1.0f)
			{
				changeBodyImage(BossBodyImage.MovingLeft);
				gameObject.transform.position = new Vector3(gameObject.transform.position.x - 0.6f, gameObject.transform.position.y, gameObject.transform.position.z);
				currentMovingDirection_ = 0;
				stateTime_ = Time.time;
			}
		}
		//head image 
		switch(getAttackDirection())
		{
			case -1:
				changeHeadImage(BossHeadImage.Left);
				break;
			case 1:
				changeHeadImage(BossHeadImage.Right);
				break;
			case 0:
				changeHeadImage(BossHeadImage.Neutral);
				break;
		}

	}
	public void procAttackStart()
	{
		if (Time.time - stateTime_ > 1.0f)
		{
			changeState(Constant.BossState.AttackDelay);
		}
	}
	public void procAttackDelay()
	{
		if (Time.time - stateTime_ > 1.3f)
		{
			changeState(Constant.BossState.Moving);
		}
	}
	private void OnEnable()
	{
		Debug.Log("Boss - Enabled");
		currentHeadDirection_ = 0;
		currentMovingDirection_ = 0;
		transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(2).gameObject.SetActive(false);
		GetComponent<BoxCollider>().enabled = false;
		currentHp_ = 30;
		dieTime_ = 0.0f;
		
		faceImages_ = Resources.LoadAll<Sprite>("Sprites/Boss/boss_face");
		bodyImages_ = Resources.LoadAll<Sprite>("Sprites/Boss/boss_body");
		faceDieImages_ = Resources.LoadAll<Sprite>("Sprites/Boss/boss_face_die");
		changeState(Constant.BossState.Closing);
		changeBodyImage(BossBodyImage.Neutral);
		changeHeadImage(BossHeadImage.Neutral);
		
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
	public void OnClosing()
	{

	}
	public void OnMeet()
	{


	}
	public void OnMoveStart()
	{
		currentMovingDirection_ = Random.Range(-1, 2);
		Debug.Log("Boss Move Direction : " + currentMovingDirection_.ToString());
		if (gameObject.transform.position.x > 2.5f)
		{
			currentMovingDirection_  = -1;
		}
		else if (gameObject.transform.position.x < -2.5f)
		{
			currentMovingDirection_ = 1;
		}
		//if (currentFire_)
		//{
		//	currentFire_.SetActive(false);
		//	currentFire_ = null;
		//}

	}
	public void OnAttackStart()
	{
		GameManagerScript gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

		//gameManagerScript.OnBossAttackStart(fireStartPos);
		int attackDirection = getAttackDirection();
		float fireStartPosX = gameObject.transform.position.x;

		if ( attackDirection == -1 )
		{
			fireStartPosX -= 0.7f;
		}
		else if (attackDirection == 1)
		{
			fireStartPosX += 0.7f;
		}
		Vector3 fireStartPos = new Vector3(fireStartPosX, 2.4f, 0.0f);
		currentFire_ = gameManagerScript.GetMonsterObjectInstance(Constant.MapMonsters.BOSS_FIRE);
		if (currentFire_)
		{
			currentFire_.transform.position = fireStartPos;
			currentFire_.SetActive(true);
		}
	}
	public void OnAttackDelay()
	{
		if (currentFire_)
		{
			//currentFire_;
			currentFire_.GetComponent<BossFireScript>().startMove(new Vector3(getAttackDirection() * 1.5f, -1.1f, 0.0f));
			
			
		}
	}
	public void OnDie()
	{
		transform.GetChild(2).gameObject.SetActive(true);
		transform.GetChild(0).gameObject.SetActive(false);
	}
	public void OnCollideBullet(Collider other)
	{
		currentHp_--;
	
		other.GetComponent<BulletScript>().OnHitObject();
	}

	
	//private
	private void changeBodyImage(BossBodyImage index) 
	{
		//string spriteName = string.Format("Sprites/Boss/boss_body_{0:D1}", (int)index);
		//transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load(spriteName, typeof(Sprite)) as Sprite;;
		//Sprite[] bodyImages = Resources.LoadAll<Sprite>("Sprites/Boss/boss_body");
		transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = bodyImages_[(int)index];
		switch(index)
		{
			
			case BossBodyImage.MovingLeft:
				transform.GetChild(0).transform.localPosition = new Vector3(-0.035f, 0.18f, 0.0f);
				break;
			case BossBodyImage.MovingRight:
				transform.GetChild(0).transform.localPosition = new Vector3(0.035f, 0.18f, 0.0f);
				break;
			case BossBodyImage.Neutral:
			default:
				transform.GetChild(0).transform.localPosition = new Vector3(0.0f, 0.18f, 0.0f);
				break;
		}
	}
	private void changeHeadImage(BossHeadImage index) 
	{
		//string spriteName = string.Format("Sprites/Boss/boss_face_{0:D1}", (int)index);
		//transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load(spriteName, typeof(Sprite)) as Sprite;;

		transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = faceImages_[(int)index];
	}
	private void changeFaceDieImage()
	{
	}

	private int getAttackDirection()
	{
		float playerposGap = GameObject.Find("Player").transform.position.x - transform.position.x;
		if (playerposGap > 0.7f)
		{
			return 1;
		}
		else if (playerposGap < -0.7f)
		{
			return -1;
		}
		else
		{
			return 0;
		}
	}
}
