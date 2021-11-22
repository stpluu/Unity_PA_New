using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class Constant
{
	public enum ItemDef
	{
		Ring = 0,
		YellowRing,
		Pendant,
		Feather,
		Gun,
		FlyingCap,
		GreenShoes,
		PlateMail,
		Secret2,
		RedShoes,
		Secret1,
		Helm,
		LetherMail,
		Bell,
		Glasses,
		Torch,
		Map,
		Robe,
		BlueBoots,
		TOTALITEMCOUNT,
	};
	public static readonly int[] itemPrice_ =
	{
	18,20,22,23,10,15,19,24,99,99,99,12,24,18,12,16,23,24,99,99
	};
};
public class ShopScript : MonoBehaviour {
	Constant.MapObjects shopType_;
	public int shopStage_;
	GameObject shopUI_;
	GameObject currentClickedItem_;
	GameObject selectRect_;
	GameManagerScript gameManagerScript_;
	WorldScript worldScript_;
	//해당 스테이지에서 파는 아이템

	private void Awake()
	{
		shopUI_ = GameObject.Find("ShopUI");
		gameManagerScript_ = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
		worldScript_ = gameObject.GetComponent<WorldScript>();
		selectRect_ = shopUI_.transform.Find("SelectRect").gameObject;
	}
	// Use this for initialization
	void Start () {
		

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetShopType(Constant.MapObjects shopType)
	{
		shopType_ = shopType;
		switch (shopType_)
		{
			case Constant.MapObjects.SHOP_NORMAL:
				//gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load()
				break;
			case Constant.MapObjects.SHOP_EXPENSIVE:
				break;
			case Constant.MapObjects.SHOP_SANTA:
				break;
			default:
				break;
		}
	}

	public void SetShopStage(int stage)
	{

		shopStage_ = stage;
		// temp..
		if (shopStage_ > 2)
			shopStage_ = 2;
	}
	
	private void OnEnable()
	{
		currentClickedItem_ = null;
		selectRect_.SetActive(false);
//		for (int i = 0; i < (int)Constant.ItemDef.TOTALITEMCOUNT; ++i)
//		{
//			
//			{
//				//if (stageSellItem_[shopStage_, i] == 0
//				//	|| gameManagerScript_.isBoughtItem((Constant.ItemDef)i) == true)
//				{
//					ChangeItemBtnState(i, false);
//				}
//				else
//				{
//					ChangeItemBtnState(i, true);
//					
//					
//				}
//			}
//				
//		}
		for (int i = 0; i < (int)Constant.ItemDef.TOTALITEMCOUNT; ++i)
		{
			
			if (worldScript_.sellItemList_.TryGetValue(i, out int price))
			{
				ChangeItemBtnState(i, gameManagerScript_.isBoughtItem((Constant.ItemDef)i) == false);
			}
			else
				ChangeItemBtnState(i, false);
		}
		shopUI_.transform.Find("SlotMachineUI").gameObject.SetActive(false);
	}

	//아이템 버튼 클릭시 처리 : 첫번째 클릭-선택, 두번째 클릭 - 구매
	public void OnClickItemBtn(int itemNumber)
	{
		string buttonName = getItemButtonName(itemNumber);
		GameObject itemBtn = shopUI_.transform.Find(buttonName).gameObject;
		if (itemBtn != null)
		{
			if (currentClickedItem_ == itemBtn)
			{
				Constant.ItemDef item = (Constant.ItemDef)itemNumber;
				if (gameManagerScript_.BuyItem(item, calcItemPrice(item)) == true)
				{
					ChangeItemBtnState(itemNumber, false);
					currentClickedItem_ = null;
					selectRect_.SetActive(false);
				}
			}
			else
			{
				Debug.Log("Select : " + buttonName + itemBtn.GetComponent<RectTransform>().localPosition);
				selectRect_.SetActive(true);
				currentClickedItem_ = itemBtn;
				selectRect_.GetComponent<RectTransform>().localPosition
					 = currentClickedItem_.GetComponent<RectTransform>().localPosition;	
				//selectRect_.GetComponent<RectTransform>().
				//currentClickedItem_.transform.localScale.Scale(new Vector3(1.1f, 1.1f, 1.0f));
			}
		}
	}

	// 각 아이템 버튼의 active 상태 변경, 가격 텍스트도 세팅
	private void ChangeItemBtnState(int itemNum, bool bEnable)
	{
		if (itemNum > (int)Constant.ItemDef.TOTALITEMCOUNT)
			return;
		string ItemControlName = getItemButtonName(itemNum);
		GameObject itemBtn = shopUI_.transform.Find(ItemControlName).gameObject;
		if (itemBtn != null)
		{
			itemBtn.SetActive(bEnable);
			if (bEnable)
			{
				int itemPrice = calcItemPrice((Constant.ItemDef)itemNum);
				itemBtn.transform.Find("Price").GetComponent<Text>().text = itemPrice.ToString();
			}
		}
	}

	private string getItemButtonName(int itemNumber)
	{
		return string.Format("Item_{0}", itemNumber);
	}

	public int calcItemPrice(Constant.ItemDef item)
	{
		if (worldScript_.sellItemList_.TryGetValue((int)item, out int price))
		{
			switch (shopType_)
			{
			case Constant.MapObjects.SHOP_NORMAL:
				return price;
			case Constant.MapObjects.SHOP_EXPENSIVE:
				return price * 2;
			case Constant.MapObjects.SHOP_SANTA:
				return 0;	
			}	
		}
		
		return -1;
	}
	public void SetShopUIVisible(bool bVisible)
	{
		shopUI_.SetActive(bVisible);
	}

	public void OnClickExitShop()
	{
		SetShopUIVisible(false);
		gameManagerScript_.OnExitShop();
	}

	public void OnClickSlotMachine()
	{
		if (gameManagerScript_.GetFishCount() > 0)
		{
			// open slow machine UI
			shopUI_.transform.Find("SlotMachineUI").gameObject.SetActive(true);
		}
		else
		{
		}
	}

}
