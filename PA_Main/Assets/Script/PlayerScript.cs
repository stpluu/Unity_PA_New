using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static partial class Constant
{  
     public const int Speed_Max_Lv1 = 26;
    public const int Speed_Max_Lv2 = 32;
    public const int Speed_Min = 5;
     public const float Speed_X_Lv1 = 0.04f;
     public const float Speed_X_Lv2 = 0.06f;
     public const float Speed_JumpLv1 = 0.1f;
     public const float Speed_JumpLv2 = 0.11f;

        public const float Position_jumpHeightLv1 = 0.4f;
        public const float Position_jumpHeightLv2 = 1.2f;
        public const int Position_objectAppearDist = 10;
    public const float Position_VerticalMoveLimit_Left = -6.0f;
    public const float Position_VerticalMoveLimit_Right = 6.0f;
	public const float Position_Bullet_Start_Y = 1.5f;

    public const int Direction_Left = -1;
        public const int Direction_Neutral = 0;
        public const int Direction_Right = 1;
        public const int Direction_SpeedUp = 1;
        public const int Direction_SpeedDown = -1;

    public const float Time_JumpLv1 = 0.8f;
    public const float Time_JumpLv2 = 1.4f;
    public const float Time_InHole_Min = 0.7f;
	public const float Time_InOutHole = 0.8f;
	public const float Time_DieDelay = 3.0f;

    public const int Count_Stop_Auto_Jump_Lv1 = 3;
	public static readonly float[] Time_stopJump = { 0.5f, 0.3f, 0.3f };
	public static readonly float[] Position_stopJumpHeight = { 0.5f, 0.2f, 0.2f };

	public const int Max_BulletCount = 3;
	public const float Time_BulletCoolTime = 0.5f;

	public const float Time_Invincible = 20.0f;

};

public class PlayerScript : MonoBehaviour {
    public enum CharacterStates
    {
        NONE,
        NEUTRAL, // 바닥에서 이동중
        JUMP_UP, //점프(올라가는중)
        JUMP_DOWN, //점프(내려가는중)
        STOP_LEFT,    //장애물에 걸려 좌측으로 가는중
        STOP_RIGHT, //장애물에 걸려 우측으로 가는중
        IN_THE_HOLE, //구멍에 빠진 상태
		IN_TO_THE_HOLE,	//구멍안(워프, 상점)으로 들어가는 상태
		IN_THE_SHOP,	//상점안에 있는 상태
		OUT_FROM_THE_HOLE, //워프,상점에서 나오는 상태

        DIE, //사망
		GOAL_MOVING,
        GOAL, //골에 도착
        CEREMONY, //보스 클리어

        MAX,
    };
    public Vector3 characterPosition
    {
        get
        {
            return characterPosition_;
        }
        set
        {
            characterPosition_ = value;
            if (characterPosition_.x > Constant.Position_VerticalMoveLimit_Right)
                characterPosition_.x = Constant.Position_VerticalMoveLimit_Right;
            if (characterPosition_.x < Constant.Position_VerticalMoveLimit_Left)
                characterPosition_.x = Constant.Position_VerticalMoveLimit_Left;
        }
    }
    private Vector3 characterPosition_;

    public CharacterStates characterState_;

    public int movingDirection_;    // -1: left, 0 : neutral, 1 : right
	private int reservedMovingDirection_;

    public int speedDirection_; // -1 : speedDown, 0: neutral, 1 : speedUp
    public int speed_;      //월드의 스크롤 스피드 (캐릭터 스피드)
    public float speedKeyInputTime_; //스피드 업/다운 키를 누른 시간

    public float jumpStartTime_;
    public float inHoleTime_;
    public float stateStartTime_;
    public int stopJumpCount_;  //구멍에 걸렸을때 자동으로 점프한 횟수.
	public float bulletShotTime_;
	public float invincibleStartTime_;

