using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
static partial class Constant
{
    public const int Distance_ObjectAppear_ =  15;
    public const int Distance_ObjectDisappear = 5;
    public const int Distance_MonsterAppear_ = 20;
	public const float Speed_WorldScrool = 0.3f;
    public const int Number_BackGroundImg = 12;
	public enum MapObjects
	{
		LHOLE,
		LHOLE_UPG,
		SHOP_NORMAL,
		SHOP_EXPENSIVE,
		SHOP_SANTA,
		CRACK,
		CRACK_UPG,
		FISH_HOLE,
		GOAL,
		BOSS,
		BOSS_HOLE,
		ROCK,
		TREE,
		WARP,
		HEART,
		CURVE_LEFT_START,
		CURVE_RIGHT_START,
		CURVE_END,
		SPECIAL_1,
		SPECIAL_2,
		SPECIAL_3,
		WING,
		BOSS_PIN,
	};
	public enum MapMonsters
	{
		BALL,
		BAT,
		CLOUD,
		SQUID,
//		BOSS,
		BOSS_FIRE,
	};
}
public class WorldScript : MonoBehaviour {
    
    public enum StageStyle
    {
        SpringForest,
        WinterForest,
        RockField,
        Cave,
        IceWorld,

        UnderTheSea,
        Swimming,

    };
    public float distance_;   //진행한 거리

    public int curveDirection_; //맵이 구부러지고 있는 방향 : -1 : 좌, 0 : 직진, 1 : 우측

    public int stageMaxDistance_;	//스테이지 전체 길이
    public int stageMaxTime_;		//스테이지 제한시간
	
	public bool isBossStage_;
	public float bossFightStartTime_;
    private StageStyle stageStyle_;	//스테이지 종류 (숲/눈/황무지/동굴/남극/수중/수면...)

    private GameObject backGround_;	//배경 오브젝트
    private Sprite[] bgSprites_;    //배경 이미지들 (12개 한세트, 0~3 : 직진, 4~7 : 좌회전중, 8~11 : 우회전중)
    private int currentBgNum_;      //현재 렌더링 되는 배경 이미지

	[System.Serializable]
	struct MapObjectStruct
	{
		public Constant.MapObjects objectType_;
		public int distance_;           //오브젝트가 나타날 위치
		public int horizonalPosition_;  //등장시 왼쪽/오른쪽 위치, 혹은 좌표
		public GameObject object_;      //게임 오브젝트 본체
		public bool bReady_;
		public void SetReady(bool bReady)
		{
			bReady_ = bReady;
		}
		public bool isPassed_;
    };

	[System.Serializable]
	struct MapMonsterStruct
	{
		public Constant.MapMonsters monsterType_;
		public int distance_;   //몬스터 등장 위치
		public int horizonalPosition_; //등장 위치 (좌/중앙/우) -1, 0, 1
		public GameObject object_;
		public bool bReady_;
		public bool bPassed_;
		public void SetReady(bool bReady)
		{
			bReady_ = bReady;
		}

	};
    private List<MapObjectStruct> objectList_;  //맵 오브젝트 리스트

	private List<MapMonsterStruct> monsterList_;

	public Dictionary<int, int> sellItemList_ { get; set; }	//해당스테이지에서 판매하는 아이템/가격(일반상점기준)

	private int speedKeyInputTime_;     //스피드 업/다운 키를 누른 시간

