using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constant
{
	public enum ItemState
	{
		None = 0,
		Have = 1,
		Used = 2,
	};

	public const int InventoryMaxSize = 7;
}

public class ItemInventoryScript : MonoBehaviour {
	public static ItemInventoryScript instance = null;
	public Constant.ItemState[] itemInventory_; //0 : none, 1: buy, 2: buy and used
	public void SetItemState(Constant.ItemDef itemName, Constant.ItemState state)
	{
		itemInventory_[(int)itemName] = state;
	}
	public Constant.ItemState GetItemState(Constant.ItemDef itemName)
	{
		return itemInventory_[(int)itemName];
	}
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		itemInventory_ = new Constant.ItemState[(int)Constant.ItemDef.TOTALITEMCOUNT];
		for (int i = 0; i < (int)Constant.ItemDef.TOTALITEMCOUNT; ++i)
		{
			itemInventory_[i] = Constant.ItemState.None;
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ResetAllInventory()
	{
		for (int i = 0; i < (int)Constant.ItemDef.TOTALITEMCOUNT; ++i)
		{

		}
		
	}

	public static float GetItemAbility(Constant.ItemDef itemName)
	{
		switch(itemName)
		{
			case Constant.ItemDef.Ring:
				break;
			default:
				return 0.0f;
		}
		return 0.0f;
	}
}