	private GameObject LastCollideObject_;
	private Constant.MapObjects enterShopType_;
	private GameManagerScript gameManagerScript_;
    Animator animator_;

	public float InvincibleEndTime_;
	
    void Awake()
    {
        characterState_ = CharacterStates.NEUTRAL;
        characterPosition = new Vector3(0, 0, 0);
        animator_ = gameObject.GetComponent<Animator>();
		LastCollideObject_ = null;
		gameManagerScript_ = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
		bulletShotTime_ = Time.time;
		invincibleStartTime_ = 0.0f;
	}
    // Use this for initialization
    void Start () {
        StartCoroutine("UpdateCharacterPosition");
		//StartCoroutine("UpdateSpeed");
		SetSpeed(Constant.Speed_Min);
	}

	
	// Update is called once per frame
	void Update () {
        //characterPosition_.y = transform.position.y;
        //animator_.SetFloat("playerHeight", characterPosition.y);
        updateCharacterState();
        UpdateSpeed();

		UpdateEffect();

    }

    public bool isControlableState()
    {
       if (characterState_ == CharacterStates.JUMP_DOWN
            || characterState_ == CharacterStates.JUMP_UP
            || characterState_ == CharacterStates.NEUTRAL)
        {
            return true;
        }
        return false;
    }
	public bool isVerticalMoveControlableState()
	{
		if (characterState_ == CharacterStates.NEUTRAL)
			return true;
		else if (characterState_ == CharacterStates.JUMP_DOWN 
			|| characterState_ == CharacterStates.JUMP_UP)
		{
			if (gameManagerScript_.hasItem(Constant.ItemDef.Feather))
			{
				return true;
			}
		}
		return false;
	}
    private void updateCharacterState()
    {
        switch(characterState_)
        {
            case CharacterStates.JUMP_UP:
                if (Time.time - jumpStartTime_ > getJumpTime() / 2)
                {
                    changeCharacterState(CharacterStates.JUMP_DOWN);
                }
                break;
            case CharacterStates.JUMP_DOWN:
                if (transform.position.y < 0.001f)
                {
                    changeCharacterState(CharacterStates.NEUTRAL);
                }
                break;
            case CharacterStates.STOP_LEFT:
            case CharacterStates.STOP_RIGHT:
                {
                    if (stopJumpCount_ >= Constant.Count_Stop_Auto_Jump_Lv1)
                    {
                        changeCharacterState(CharacterStates.NEUTRAL);
                    }
                }
                break;
			case CharacterStates.IN_TO_THE_HOLE:
				{
					if (Time.time - stateStartTime_ > Constant.Time_InOutHole)
					{
						changeCharacterState(CharacterStates.IN_THE_SHOP);
					}
				}
				break;
			case CharacterStates.OUT_FROM_THE_HOLE:
				{
					if (Time.time - stateStartTime_ > Constant.Time_InOutHole)
					{
						changeCharacterState(CharacterStates.IN_THE_HOLE);
					}
				}
				break;
			case CharacterStates.CEREMONY:
			case CharacterStates.GOAL_MOVING:
				{
					if (Mathf.Abs(characterPosition_.x) < 0.3f)
					{
						characterPosition_.x = 0.0f;
						changeCharacterState(CharacterStates.GOAL);
					}
				}
				break;
			case CharacterStates.DIE:
				{
					// animation에서 이벤트로 처리
					//if (Time.time - stateStartTime_ > Constant.Time_DieDelay)
					//{
					//	gameManagerScript_.OnRestratButton();
					//}
				}
				break;
            default:
                break;
        }
    }
    public void changeCharacterState(CharacterStates state)
    {
        if (state == characterState_)
            return;
       
        Debug.Log("CharacterState Change to : " + state.ToString());
        switch (state)
        {
            case CharacterStates.JUMP_UP:
                //characterPosition_.y = 0.1f;
                animator_.SetTrigger("trJumped");
                jumpStartTime_ = Time.time;
                //animator_.SetFloat("playerHeight", characterPosition.y);
                //gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, 100.0f) , ForceMode2D.Impulse);

                break;
            case CharacterStates.JUMP_DOWN:
               // animator_.SetTrigger("trLanded");
                //jumpStartTime_ = 0.0f;
                break;
            case CharacterStates.NEUTRAL:
                if (characterState_ == CharacterStates.IN_THE_HOLE)
                {
                    animator_.SetBool("bInHole", false);
                    inHoleTime_ = 0.0f;
                }
                if (characterState_ == CharacterStates.NONE
                    || characterState_ == CharacterStates.STOP_LEFT
                    || characterState_ == CharacterStates.STOP_RIGHT
                    || characterState_ == CharacterStates.IN_THE_HOLE)
                {
                    SetSpeed(Constant.Speed_Min);
					if (reservedMovingDirection_ != 0)
					{
						if (reservedMovingDirection_ != 99)
							movingDirection_ = reservedMovingDirection_;
						else
							movingDirection_ = 0;
						reservedMovingDirection_ = 0;
					}
					
				}
                if (characterState_ == CharacterStates.JUMP_DOWN
					|| characterState_ == CharacterStates.STOP_LEFT
					|| characterState_ == CharacterStates.STOP_RIGHT
					)
                {
					animator_.SetTrigger("trLanded");
					//Debug.Log("Jump End : " + movingDirection_.ToString() + " -> " + reservedMovingDirection_.ToString());
					if (reservedMovingDirection_ != 0)
					{
						if (reservedMovingDirection_ != 99)
							movingDirection_ = reservedMovingDirection_;
						else
							movingDirection_ = 0;
						reservedMovingDirection_ = 0;
					}
				
				}
				
				if (characterState_ == CharacterStates.JUMP_DOWN
					|| characterState_ == CharacterStates.JUMP_UP)
				{
					gameManagerScript_.ChangeHeartColor();
				}
				if (characterState_ == CharacterStates.JUMP_DOWN)
				{
					gameManagerScript_.OnCharacterLanding(gameObject.transform.position);
				}
                characterPosition_.y = 0.0f;
                break;
            case CharacterStates.STOP_LEFT:
                {
                    SetSpeed(0);
                    movingDirection_ = Constant.Direction_Left;
                    animator_.SetTrigger("trDamagedLeft");
                    stopJumpCount_ = 0;
                }
                break;
            case CharacterStates.STOP_RIGHT:
                {
                    SetSpeed(0);
                    movingDirection_ = Constant.Direction_Right;
                    animator_.SetTrigger("trDamagedRight");
                    stopJumpCount_ = 0;
                }
                break;
            case CharacterStates.IN_THE_HOLE:
                {
                    SetSpeed(0);
                    movingDirection_ = Constant.Direction_Neutral;
                    animator_.SetBool("bInHole", true);
                    inHoleTime_ = Time.time;
                }
                break;
			case CharacterStates.IN_TO_THE_HOLE:
				{
					SetSpeed(0);
					movingDirection_ = Constant.Direction_Neutral;
					animator_.SetBool("trInOutHole", true);
				}
				break;
			case CharacterStates.IN_THE_SHOP:
				{
					GameObject.Find("GameManager").GetComponent<GameManagerScript>()
					.OnEnterShop(LastCollideObject_, enterShopType_);
				}
				break;
			case CharacterStates.OUT_FROM_THE_HOLE:
				{
					SetSpeed(0);
					movingDirection_ = Constant.Direction_Neutral;
					animator_.SetBool("trInOutHole", true);
				}
				break;
			case CharacterStates.GOAL_MOVING:
			case CharacterStates.CEREMONY:
				{
					SetSpeed(0);
					if (characterState_ == CharacterStates.JUMP_DOWN
						|| characterState_ == CharacterStates.JUMP_UP)
						animator_.SetTrigger("trLanded");
				}
				break;
			case CharacterStates.GOAL:
				{
					movingDirection_ = Constant.Direction_Neutral;
					animator_.SetTrigger("trGoal");
				}
				break;
			case CharacterStates.DIE:
				{
					SetSpeed(0);
					movingDirection_ = Constant.Direction_Neutral;
					animator_.SetTrigger("trDied");
				}
				break;
            default:
                break;
        }
        characterState_ = state;
        stateStartTime_ = Time.time;
    }
    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    // 키입력시 처리부분
    public void onJumpKeyDown()
    {
        Debug.Log("Key - Jump");
        switch(characterState_)
        {
            case  CharacterStates.NEUTRAL:
                changeCharacterState(CharacterStates.JUMP_UP);
                break;
            case CharacterStates.IN_THE_HOLE:
                if (inHoleTime_ + Constant.Time_InHole_Min <= Time.time)
                {
                    changeCharacterState(CharacterStates.NEUTRAL);
                }
                break;
            default:
                break;
        }
        
        //구름 상태일때는 Y포지션 UPforce 추가
    }
    public void onJumpKeyUp()
    {
        //Debug.Log("Key Up - Jump");
        //if (characterState_ == CharacterStates.JUMP_UP)
        //{
        //   changeCharacterState(CharacterStates.JUMP_DOWN);
        //}
		
		if (characterState_ == CharacterStates.JUMP_UP)
		{
			if (gameManagerScript_.hasItem(Constant.ItemDef.FlyingCap)
				&& Time.time - jumpStartTime_ < Constant.Time_JumpLv1)
			{
				//changeCharacterState(CharacterStates.JUMP_DOWN);
			}
		}
    }

	
    public void onMoveKey(bool isLeft)
    {
		//Debug.Log("Key - Move");
		//characterSpeed_.x = isLeft ? -1 : 1;
		if (isVerticalMoveControlableState())
			movingDirection_ = isLeft ? -1 : 1;
		else
			reservedMovingDirection_ = isLeft ? -1 : 1;

	}
    public void onMoveKeyUp(bool isLeft)
    {
		if (isVerticalMoveControlableState())
		{
			if (isLeft)
			{
				if (movingDirection_ == -1)
				{
					movingDirection_ = 0;
				}
			}
			else
			{
				if (movingDirection_ == 1)
				{
					movingDirection_ = 0;
				}
			}
		}
        else
		{
			if (isLeft)
			{
				if (reservedMovingDirection_ == -1
					|| movingDirection_ == -1)
				{
					reservedMovingDirection_ = 99;
				}
			}
			else
			{
				if (reservedMovingDirection_ == 1
					|| movingDirection_ == 1)
				{
					reservedMovingDirection_ = 99;
				}
			}
		}
    }

