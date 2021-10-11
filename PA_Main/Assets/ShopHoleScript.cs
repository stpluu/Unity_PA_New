using System.Collections;
using System.Collections.Generic;
using UnityEngine;
static partial class Constant
{
	public const float Width_INTO_ShopHole = 1.7f;

}
public class ShopHoleScript : MonoBehaviour {
	public Constant.MapObjects shopType_;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update()
	{

	}
	private void OnEnable()
	{
		//Debug.Log("Crack - Enabled");
		GetComponent<BoxCollider>().enabled = true;
	}
	public void SetShopType(Constant.MapObjects shopType)
	{
		shopType_ = shopType;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			//Debug.Log("Crack: Player col ");
			GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideShop(gameObject, shopType_);
			GetComponent<BoxCollider>().enabled = false;
		}
	}
}