	bool bGoal_;						//스테이지 목표에 도착 했는가?
	public StageStyle GetStageStyle()
	{
		return stageStyle_;
	}
	public bool SetStageStyle(string stageType)
	{
		if (stageType.Equals("spring_forest"))
		{
			stageStyle_ = StageStyle.SpringForest;
		}
		else if (stageType.Equals("cave"))
		{
			stageStyle_ = StageStyle.Cave;
		}
		else if (stageType.Equals("ice_world"))
		{
			stageStyle_ = StageStyle.IceWorld;
		}
		else if (stageType.Equals("rock_field"))
		{
			stageStyle_ = StageStyle.RockField;
		}
		else if (stageType.Equals("swimming"))
		{
			stageStyle_ = StageStyle.Swimming;
		}
		else if (stageType.Equals("under_the_sea"))
		{
			stageStyle_ = StageStyle.UnderTheSea;
		}
		else if (stageType.Equals("winter_Forest"))
		{
			stageStyle_ = StageStyle.WinterForest;
		}
		else
		{
			return false;
		}

		switch (stageStyle_)
		{
			case StageStyle.SpringForest:
				for (int i = 0; i < Constant.Number_BackGroundImg; ++i)
				{
					string spriteName = string.Format("Sprites/Spring_Forest/BG_{0:D2}", i + 1);

					bgSprites_[i] = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
				}
				backGround_.GetComponent<SpriteRenderer>().sprite = bgSprites_[currentBgNum_];
				break;
			case StageStyle.Cave:
				for (int i = 0; i < Constant.Number_BackGroundImg; ++i)
				{
					string spriteName = string.Format("Sprites/Cave/BG_{0:D2}", i + 1);

					bgSprites_[i] = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
				}
				backGround_.GetComponent<SpriteRenderer>().sprite = bgSprites_[currentBgNum_];
				break;
			case StageStyle.IceWorld:
				for (int i = 0; i < Constant.Number_BackGroundImg; ++i)
				{
					string spriteName = string.Format("Sprites/Ice_World/BG_{0:D2}", i + 1);

					bgSprites_[i] = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
				}
				backGround_.GetComponent<SpriteRenderer>().sprite = bgSprites_[currentBgNum_];
				break;
			default:
				for (int i = 0; i < Constant.Number_BackGroundImg; ++i)
				{
					bgSprites_[i] = Resources.Load("Sprites/Spring_Forest/BG_{0:D2}" + i + 1, typeof(Sprite)) as Sprite;
				}
				backGround_.GetComponent<SpriteRenderer>().sprite = bgSprites_[currentBgNum_];
				break;
		}
		return true;
	}

	public bool addObject(int distance,	string objectType, int hPos)
	{
		if (stageMaxDistance_ <= 0
			|| distance > stageMaxDistance_)
			return false;
		
		// 오브젝트 생성(실제생성은 나중에)
		MapObjectStruct obj = new MapObjectStruct();
		
		///////////////////////////////////////////////////////////////
		if (objectType.Equals("boss"))
		{
			obj.objectType_ = Constant.MapObjects.BOSS;
		}
		else if (objectType.Equals("fish_hole"))
		{
			obj.objectType_ = Constant.MapObjects.FISH_HOLE;
		}
		else if (objectType.Equals("crack"))
		{
			obj.objectType_ = Constant.MapObjects.CRACK;
		}
		else if (objectType.Equals("crack_upg"))
		{
			obj.objectType_ = Constant.MapObjects.CRACK_UPG;
		}
		else if (objectType.Equals("curve_end"))
		{
			obj.objectType_ = Constant.MapObjects.CURVE_END;
		}
		else if (objectType.Equals("curve_left"))
		{
			obj.objectType_ = Constant.MapObjects.CURVE_LEFT_START;
		}
		else if (objectType.Equals("curve_right"))
		{
			obj.objectType_ = Constant.MapObjects.CURVE_RIGHT_START;
		}
		else if (objectType.Equals("goal"))
		{
			obj.objectType_ = Constant.MapObjects.GOAL;
		}
		else if (objectType.Equals("heart"))
		{
			obj.objectType_ = Constant.MapObjects.HEART;
		}
		else if (objectType.Equals("large_hole"))
		{
			obj.objectType_ = Constant.MapObjects.LHOLE;
		}
		else if (objectType.Equals("large_hole_upg"))
		{
			obj.objectType_ = Constant.MapObjects.LHOLE_UPG;
		}
		else if (objectType.Equals("rock"))
		{
			obj.objectType_ = Constant.MapObjects.ROCK;
		}
		else if (objectType.Equals("shop_expensive"))
		{
			obj.objectType_ = Constant.MapObjects.SHOP_EXPENSIVE;
		}
		else if (objectType.Equals("shop_normal"))
		{
			obj.objectType_ = Constant.MapObjects.SHOP_NORMAL;
		}
		else if (objectType.Equals("shop_santa"))
		{
			obj.objectType_ = Constant.MapObjects.SHOP_SANTA;
		}
		else if (objectType.Equals("special_1"))
		{
			obj.objectType_ = Constant.MapObjects.SPECIAL_1;
		}
		else if (objectType.Equals("special_2"))
		{
			obj.objectType_ = Constant.MapObjects.SPECIAL_2;
		}
		else if (objectType.Equals("special_3"))
		{
			obj.objectType_ = Constant.MapObjects.SPECIAL_3;
		}
		else if (objectType.Equals("tree"))
		{
			obj.objectType_ = Constant.MapObjects.TREE;
		}
		else if (objectType.Equals("warp"))
		{
			obj.objectType_ = Constant.MapObjects.WARP;
		}
		else if (objectType.Equals("wing"))
		{
			obj.objectType_ = Constant.MapObjects.WING;
		}
		else
		{
			return false;
		}
		obj.distance_ = distance;
		obj.horizonalPosition_ = hPos;
		obj.bReady_ = false;
		obj.isPassed_ = false;
		objectList_.Add(obj);
		
		return true;
	}
	public bool addMonster(int distance, string monsterType, int hPos)
	{
		if (stageMaxDistance_ <= 0
			|| distance > stageMaxDistance_)
			return false;

		// 오브젝트 생성(실제생성은 나중에)
		MapMonsterStruct obj = new MapMonsterStruct();

		///////////////////////////////////////////////////////////////
//		if (monsterType.Equals("boss"))
//		{
//			obj.monsterType_ = Constant.MapMonsters.BOSS;
//		}
//		else 
		if (monsterType.Equals("ball"))
		{
			obj.monsterType_ = Constant.MapMonsters.BALL;
		}
		
		else
		{
			return false;
		}
		obj.distance_ = distance;
		obj.horizonalPosition_ = hPos;
		obj.bReady_ = false;
		obj.bPassed_ = false;
		monsterList_.Add(obj);

		return true;
	}
	public void addShopItem(int itemID, int goodShopPrice)
	{
		Debug.Log("add shop Item : " + itemID.ToString() + " / " + goodShopPrice.ToString());
		sellItemList_[itemID] = goodShopPrice;
	}
	void CreateObjects()
    {
      
    }

