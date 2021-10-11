using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public partial class Constant
{
	public const uint slotMachineSize = 3;
	public enum SlotMachineImage
	{
		Cherry = 0,
		Bad = 1,
		Konami = 2,
		Egg = 3,
		Grape = 4,
		Penguin = 5,
		MAX,
	};
	public enum SlotMachineState
	{
		sleep = 0,
		start = 1,
		running_3 = 2,
		running_2 = 3,
		running_1 = 4,
		result = 5,
		end = 6,
	};
	public enum SlotMachineRate
	{
		Cherry = 40,
		Bad = 45,
		Konami = 55,
		Egg = 85,
		Grape = 95,
		Penguin = 100,

	};
	public const float resultHoldTime = 2.0f;
	public const float resultForceHoldTime = 1.5f;
	public const int maximumFish = 999;
}
public class SlotMachineScript : MonoBehaviour {
	public Slider fishSlider_;
	public Text fishCountText_;
	public Text fishResultCountText_;
	public Image[] slot_;
	
	private GameManagerScript gameManagerScript_;

	private Sprite[] slotSprite_; 

	private Constant.SlotMachineImage[] currentSlotImageIndex_;
	private Constant.SlotMachineState currentSlotMachineState_;

	private float[] updateTime_;
	private float endTime_;
	private int betFish_;
	private int resultFish_;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (IsSlotMachineRunning())
		{
			
			for (uint i = GetFirstRunningSlot(); i < Constant.slotMachineSize; ++i)
			{
				if (updateTime_[i] + 0.03f < Time.time)
				{
					ChangeSlotImage(i);
					UpdateSlotImage(i);
					updateTime_[i] = Time.time;
				}
			}
			if (currentSlotMachineState_ == Constant.SlotMachineState.result
				&& Time.time - endTime_ > Constant.resultHoldTime)
			{
				SetSlotMachineState(Constant.SlotMachineState.end);
			}
		}
		UpdateUI();
	}
	
	private void Awake()
	{
		gameManagerScript_ = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		currentSlotImageIndex_ = new Constant.SlotMachineImage[Constant.slotMachineSize];
		slot_ = new Image[Constant.slotMachineSize];
		updateTime_ = new float[Constant.slotMachineSize];
		for (uint i = 0; i < Constant.slotMachineSize; ++i)
		{
			string slotName = string.Format("Slot{0}", i.ToString());
			slot_[i] = gameObject.transform.Find(slotName).GetComponent<Image>();
		}
		slotSprite_ = new Sprite[(int)Constant.SlotMachineImage.MAX];
		for (int i = 0; i < (int)Constant.SlotMachineImage.MAX; ++i)
		{
			string spriteName = string.Format("Sprites/SlotMachine/Slot_{0:D2}", i);

			slotSprite_[i] = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
		}
		fishResultCountText_.enabled = false;
		resultFish_ = 0;
	}

	private void OnEnable()
	{
		int currentFish = gameManagerScript_.GetFishCount();
		fishSlider_.maxValue = currentFish;
		for (uint i = 0; i < Constant.slotMachineSize; ++i)
		{ currentSlotImageIndex_[i] = Constant.SlotMachineImage.MAX; }
		currentSlotMachineState_ = Constant.SlotMachineState.sleep;
	}

	public void OnSliderChange()
	{
		fishCountText_.text = fishSlider_.value.ToString();
	}
	public void OnClickSlotMachine()
	{
		switch(currentSlotMachineState_)
		{
			case Constant.SlotMachineState.sleep:
			case Constant.SlotMachineState.end:
				{
					if (fishSlider_.value > 0
						&& fishSlider_.value <= gameManagerScript_.GetFishCount())
					{
						SetSlotMachineState(Constant.SlotMachineState.running_3);
					}
				}
				break;
			case Constant.SlotMachineState.running_3:
				{
					SetSlotMachineState(Constant.SlotMachineState.running_2);
				}
				break;
			case Constant.SlotMachineState.running_2:
				{
					SetSlotMachineState(Constant.SlotMachineState.running_1);
				}
				break;
			case Constant.SlotMachineState.running_1:
				{
					SetSlotMachineState(Constant.SlotMachineState.result);
				}
				break;
			case Constant.SlotMachineState.result:
				{
					if (Time.time - endTime_ > Constant.resultForceHoldTime)
					{
						SetSlotMachineState(Constant.SlotMachineState.end);
					}
				}
				break;
			default:
				break;
		}
		
	}

	public void OnExitButton()
	{
		if (IsSlotMachineRunning())
			return;
		gameObject.SetActive(false);
	}

	private void UpdateUI()
	{
		switch (currentSlotMachineState_)
		{
			case Constant.SlotMachineState.result:
				{
					// TODO : FadeOut4
					float alpha = (Constant.resultHoldTime - (Time.time - endTime_)) / Constant.resultHoldTime;
					fishResultCountText_.color = new Color(fishResultCountText_.color.r
															, fishResultCountText_.color.g
															, fishResultCountText_.color.b
															, alpha);
				}
				break;
			default:
				break;
		}
	}

	private bool IsSlotMachineRunning()
	{
		if (currentSlotMachineState_
			== Constant.SlotMachineState.result
			|| currentSlotMachineState_ == Constant.SlotMachineState.running_3
			|| currentSlotMachineState_ == Constant.SlotMachineState.running_2
			|| currentSlotMachineState_ == Constant.SlotMachineState.running_1
			|| currentSlotMachineState_ == Constant.SlotMachineState.start)
		{
			return true;
		}
		return false;
	}
	private uint GetFirstRunningSlot()
	{

		switch (currentSlotMachineState_)
		{
			case Constant.SlotMachineState.running_3:
				return 0;

			case Constant.SlotMachineState.running_2:
				return 1;

			case Constant.SlotMachineState.running_1:
				return 2;

			default:
				break;
		}
		return Constant.slotMachineSize;
	}
	private void SetSlotMachineState(Constant.SlotMachineState state)
	{
		Constant.SlotMachineState oldState = currentSlotMachineState_;
		currentSlotMachineState_ = state;
		Debug.Log("SlotMachine : " + oldState.ToString() + "->" + currentSlotMachineState_.ToString());
		OnChangeSlotMachineState(oldState);
	}
	private void OnChangeSlotMachineState(Constant.SlotMachineState oldState)
	{
		switch(oldState)
		{
			default:
				break;
		}
		switch(currentSlotMachineState_)
		{
			case Constant.SlotMachineState.sleep:
				{
					for (uint i = 0; i < Constant.slotMachineSize; ++i)
					{
						currentSlotImageIndex_[i] = Constant.SlotMachineImage.MAX;
					}
				}
				break;
			case Constant.SlotMachineState.running_3:
				{
					betFish_ = (int)fishSlider_.value;
					gameManagerScript_.ReduceFish(betFish_);
					//gameManagerScript_
					for (int i = 0; i < Constant.slotMachineSize; ++i)
						updateTime_[i] = Time.time + 0.025f * i;
					//StartCoroutine("SlotMachineRoutine");
				}
				break;
			case Constant.SlotMachineState.result:
				{
					endTime_ = Time.time;

					resultFish_ = CalculateResult();
					fishResultCountText_.text = string.Format("{0:D3}", resultFish_);
					fishResultCountText_.enabled = true;
				}
				break;
			case Constant.SlotMachineState.end:
				{
					ApplyResultFishCount();
					//StopCoroutine("SlotMachineRoutine");
					fishResultCountText_.enabled = false;
				}
				break;
			default:
				break;
		}
	}

	private void UpdateSlotImage(uint slotNum)
	{
		if (slotNum < Constant.slotMachineSize)
		{
			if (currentSlotImageIndex_[slotNum] == Constant.SlotMachineImage.MAX)
				slot_[slotNum].enabled = false;
			else
			{
				slot_[slotNum].sprite = slotSprite_[(int)currentSlotImageIndex_[slotNum]];
				slot_[slotNum].enabled = true;
			}
			
		}
			
	}

	private void ChangeSlotImage(uint slotNum)
	{
		int randomCount = Random.Range(0, 100);

		if (randomCount < (int)Constant.SlotMachineRate.Cherry)
		{
			currentSlotImageIndex_[slotNum] = Constant.SlotMachineImage.Cherry;
		}
		else if (randomCount < (int)Constant.SlotMachineRate.Bad)
		{
			currentSlotImageIndex_[slotNum] = Constant.SlotMachineImage.Bad;
		}
		else if (randomCount < (int)Constant.SlotMachineRate.Konami)
		{
			currentSlotImageIndex_[slotNum] = Constant.SlotMachineImage.Konami;
		}
		else if (randomCount < (int)Constant.SlotMachineRate.Egg)
		{
			currentSlotImageIndex_[slotNum] = Constant.SlotMachineImage.Egg;
		}
		else if (randomCount < (int)Constant.SlotMachineRate.Grape)
		{
			currentSlotImageIndex_[slotNum] = Constant.SlotMachineImage.Grape;
		}
		else if (randomCount < (int)Constant.SlotMachineRate.Penguin)
		{
			currentSlotImageIndex_[slotNum] = Constant.SlotMachineImage.Penguin;
		}
	}
	
	private int CalculateResult()
	{
		int fishRate = 0;
		int cherryCount = 0;
		if (currentSlotImageIndex_[0] == currentSlotImageIndex_[1]
			&& currentSlotImageIndex_[0] == currentSlotImageIndex_[2])
		{
			switch(currentSlotImageIndex_[0])
			{
				case Constant.SlotMachineImage.Cherry:
					fishRate = 4;
					break;
				case Constant.SlotMachineImage.Egg:
					fishRate = 8;
					break;
				case Constant.SlotMachineImage.Grape:
					fishRate = 10;
					break;
				case Constant.SlotMachineImage.Konami:
					fishRate = 15;
					break;
				case Constant.SlotMachineImage.Penguin:
					fishRate = 20;
					break;
				case Constant.SlotMachineImage.Bad:
					fishRate = 0;
					break;
				default:
					break;
			}
		}
		else
		{
			for (uint i = 0; i < Constant.slotMachineSize; ++i)
			{
				if (currentSlotImageIndex_[i] == Constant.SlotMachineImage.Cherry)
				{
					cherryCount++;
				}
			}
			if (cherryCount == 1)
			{ fishRate = 1; }
			else if (cherryCount == 2)
			{ fishRate = 2; }
		}
		//betFish_ *= fishRate;
		//if (betFish_ > 0)
		//{
		//	gameManagerScript_.AddFish(betFish_);
		//}
		if (betFish_ * fishRate > Constant.maximumFish)
		{
			return Constant.maximumFish;
		}
		return betFish_ * fishRate;
	}
	private void ApplyResultFishCount()
	{
		if (resultFish_ > 0)
		{
			gameManagerScript_.AddFish(resultFish_);
		}
	}
}