    public void onSpeedKey(bool isSpeedUp)
    {
		//Debug.Log("Key - Speed +");

		if (isSpeedUp)
			speedDirection_ = 1;
		else
		{
			speedDirection_ = -1;
			// 워프홀일경우 구멍안으로 들어가도록 처리
		}
        speedKeyInputTime_ = Time.time;
    }
    public void onSpeedKeyUp(bool isSpeedUp)
    {
        //Debug.Log("Key - Speed -");
        if (isSpeedUp)
        {
            if (speedDirection_ == 1)
            {
                speedDirection_ = 0;
                speedKeyInputTime_ = 0.0f;
            }
        }
        else
        {
            if (speedDirection_ == -1)
            {
                speedDirection_ = 0;
                speedKeyInputTime_ = 0.0f;
            }
        }
    }


    public void onShotKey()
    {
		if (bulletShotTime_ + Constant.Time_BulletCoolTime > Time.time)
			return;
		GameObject obj = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().GetBulletInstance();
		if (obj != null)
		{
			Vector3 bulletInitPos = new Vector3();
			bulletInitPos = transform.position;
			bulletInitPos.y += Constant.Position_Bullet_Start_Y;
			obj.transform.position = bulletInitPos;
			obj.SetActive(true);
			bulletShotTime_ = Time.time;
		}
	}

