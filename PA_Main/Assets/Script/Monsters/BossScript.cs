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
	// Use this for initialization
	void Start () {

		stateTime_ = Time.time;
		currentHeadDirection_ = 0;
		currentMovingDirection_ = 0;
		transform.GetChild(2).gameObject.SetActive(false);
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

	public void procMoving()
	{
		if (currentMovingDirection_ == 0)
		{
			//0은 이동을 안해서 일정시간후 Attack 바로 시작
			if (Time.time - stateTime_ > 0.5f)
			{
				changeState(Constant.BossState.AttackStart);
			}
		}
		else if (currentMovingDirection_ == 1)
		{
			if (Time.time - stateTime_ > 1.0f)
			{
				changeBodyImage(BossBodyImage.Neutral);
				changeState(Constant.BossState.AttackStart);
			}
			else if (Time.time - stateTime_ > 0.5f)
			{
				changeBodyImage(BossBodyImage.MovingLeft);
			}
		}
		else if (currentMovingDirection_ == 2)
		{
			if (Time.time - stateTime_ > 1.0f)
			{
				changeBodyImage(BossBodyImage.Neutral);
				changeState(Constant.BossState.AttackStart);
			}
			else if (Time.time - stateTime_ > 0.5f)
			{
				changeBodyImage(BossBodyImage.MovingRight);
			}
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
		if (Time.time - stateTime_ > 1.0f)
		{
			changeState(Constant.BossState.Moving);
		}
	}
	private void OnEnable()
	{
		//Debug.Log("Monster - Enabled");
		GetComponent<BoxCollider>().enabled = false;
		currentHp_ = 30;
		dieTime_ = 0.0f;
		bossState_ = Constant.BossState.Closing;
		faceImages_ = Resources.LoadAll<Sprite>("Sprites/Boss/boss_face");
		bodyImages_ = Resources.LoadAll<Sprite>("Sprites/Boss/boss_body");
		changeBodyImage(BossBodyImage.Neutral);
		
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
	public void OnMoveStart()
	{
		currentMovingDirection_ = Random.Range(0, 3);
		if (gameObject.transform.position.x > 2.5f)
		{
			currentMovingDirection_  = 1;
		}
		else if (gameObject.transform.position.x < -2.5f)
		{
			currentMovingDirection_ = -1;
		}
		if (currentFire_)
		{
			currentFire_.SetActive(false);
			currentFire_ = null;
		}

	}
	public void OnAttackStart()
	{
		GameManagerScript gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
		Vector3 fireStartPos = new Vector3(gameObject.transform.position.x, 3.0f, 0.0f);
		//gameManagerScript.OnBossAttackStart(fireStartPos);
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
			currentFire_.GetComponent<Rigidbody>().useGravity = true;
		}
	}
	public void OnDie()
	{

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
	}
	private void changeHeadImage(BossHeadImage index) 
	{
		//string spriteName = string.Format("Sprites/Boss/boss_face_{0:D1}", (int)index);
		//transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load(spriteName, typeof(Sprite)) as Sprite;;

		transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = faceImages_[(int)index];
	}
}
