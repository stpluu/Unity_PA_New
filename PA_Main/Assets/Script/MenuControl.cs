using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour {
	float startTime_;
	bool bStart_;
	// Use this for initialization
	private Sprite[] titleBgSprite_;
	private const int titleBgNum_ = 2;
	private void Awake()
	{
		titleBgSprite_ = new Sprite[titleBgNum_];

		for (int i = 0; i < titleBgNum_; ++i)
		{
			string spriteName = string.Format("Sprites/Title_{0:D2}", i);
			titleBgSprite_[i] = Resources.Load(spriteName, typeof(Sprite)) as Sprite;
		}
	}
	void Start() {
		startTime_ = 0.0f;
		bStart_ = false;
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Space))	
		{
			OnStartKey();
		}
		
		if (bStart_)
		{
			if (Time.time - startTime_ > 1.2f)
			{
				SceneManager.LoadScene("PA_MainGame");
			}
			else
			{
				int tempTime = (int)((Time.time - startTime_) * 10.0f);
				GameObject.Find("TitleBG").GetComponent<SpriteRenderer>().sprite = titleBgSprite_[tempTime % 2];
			}
		}
	}

	public void OnStartKey()
	{
		startTime_ = Time.time;
		bStart_ = true;
	}
}