	//GM
	public void OnRightBlockKey()
	{
		speed_ = Constant.Speed_Max_Lv2;
	}
	public void OnLeftBlockKey()
	{
		speed_ = Constant.Speed_Min;
	}

	/// coroutine
	IEnumerator UpdateCharacterPosition()
    {
		GameObject shadow = GameObject.Find("Player").transform.Find("PlayerShadow").gameObject;
        while(true)
        {
            yield return new WaitForSeconds(0.01f);
            
            //X축 이동 처리
            //Transform tf = ;
            // 맵이 굽어진 경우 force값 보정
			if (characterState_ == CharacterStates.GOAL_MOVING
				&& Mathf.Abs(characterPosition_.x) > 0.3f)
			{
				movingDirection_ = characterPosition_.x < 0.0f ? 1 : -1;
			}
			if (characterState_ == CharacterStates.GOAL)
				characterPosition_.x = 0.0f;
            //특정 아이템이 있는 경우 보정 필요함
            characterPosition_.x += Constant.Speed_X_Lv1 * movingDirection_;
            if (characterPosition_.x > Constant.Position_VerticalMoveLimit_Right)
                characterPosition_.x = Constant.Position_VerticalMoveLimit_Right;
            else if (characterPosition_.x < Constant.Position_VerticalMoveLimit_Left)
                characterPosition_.x = Constant.Position_VerticalMoveLimit_Left;

			/////////////////////////////////////////////////////////////////////////////
            // y축 이동 처리
            if (characterState_ == CharacterStates.JUMP_UP
                || characterState_ == CharacterStates.JUMP_DOWN)
            {
                float jumpDeltaTime = Time.time - jumpStartTime_;

                characterPosition_.y = calcJumpHeight(jumpDeltaTime);
                //Debug.Log("jumpingTime : " + jumpDeltaTime.ToString());
            }

            // crack 또는 hole 가장자리등에 걸렸을때 처리
            if (characterState_ == CharacterStates.STOP_LEFT
                || characterState_ == CharacterStates.STOP_RIGHT)
            {
                characterPosition_.y = calcStopJumpHeight(stopJumpCount_, Time.time - stateStartTime_);
                //Debug.Log("Stop State : y : " + characterPosition_.y.ToString());
				if (characterPosition_.y < 0.0f)
				{
					stopJumpCount_++;
					characterPosition_.y = 0.0f;
				}
            }
            if (characterState_ == CharacterStates.NEUTRAL
				|| characterState_ == CharacterStates.GOAL
				|| characterState_ == CharacterStates.GOAL_MOVING
				|| characterState_ == CharacterStates.IN_THE_HOLE)
                characterPosition_.y = 0.0f;
			if (characterState_ == CharacterStates.IN_TO_THE_HOLE)
			{
				characterPosition_.y = (stateStartTime_ - Time.time) * 2;
			}
			else if (characterState_ == CharacterStates.OUT_FROM_THE_HOLE)
			{
				characterPosition_.y = -1.6f + (Time.time - stateStartTime_) * 2;
			}
			
			float shadowScale = Mathf.Clamp(Constant.Position_jumpHeightLv2 
											- Mathf.Min(characterPosition.y, Constant.Position_jumpHeightLv2)
											, 0.2f, 1.0f);
			shadow.transform.localScale = new Vector3(shadowScale, shadowScale, 1.0f);
			
			// 
			characterPosition_.z = 0;
            //tf.position.x = characterSpeed_.x;
            //tf.position.x += characterSpeed_.x;
            //transform.position.x += characterSpeed_.x;
            transform.position = characterPosition;
			shadow.transform.position = new Vector3(characterPosition_.x, 0.0f, 0.0f);
		}
    }
    
