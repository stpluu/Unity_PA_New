using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManagerScript : MonoBehaviour {

    private PlayerScript playerScript_;
    private WorldScript worldScript_;
	private StageLoader stageLoader_;
	private ShopScript shopScript_;

	public GameObject fish_;
	public GameObject bullet_;
	public GameObject heart_;
	public GameObject large_hole_;
	public GameObject fish_hole_;
	public GameObject crack_;
	public GameObject rock_;
	public GameObject shop_;
	public GameObject tree_;
	public GameObject warp_;
	public GameObject wing_;
	public GameObject goal_;
	public GameObject monsterBall_;
	public GameObject monsterBat_;
	public GameObject monsterCloud_;
	public GameObject monsterSquid_;

	// Item / monster / bullet pool
    public GameObject[] fishPool_;
	public GameObject[] bulletPool_;
	public GameObject[] heartPool_;

	// MapObject Pool
	public GameObject[] largeHolePool_;
	public GameObject[] fishHolePool_;
	public GameObject[] crackPool_;
	public GameObject[] rockPool_;
	public GameObject[] shopPool_;
	public GameObject[] monsterBallPool_;
	public GameObject goalObject_;

	private const int FISH_INSCREEN_MAX_NUM = 10;
	private const int HEART_INSCREEN_MAX_NUM = 4;
	private const int LARGE_HOLE_INSCREEN_MAX_NUM = 7;
	private const int CRACK_INSCREEN_MAX_NUM = 7;
	private const int FISH_HOLE_INSCREEN_MAX_NUM = 7;
	private const int ROCK_INSCREEN_MAX_NUM = 7;
	private const int SHOP_INSCREEN_MAX_NUM = 3;
	private const int MONSTER_INSCREEN_MAX_NUM = 5;

	private const int BULLET_INSCREEN_MAX_NUM = 3;

	[SerializeField] private int fishCount_;	//player 가 소지한 fish 수

    private Text TimeText_;
    private Text FishText_;
    private Text DistanceText_;
    private Text SpeedText_;

	private Text DebugText_;
    float stageStartTime_;
	float pauseTime_;
	float stageFinishTime_;
	bool bInGoal_;
	bool bInShop_;
	bool bPause_;
	bool bDie_;
	bool bShowingStageStartMap_;
	private float mapShowingTime_;

	public static int currentStage_;
	public static int totalStage_;

	public static void setCurrentStage(int stage)
	{
		currentStage_ = stage;
	}
	public static int getCurrentStage()
	{
		return currentStage_;
	}
	public static void setTotalStage(int stage)
	{
		totalStage_ = stage;
	}
	public static int getTotalStage()
	{
		return totalStage_;
	}
	//public int[] itemInventory_;    //0 : none, 1: buy, 2: buy and used
	private ItemInventoryScript itemInven_;

	public ItemInventoryScript ItemInven_
	{
		get
		{
			return itemInven_;
		}

		set
		{
			itemInven_ = value;
		}
	}

	private void Awake()
    {
        playerScript_ = GameObject.Find("Player").GetComponent<PlayerScript>();
        worldScript_ = gameObject.GetComponent<WorldScript>();
		stageLoader_ = gameObject.GetComponent<StageLoader>();
		shopScript_ = GameObject.Find("ShopUI").GetComponent<ShopScript>();
		//ItemInven_ = gameObject.GetComponent<ItemInventoryScript>();

		TimeText_ = GameObject.Find("UI").transform.Find("Time").GetComponent<Text>();
        FishText_ = GameObject.Find("UI").transform.Find("Fish").GetComponent<Text>();
        DistanceText_ = GameObject.Find("UI").transform.Find("Distance").GetComponent<Text>();
        SpeedText_ = GameObject.Find("UI").transform.Find("Speed").GetComponent<Text>();

        // create object instanace
        fishPool_ = new GameObject[FISH_INSCREEN_MAX_NUM];
		bulletPool_ = new GameObject[BULLET_INSCREEN_MAX_NUM];
		heartPool_ = new GameObject[HEART_INSCREEN_MAX_NUM];

		largeHolePool_ = new GameObject[LARGE_HOLE_INSCREEN_MAX_NUM];
		fishHolePool_ = new GameObject[FISH_HOLE_INSCREEN_MAX_NUM];
		crackPool_ = new GameObject[CRACK_INSCREEN_MAX_NUM];
		rockPool_ = new GameObject[ROCK_INSCREEN_MAX_NUM];
		shopPool_ = new GameObject[SHOP_INSCREEN_MAX_NUM];

		monsterBallPool_ = new GameObject[MONSTER_INSCREEN_MAX_NUM];

		DebugText_ = GameObject.Find("UI").transform.Find("Debug").GetComponent<Text>();

		//itemInventory_ = new int[(int)Constant.ItemDef.TOTALITEMCOUNT];
		//for (int i = 0; i < (int)Constant.ItemDef.TOTALITEMCOUNT; ++i)
		//{//
		//	itemInventory_[i] = 0;
		//}

		GameObject menu = GameObject.Find("UI").transform.Find("PauseMenu").gameObject;
		menu.SetActive(false);
		GameObject MapObj = GameObject.Find("UI").transform.Find("Map").gameObject;
		//MapScript mapScript = MapObj.GetComponent<MapScript>();
		//mapScript.LoadMapData(StageLoader.GameMode.orignal);
		MapObj.SetActive(false);
		mapShowingTime_ = 0.0f;
		bShowingStageStartMap_ = false;
		stageStartTime_ = 0.0f;
	}

	// Use this for initialization
	void Start() {
		ShowMap(false);
		//stageStartTime_ = Time.time;
		fishCount_ = 20;
		//instantiate
		goalObject_ = Instantiate(goal_, Vector3.zero, Quaternion.identity) as GameObject;
		goalObject_.SetActive(false);
		for (int i = 0; i < FISH_INSCREEN_MAX_NUM; ++i)
		{
			fishPool_[i] = Instantiate(fish_, Vector3.zero, Quaternion.identity) as GameObject;
			fishPool_[i].SetActive(false);
		}

		for (int i = 0; i < BULLET_INSCREEN_MAX_NUM; ++i)
		{
			bulletPool_[i] = Instantiate(bullet_, Vector3.zero, Quaternion.identity) as GameObject;
			bulletPool_[i].SetActive(false);
		}

		for (int i = 0; i < HEART_INSCREEN_MAX_NUM; ++i)
		{
			heartPool_[i] = Instantiate(heart_, Vector3.zero, Quaternion.identity) as GameObject;
			heartPool_[i].SetActive(false);
		}
		for (int i = 0; i < LARGE_HOLE_INSCREEN_MAX_NUM; ++i)
		{
			largeHolePool_[i] = Instantiate(large_hole_, Vector3.zero, Quaternion.identity) as GameObject;
			largeHolePool_[i].SetActive(false);
		}

		for (int i = 0; i < FISH_HOLE_INSCREEN_MAX_NUM; ++i)
		{
			fishHolePool_[i] = Instantiate(fish_hole_, Vector3.zero, Quaternion.identity) as GameObject;
			fishHolePool_[i].SetActive(false);
		}

		for (int i = 0; i < CRACK_INSCREEN_MAX_NUM; ++i)
		{
			crackPool_[i] = Instantiate(crack_, Vector3.zero, Quaternion.identity) as GameObject;
			crackPool_[i].SetActive(false);
		}

		for (int i = 0; i < ROCK_INSCREEN_MAX_NUM; ++i)
		{
			rockPool_[i] = Instantiate(rock_, Vector3.zero, Quaternion.identity) as GameObject;
			rockPool_[i].SetActive(false);
		}

		for (int i = 0; i < SHOP_INSCREEN_MAX_NUM; ++i)
		{
			shopPool_[i] = Instantiate(shop_, Vector3.zero, Quaternion.identity) as GameObject;
			shopPool_[i].SetActive(false);
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////
		// Monster
		for (int i = 0; i < MONSTER_INSCREEN_MAX_NUM; ++i)
		{
			monsterBallPool_[i] = Instantiate(monsterBall_, Vector3.zero, Quaternion.identity) as GameObject;
			monsterBallPool_[i].SetActive(false);
		}
		bInGoal_ = false;

		/// for debug
		if (getCurrentStage() < 1
			|| getCurrentStage() > 2)
			setCurrentStage(1);
		/// 

		stageLoader_.LoadStage(StageLoader.GameMode.orignal, getCurrentStage());
		shopScript_.SetShopStage(getCurrentStage());
		shopScript_.SetShopUIVisible(false);
		pauseTime_ = 0.0f;
		bInShop_ = false;
		bPause_ = false;
		bDie_ = false;
		refreshInventoryUI();
	}
	
	// Update is called once per frame
	void Update () {
        //float speed = playerScript_.speed_;
        worldScript_.updateDistance(playerScript_.speed_);
		if (bInGoal_)
			UpdateUIInGoal();
        else
			UpdateUI();
		if (isTimePauseState())
			pauseTime_ += Time.deltaTime;
		if (bShowingStageStartMap_ == true)
		{
			ProcShowingMap();
		}
	}
   
    void UpdateUI()
    {

	   if (playerScript_.speed_ > Constant.Speed_Max_Lv1)
		{
			SpeedText_.color = new Color(255, 50, 50);
		}
	   else
		{
			SpeedText_.color = new Color(255, 255, 255);
		}
        SpeedText_.text = playerScript_.speed_.ToString();

        //time text
        int remainTime = worldScript_.stageMaxTime_ * 10 - (int)((Time.time - stageStartTime_ + pauseTime_) * 10.0f);
        TimeText_.text = remainTime.ToString();

        //distanceText
        int distance = worldScript_.stageMaxDistance_ - (int)worldScript_.distance_;
        DistanceText_.text = distance.ToString();

        //fish text
        FishText_.text = fishCount_.ToString();

		//DebugText
		DebugText_.text = GameObject.Find("Player").GetComponent<PlayerScript>().characterState_.ToString();
		DebugText_.enabled = false;
	}
	void UpdateUIInGoal()
	{

	}
    public void OnCollideFish(GameObject fishObj)
    {
        fishCount_++;
		Debug.Log("Fish Collide : " + fishCount_.ToString());
		fishObj.SetActive(false);
		fishObj.transform.position = Vector3.zero;
    }
	public void OnCollideBullet(GameObject obj)
	{
		obj.SetActive(false);
		obj.transform.position = Vector3.zero;
	}
	public void OnCollideHeart(GameObject heartObj, Constant.HeartColor heartColor)
	{
		heartObj.SetActive(false);
		heartObj.transform.position = Vector3.zero;
		switch(heartColor)
		{
			case Constant.HeartColor.Purple:
				AddFish(2);
				break;
			case Constant.HeartColor.Green:
				AddFish(5);
				break;
			case Constant.HeartColor.Sky:
				AddFish(10);
				break;
			case Constant.HeartColor.Yellow:
				AddFish(20);
				break;
			default:
				break;
		}
	}
	public void ChangeHeartColor()
	{
		foreach (GameObject obj in heartPool_)
		{
			if (obj.activeSelf == true)
			{
				HeartScript script = obj.GetComponent<HeartScript>();
				if(script != null)
				{ script.ChangeToNextColor(); }
			}
		}
	}
	public void OnEnterShop(GameObject shopObj, Constant.MapObjects shopType)
	{
		shopScript_.SetShopType(shopType);
		//GameObject.Find("ShopUI").SetActive(true);
		shopScript_.SetShopUIVisible(true);
		playerScript_.SetSpeed(0);
	}
	public void OnExitShop()
	{
		shopScript_.SetShopUIVisible(false);
		playerScript_.OnExitFromHole();

	}
    public GameObject GetMapObjectInstance(Constant.MapObjects objectType)
    {
		switch (objectType)
		{
			case Constant.MapObjects.CRACK:
				foreach (GameObject obj in crackPool_)
				{
					if (obj.activeSelf == false)
					{
						return obj;
					}
				}
				break;
			case Constant.MapObjects.FISH_HOLE:
				foreach (GameObject obj in fishHolePool_)
				{
					if (obj.activeSelf == false)
					{
						return obj;
					}
				}
				break;
			case Constant.MapObjects.LHOLE:
				foreach (GameObject obj in largeHolePool_)
				{
					if (obj.activeSelf == false)
					{
						return obj;
					}
				}
				break;
			case Constant.MapObjects.ROCK:
				foreach (GameObject obj in rockPool_)
				{
					if (obj.activeSelf == false)
					{
						return obj;
					}
				}
				break;
			case Constant.MapObjects.SHOP_EXPENSIVE:
			case Constant.MapObjects.SHOP_NORMAL:
			case Constant.MapObjects.SHOP_SANTA:
				foreach (GameObject obj in shopPool_)
				{
					if (obj.activeSelf == false)
					{
						return obj;
					}
				}
				break;
			case Constant.MapObjects.GOAL:
				{
					return goalObject_;
				}
			case Constant.MapObjects.HEART:
				{
					return GetHeartInstance();
				}
			default:
				// not yet ... return crack object
				foreach (GameObject obj in crackPool_)
				{
					if (obj.activeSelf == false)
					{
						return obj;
					}
				}
				break;
		}
        return null;
    }

	public GameObject GetMonsterObjectInstance(Constant.MapMonsters monsterType)
	{
		switch (monsterType)
		{
			case Constant.MapMonsters.BALL:
				foreach (GameObject obj in monsterBallPool_)
						{
					if (obj.activeSelf == false)
					{
						return obj;
					}
				}
				break;
			case Constant.MapMonsters.BAT:
				break;
			case Constant.MapMonsters.BOSS:
				break;
			default:
				break;
		}
		return null;
	}
	public GameObject GetFishInstance()
	{
		foreach (GameObject obj in fishPool_)
		{
			if (obj.activeSelf == false)
			{
				//Debug.Log("Free Fish : " + i.ToString());
				return obj;
			}
		}
		return null;
	}
	public GameObject GetBulletInstance()
	{
		foreach (GameObject obj in bulletPool_)
		{
			if (obj.activeSelf == false)
			{
				return obj;
			}
		}
		return null;
	}
	public GameObject GetHeartInstance()
	{
		foreach (GameObject obj in heartPool_)
		{
			if (obj.activeSelf == false)
			{
				return obj;
			}
		}
		return null;
	}
	public void OnGoal()
	{
		stageFinishTime_ = Time.time;
		//GameObject.FindWithTag("Player").GetComponent<Animator>().SetTrigger("trGoal");
		playerScript_.OnGoal();
		//playerScript_.SetSpeed(0);
		Debug.Log("FinishTime : " + stageFinishTime_.ToString());
		bInGoal_ = true;
	}

	public void OnPauseButton()
	{
		GameObject menu = GameObject.Find("UI").transform.Find("PauseMenu").gameObject;
		if (menu.activeSelf == false)
		{
			Time.timeScale = 0;
			menu.SetActive(true);
			ShowMap(true);
			
		}
		else
		{
			OnReturnButton();
		}
		
	}

	public void OnReturnButton()
	{
		GameObject menu = GameObject.Find("UI").transform.Find("PauseMenu").gameObject;
		Time.timeScale = 1.0f;
		menu.SetActive(false);
		HideMap();
	}

	public void OnRestratButton()
	{
		//Time.timeScale = 1.0f;
		HideMap();
		SceneManager.LoadScene("PA_MainGame");
	}
	public void OnExitButton()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene("TitleScreen");
	}
	public void OnFishUpButtonGM()
	{
		fishCount_ += 10;
	}
	public bool BuyItem(Constant.ItemDef item, int price)
	{
		if (fishCount_ >= price
			//&& itemInventory_[(int)item] == 0)
			//&& ItemInven_.GetItemState(item) == Constant.ItemState.None)
			&& ItemInventoryScript.instance.GetItemState(item) == Constant.ItemState.None)
		{
			//fishCount_ -= price;
			ReduceFish(price);
			//itemInventory_[(int)item] = 1;
			//ItemInven_.SetItemState(item, Constant.ItemState.Have);
			ItemInventoryScript.instance.SetItemState(item, Constant.ItemState.Have);
			string inventoryName = "Inventory_" + ((int)item).ToString();
			GameObject itemIcon = GameObject.Find("UI").transform.Find(inventoryName).gameObject;
			itemIcon.SetActive(true);
			return true;
		}
		return false;
	}

	public bool isBoughtItem(Constant.ItemDef item)
	{
		//if (itemInventory_[(int)item] != 0)
		//if (ItemInven_.GetItemState(item) != Constant.ItemState.None)
		if (ItemInventoryScript.instance.GetItemState(item) != Constant.ItemState.None)
			return true;
		return false;
	}

	public bool hasItem(Constant.ItemDef item)
	{
		//if (itemInventory_[(int)item] == 1)
		//if (ItemInven_.GetItemState(item) == Constant.ItemState.Have)
		if (ItemInventoryScript.instance.GetItemState(item) == Constant.ItemState.Have)
			return true;
		return false;
	}

	public bool isTimePauseState()
	{
		if (bInGoal_)
			return true;
		if (bInShop_)
			return true;
		if (bDie_)
			return true;
		if (bPause_)
			return true;
		if (bShowingStageStartMap_)
			return true;
		return false;
	}
	public void onDie(int dieReason)
	{
		bDie_ = true;
	}
	public void loadStage(int level)
	{
		setCurrentStage(level);
		SceneManager.LoadScene("PA_MainGame");
		//Time.timeScale = 1.0f;
	}
	public void goNextStage()
	{
		if (getCurrentStage() >= 2)
		{
			SceneManager.LoadScene("TitleScreen");
		}
		else
		{
			loadStage(getCurrentStage() + 1);
			
		}
		
	}
	public void ShowMap(bool bFrompause)
	{
		GameObject MapObj = GameObject.Find("UI").transform.Find("Map").gameObject;
		//MapScript mapScript = MapObj.GetComponent<MapScript>();
		//mapScript.LoadMapData(StageLoader.GameMode.orignal);
		MapObj.SetActive(true);
		MapObj.transform.Find("MapBackground").gameObject.SetActive(!bFrompause);
		if (bFrompause == false)
		{
			mapShowingTime_ = Time.unscaledTime;
			bShowingStageStartMap_ = true;
		}
	}
	public void HideMap()
	{
		GameObject MapObj = GameObject.Find("UI").transform.Find("Map").gameObject;
		//MapScript mapScript = MapObj.GetComponent<MapScript>();
		//mapScript.LoadMapData(StageLoader.GameMode.orignal);
		MapObj.SetActive(false);
	}
	public void ProcShowingMap()
	{
		if (Time.unscaledTime - mapShowingTime_ > 6.0f)
		{
			mapShowingTime_ = 0.0f;
			bShowingStageStartMap_ = false;
			HideMap();
			stageStartTime_ = Time.time;
			Time.timeScale = 1.0f;
		}
	}

	public void refreshInventoryUI()
	{
		for (int i = 0; i < (int)Constant.InventoryMaxSize; ++i)
		{
			string inventoryName = "Inventory_" + i.ToString();
			GameObject itemIcon = GameObject.Find("UI").transform.Find(inventoryName).gameObject;
			if (hasItem((Constant.ItemDef)i))
			{
				
				itemIcon.SetActive(true);
			}
			else
			{
				itemIcon.SetActive(false);
			}
			
		}
		
		
	}

	public int GetFishCount()
	{
		return fishCount_;
	}
	public bool ReduceFish(int count)
	{
		if (count > fishCount_)
			return false;
		fishCount_ = fishCount_ - count;
		return true;
	}
	public void AddFish(int count)
	{
		fishCount_ += count;
		if (fishCount_ > Constant.maximumFish)
			fishCount_ = Constant.maximumFish;
	}
}