    private void Awake()
    {
        backGround_ = GameObject.FindWithTag("Background");
        currentBgNum_ = 0;
		isBossStage_ = false;
		bossFightStartTime_ = 0.0f;
        bgSprites_ = new Sprite[Constant.Number_BackGroundImg];
		bGoal_ = false;

		objectList_ = new List<MapObjectStruct>();
		monsterList_ = new List<MapMonsterStruct>();
		sellItemList_ = new Dictionary<int, int>();

	}
    void Start () {

		//loadStage(1);
		objectList_.Clear();
		monsterList_.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private bool IsItemObject(Constant.MapObjects objType)
	{
		if (objType == Constant.MapObjects.HEART
			|| objType == Constant.MapObjects.WING)
			return true;
		return false;
	}
    private void updateObjectPosition()
    {
        int iDistance = (int)distance_;
        Vector3 curPostionVector = new Vector3();

		for (int i = 0; i < objectList_.Count; ++i)
		{

			MapObjectStruct mapObj = objectList_[i];
			if (mapObj.isPassed_)
				continue;
			if (IsItemObject(mapObj.objectType_) == true)
			{
				if (iDistance < mapObj.distance_)
					continue;
				
			}
			else
			{
				//너무 멀리 있는 오브젝트 패스
				if (iDistance < mapObj.distance_ - Constant.Distance_ObjectAppear_)
					break;
				if (iDistance > mapObj.distance_ + Constant.Distance_ObjectDisappear)
				{
					if (mapObj.object_ != null)
					{
						mapObj.object_.SetActive(false);
						Debug.Log("object deactived : " + mapObj.distance_.ToString() + " type : " + mapObj.objectType_.ToString());
						mapObj.object_ = null;
						mapObj.isPassed_ = true;
						objectList_[i] = mapObj;
						
					}
					continue;
				}
			}
           

			// 여기서부턴 거리에 들어와있는 오브젝트들
			if (mapObj.object_ == null)
			{
				mapObj.object_
					= GameObject.FindGameObjectWithTag(
						"GameController").GetComponent<GameManagerScript>().GetMapObjectInstance(mapObj.objectType_);
				if (mapObj.object_ == null)
				{
					Debug.LogError("map object get error - dist : " + mapObj.distance_.ToString()
						+ " type : " + mapObj.objectType_.ToString());
					continue;
				}
				if (mapObj.objectType_ == Constant.MapObjects.SHOP_EXPENSIVE
					|| mapObj.objectType_ == Constant.MapObjects.SHOP_NORMAL
					|| mapObj.objectType_ == Constant.MapObjects.SHOP_SANTA)
				{
					mapObj.object_.GetComponent<ShopHoleScript>().SetShopType(mapObj.objectType_);
				}
				//if (mapObj.objectType_ != Constant.MapObjects.BOSS)
				mapObj.object_.SetActive(true);
			}
			if (IsItemObject(mapObj.objectType_))
			{
				mapObj.isPassed_ = true;
			}
			else if (mapObj.objectType_ == Constant.MapObjects.BOSS_PIN)
			{

			}
			else if (mapObj.objectType_ == Constant.MapObjects.BOSS)
			{
				// 보스 x축은 이동 스스로 처리함
				curPostionVector.x = mapObj.object_.transform.position.x;
				curPostionVector.y = calcObjectYPos(mapObj.objectType_, mapObj.distance_);
				curPostionVector.z = calcObjectZpos(mapObj.objectType_, mapObj.distance_);
				mapObj.object_.transform.position = curPostionVector;
			}
			else
			{
				curPostionVector.x = calcObjectXPos(mapObj.objectType_, mapObj.horizonalPosition_, mapObj.distance_);
				curPostionVector.y = calcObjectYPos(mapObj.objectType_, mapObj.distance_);
				curPostionVector.z = calcObjectZpos(mapObj.objectType_, mapObj.distance_);
				mapObj.object_.transform.position = curPostionVector;
			}
			
			objectList_[i] = mapObj;
        }
        
    }

    public void updateBG()
    {
        ++currentBgNum_;
        if ((currentBgNum_ % 4) == 0)
            currentBgNum_ -= 4;
        backGround_.GetComponent<SpriteRenderer>().sprite = bgSprites_[currentBgNum_];
    }

	private void UpdateMonsterPosition(float frameMoveDistance)
	{
		int iDistance = (int)distance_;
		Vector3 curPostionVector = new Vector3();

		for (int i = 0; i < monsterList_.Count; ++i)
		{

			MapMonsterStruct mapMon = monsterList_[i];

			if (mapMon.monsterType_ == Constant.MapMonsters.BOSS_FIRE)
				continue;
			//너무 멀리 있는 오브젝트 패스
			if (iDistance < mapMon.distance_ - Constant.Distance_ObjectAppear_)
				break;

			if (mapMon.bPassed_ == true)
				continue;
			

			// 여기서부턴 거리에 들어와있는 오브젝트들
			if (mapMon.object_ == null)
			{
				mapMon.object_
					= GameObject.FindGameObjectWithTag(
						"GameController").GetComponent<GameManagerScript>().GetMonsterObjectInstance(mapMon.monsterType_);
				if (mapMon.object_ == null)
				{
					Debug.LogError("map object get error - dist : " + mapMon.distance_.ToString()
						+ " type : " + mapMon.monsterType_.ToString());
					continue;
				}
				
				mapMon.object_.SetActive(true);
			}
			MonsterScript monScript = mapMon.object_.GetComponent<MonsterScript>();
			if (monScript.monsterState_ == Constant.MonsterState.Active)
			{
				curPostionVector.x = calcMonsterXPos(mapMon.monsterType_, mapMon.horizonalPosition_, mapMon.distance_);
				curPostionVector.y = monScript.CalcYPos();
				curPostionVector.z = monScript.CalcZPos();
				mapMon.object_.transform.position = curPostionVector;
				//
				//Debug.Log("Monster ZPos :" + curPostionVector.z.ToString());

			}
			if (curPostionVector.z < -3.0f
				|| ((monScript.monsterState_ == Constant.MonsterState.Die) 
					&& Time.time - monScript.dieTime_ > 2.0f
					)
				)
			{
				if (mapMon.object_ != null)
				{
					mapMon.object_.SetActive(false);
					Debug.Log("object deactived : " + mapMon.distance_.ToString() + " type : " + mapMon.monsterType_.ToString());
					mapMon.object_ = null;
					mapMon.bPassed_ = true;
					monsterList_[i] = mapMon;
				}
				continue;
			}
			monsterList_[i] = mapMon;
		}

	}
	
	// 현재 거리 갱신
	public void updateDistance(int speed)
    {
        float FrameDistance = Time.deltaTime * ((float)speed * Constant.Speed_WorldScrool);
		if (speed == 0)
			FrameDistance = 0.0f;
        float prevDistance = distance_;
        distance_ += FrameDistance;
		// 1.0단위로 bg / object 위치를 업데이트 한다(끊어지는 듯한 연출)
        if ((int)(distance_) > (int)prevDistance)
        {
            updateObjectPosition();
            updateBG();
        }
		UpdateMonsterPosition(FrameDistance);//몬스터는 매프레임 위치 갱신
		// 최종 거리 도달할경우 골인 처리 시작

		if (bGoal_ == false
			&& distance_ >= stageMaxDistance_ )
		{
			if (isBossStage_)
			{
				if (bossFightStartTime_ == 0.0f)
				{
					bossFightStartTime_ = Time.time;
					GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().OnMeetBoss();
					//BossScript script = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().GetMapObjectInstance(Constant.MapObjects.BOSS).GetComponent<BossScript>();
					//script.changeState(Constant.BossState.Meet)
				}
			}
			else
			{
				bGoal_ = true;
				distance_ = stageMaxDistance_;
			
				GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().OnGoal();
			}
			
		}
		// 보스 조우했을경우 남은거리 0 고정
		if (bossFightStartTime_ > 0.0f)
		{
			distance_ = stageMaxDistance_;
		}
    }

    public void onCurveObject(int objDirection)
    {
        switch(objDirection)
        {
            case Constant.Direction_Left:
                currentBgNum_ = 4;
                break;
            case Constant.Direction_Right:
                currentBgNum_ = 8;
                break;
            case Constant.Direction_Neutral:
                currentBgNum_ = 0;
                break;
            default:
                break;
        }
        curveDirection_ = objDirection;
    }

	// 각 오브젝트에 맞는 x축 거리를 계산한다, (파일에는 x위치가 int 단위임)
    float calcObjectXPos(Constant.MapObjects objType, int hPos, int dist)
    {
        switch(objType)
        {
            case Constant.MapObjects.LHOLE:
				return (float)hPos * 3.0f;
			case Constant.MapObjects.FISH_HOLE:
            case Constant.MapObjects.CRACK:
                return (float)hPos * 3.5f;
			case Constant.MapObjects.SHOP_NORMAL:
			case Constant.MapObjects.SHOP_EXPENSIVE:
			case Constant.MapObjects.SHOP_SANTA:
				return (float)hPos * 4.0f;
			case Constant.MapObjects.ROCK:
				return (float)hPos * 4.8f;
            default:
                break;
        }
        return 0.0f;
    }
    float calcObjectZpos(Constant.MapObjects objType, int dist)
    {

        int distFromCurPos = dist - (int)distance_;
        switch (objType)
        {
            case Constant.MapObjects.LHOLE:
			case Constant.MapObjects.SHOP_NORMAL:
			case Constant.MapObjects.SHOP_EXPENSIVE:
			case Constant.MapObjects.SHOP_SANTA:
                return (float)distFromCurPos * 4.0f;
            case Constant.MapObjects.FISH_HOLE:
            case Constant.MapObjects.CRACK:
                return (float)distFromCurPos * 4.0f;
            case Constant.MapObjects.GOAL:
                return (float)distFromCurPos * 4.0f;
			case Constant.MapObjects.ROCK:
				return (float)distFromCurPos * 4.0f;
			case Constant.MapObjects.BOSS:
				return (float)distFromCurPos * 4.0f + 0.3f;
			default:
                break;
        }
        return 0.0f;
    }

	// 
    float calcObjectYPos(Constant.MapObjects objType, int dist)
    {
        int distFromCurPos = dist - (int)distance_;
        switch (objType)
        {
            case Constant.MapObjects.LHOLE:
            case Constant.MapObjects.FISH_HOLE:
            case Constant.MapObjects.CRACK:
			case Constant.MapObjects.SHOP_NORMAL:
			case Constant.MapObjects.SHOP_EXPENSIVE:
			case Constant.MapObjects.SHOP_SANTA:
			case Constant.MapObjects.ROCK:
			//case Constant.MapObjects.BOSS_PIN:
                return (float)distFromCurPos * 0.3f;
			case Constant.MapObjects.GOAL:
			case Constant.MapObjects.BOSS:
				return (float)distFromCurPos * 0.3f + 0.5f;
			default:
                break;
        }
        return 0.0f;
    }

	float calcMonsterXPos(Constant.MapMonsters objType, int hPos, int dist)
	{
		switch (objType)
		{
			case Constant.MapMonsters.BALL:
				return (float)hPos * 1.0f;
			default:
				return (float)hPos * 1.0f;
		}
	}
	float calcMonsterZPos(Constant.MapMonsters objType, int dist)
	{

		float distFromCurPos = (float)dist - distance_;
		switch (objType)
		{
			case Constant.MapMonsters.BALL:
				return distFromCurPos * 4.0f;
			default:
				return distFromCurPos * 4.0f;
		}
	}

	// 
	float calcMonsterYPos(Constant.MapMonsters objType, int dist)
	{
		//float distFromCurPos = (float)dist - distance_;
		//switch (objType)
		//{
		//	case Constant.MapMonsters.BALL:
		//		return distFromCurPos * 0.3f;
		//	default:
		//		break;
		//}
		return 0.0f;
	}

}