    private void UpdateSpeed()
    {
        if (isControlableState() == false)
            return;
        //while(true)
        {
            //yield return new WaitForSeconds(0.02f);
            if (speedDirection_ == 1)
            {
                //GameObject.Find("GameManager").GetComponent<WorldScript>
                if (speedKeyInputTime_ + 0.2f < Time.time)
                {
                    if(speed_ < getMaxSpeed())
                        speed_ += 1;
                    speedKeyInputTime_ = Time.time;
                    //Debug.Log("Speed Up : " + speed_.ToString());
                }
                
            }
            else if (speedDirection_ == -1)
            {
                if (speedKeyInputTime_ + 0.15f < Time.time)
                {
                    if (speed_ > Constant.Speed_Min)
                        speed_ -= 1;
                    speedKeyInputTime_ = Time.time;
                    //Debug.Log("Speed Down : " + speed_.ToString());
                }
                
            }
        }
        animator_.SetFloat("playerSpeed", (speed_ / Constant.Speed_Max_Lv1));
    }

	private void UpdateEffect()
	{
		//RuntimeAnimatorController ac = gameObject.GetComponent<Animator>().runtimeAnimatorController;
		//gameObject.GetComponent<Animator>().runtimeAnimatorController = null;
		//
		//gameObject.GetComponent<Animator>().color = new Color(0.4f, 0.6f, 1f);
		//gameObject.GetComponent().runtimeAnimatorController = ac;
	}

    private float calcJumpHeight(float jumpDeltaTime)
    {
        //if (getJumpTime() / 2 > jumpDeltaTime)
        {
            return Mathf.Sin(Mathf.PI * ( jumpDeltaTime / getJumpTime())) * getJumpHeight();
        }
        //else
        //{
         //   return Mathf.Cos(Mathf.PI / 2 * (jumpDeltaTime - (getJumpTime() / 2))*2) * getJumpHeight();
        //}
      //  return Mathf.Sin((Mathf.PI * 2 * jumpDeltaTime) / jumpTime) * jumpHeight;
    }

	private float calcStopJumpHeight(int jumpCount, float stateDeltaTime)
	{
		if (jumpCount >= Constant.Count_Stop_Auto_Jump_Lv1)
		{
			return 0.0f;
		}
		//float[] stopJumpTime = new float[]{ 0.6f, 0.4f, 0.3f };
		float curJumpDeltaTime = stateDeltaTime;
		for (int i = 0; i < jumpCount; ++i)
		{
			curJumpDeltaTime -= Constant.Time_stopJump[i];
		}
		Debug.Log("Curr Jump : " + curJumpDeltaTime.ToString() + "JumpCount : " + stopJumpCount_.ToString()) ;
		
		//const float jumpHeight = 1.0f;
		//if (Constant.Time_stopJump[jumpCount] / 2 > curJumpDeltaTime)
		{
            return Mathf.Sin(Mathf.PI * (curJumpDeltaTime / Constant.Time_stopJump[jumpCount]))
						* Constant.Position_stopJumpHeight[jumpCount];
        }
        //else
        //{
        //    return Mathf.Cos(Mathf.PI / 2 * (curJumpDeltaTime - (Constant.Time_stopJump[jumpCount] / 2)) * 2);
        //}
    }

    public void SetSpeed(int speed)
    {
        //Debug.Log("SetSpeed : " + speed.ToString());
        speed_ = speed;
    }


    // Collide
    public void OnCollideLargeHole(GameObject collideObj)
    {
        float collidePos = gameObject.transform.position.x - collideObj.transform.position.x;
        //characterPosition_.y = 0.0f;
        if (Mathf.Abs(collidePos) < Constant.Width_INTO_LargeHole)
        {
            //홀에 빠짐
            changeCharacterState(CharacterStates.IN_THE_HOLE);
        }
        else if (collidePos < 0.0f)
        {
            //왼쪽에 부딪힘
            changeCharacterState(CharacterStates.STOP_LEFT);
        }
        else
        {
            //오른쪽에 부딪힘
            changeCharacterState(CharacterStates.STOP_RIGHT);
        }
    }

    public void OnCollideCrack(GameObject collideObj)
    {
        float collidePos = gameObject.transform.position.x - collideObj.transform.position.x;
        //characterPosition_.y = 0.0f;
        if (collidePos < 0.0f)
        {
            changeCharacterState(CharacterStates.STOP_LEFT);
        }
        else
        {
            changeCharacterState(CharacterStates.STOP_RIGHT);
        }
    }

	// Shop
	public void OnCollideShop(GameObject collideObj, Constant.MapObjects shopType)
	{
		LastCollideObject_ = collideObj;
		float collidePos = gameObject.transform.position.x - collideObj.transform.position.x;
		//characterPosition_.y = 0.0f;
		if (Mathf.Abs(collidePos) < Constant.Width_INTO_ShopHole)
		{
			//홀에 빠짐
			changeCharacterState(CharacterStates.IN_THE_HOLE);
			//shop이므로 바로 구멍 안으로 들어가는 트리거 발동
			//animator_.SetTrigger("trInOutHole");
			changeCharacterState(CharacterStates.IN_TO_THE_HOLE);
			enterShopType_ = shopType;
		}
		else if (collidePos < 0.0f)
		{
			//왼쪽에 부딪힘
			changeCharacterState(CharacterStates.STOP_LEFT);
		}
		else
		{
			//오른쪽에 부딪힘
			changeCharacterState(CharacterStates.STOP_RIGHT);
		}
	}

	public void OnCollideRock(GameObject collideObj)
	{
		//Todo : 무적/아이템 활성화상태면 통과
		
		Debug.Log("Rock Collide : " + collideObj.transform.position.z.ToString() + "player : "
			+ gameObject.transform.position.z.ToString());

		OnDie();
	}

	public void OnDie()
	{
		changeCharacterState(CharacterStates.DIE);
		gameManagerScript_.onDie(0);

	}

	// animation event func
	public void EventGoalCeremonyPhase_1()
	{

	}
	public void EventGoalCeremonyPhase_2()
	{

	}
	public void EventGoalCeremonyPhase_3()
	{
		//gameManagerScript_.loadStage(gameManagerScript_.currentStage_+1);
		gameManagerScript_.goNextStage();
	}
	public void EventDieAnimationEnd()
	{
		gameManagerScript_.OnRestratButton();
	}
	/// <summary>
	/// /////////////////////////////
	/// </summary>
	///
	
	public void OnExitFromHole() // shop, warp에서 나왔을때
	{
		changeCharacterState(CharacterStates.OUT_FROM_THE_HOLE);
	}
	/////////////////////////////
	// goal
	public void OnGoal(bool isBossClear)
	{
		if (isBossClear)
		{
			changeCharacterState(CharacterStates.CEREMONY);
		}
		else
		{
			changeCharacterState(CharacterStates.GOAL_MOVING);
		}
		
	}

	private int getMaxSpeed()
	{
		if (gameManagerScript_.hasItem(Constant.ItemDef.GreenShoes))
			return Constant.Speed_Max_Lv2;
		return Constant.Speed_Max_Lv1;
	}
	private float getJumpHeight()
	{
		if (gameManagerScript_.hasItem(Constant.ItemDef.FlyingCap))
			return Constant.Position_jumpHeightLv2;
		return Constant.Position_jumpHeightLv1;
	}
	private float getJumpTime()
	{
		if (gameManagerScript_.hasItem(Constant.ItemDef.FlyingCap))
			return Constant.Time_JumpLv2;
		return Constant.Time_JumpLv1;
	}

	public float GetInvincibleRemainTime()
	{
		return Time.time - InvincibleEndTime_;
	}
	public bool IsInvincible()
	{
		return GetInvincibleRemainTime() > 0.0f;
	}
}
