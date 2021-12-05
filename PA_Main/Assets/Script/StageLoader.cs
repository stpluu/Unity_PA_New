using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class StageLoader : MonoBehaviour {
	private WorldScript worldScript_;
	string loadingFilePath_;
	int parcingLineNum_;
	public enum GameMode
	{
		orignal,
		hard,
		custom,

	}
	private enum TagType
	{
		stage,
		objects,
		monsters,
		shop,
		NONE,
	}
	
	private void Awake()
	{
		worldScript_ = gameObject.GetComponent<WorldScript>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public bool LoadStage(GameMode stageStyle, int stageNum)
	{
		string stageFolder;
		switch (stageStyle)
		{
			case GameMode.orignal:
				stageFolder = "Assets/Resources/StageData/OriginalStages/";
				break;
			case GameMode.hard:
				stageFolder = "Assets/Resources/StageData/HardStages/";
				break;
			case GameMode.custom:
				stageFolder = "Assets/Resources/StageData/CustomStages/";
				break;
			default:
				stageFolder = "Assets/Resources/StageData/OriginalStages/";
				break;
		}
		loadingFilePath_ = stageFolder + string.Format("{0:D2}.txt", stageNum);
		if (File.Exists(loadingFilePath_) == false)
			return false;
		using (StreamReader sr = new StreamReader(loadingFilePath_))
		{
			parcingLineNum_ = 0;
			string currentLine;
			TagType parcingTag = TagType.NONE;
			while (string.IsNullOrEmpty(currentLine = sr.ReadLine()) == false)
			{
				currentLine.Trim();
				parcingLineNum_++;
				if (IsComment(currentLine))
				{
					continue;
				}
				if (IsCloseTag(currentLine))
				{
					if (parcingTag != TagType.NONE)
						parcingTag = TagType.NONE;
					else
					{
						ParseError(currentLine, "unexcpeted close tag");
					}
					continue;
				}
				switch (parcingTag)
				{
					case TagType.monsters:
						ProcessMonsterLine(currentLine);
						break;
					case TagType.objects:
						ProcessObjectLine(currentLine);
						break;
					case TagType.stage:
						ProcessStageLine(currentLine);
						break;
					case TagType.shop:
						ProcessShopLine(currentLine);
						break;
					case TagType.NONE:
					default:
						{
							//ParseError(currentLine, "unknown open tag");
						}
						break;
				}

				if (IsOpenTag(currentLine))
				{
					if (parcingTag != TagType.NONE)
					{
						ParseError(currentLine, "unexcepted open tag");
					}
					parcingTag = ParseTag(currentLine);
					if (parcingTag == TagType.NONE)
						ParseError(currentLine, "unknown open tag");
					continue;
				}
			}
		}

		return false;
	}
	
	private bool IsComment(string data)
	{
		if (data.StartsWith("//"))
			return true;
		return false;
	}

	private bool IsOpenTag(string data)
	{
		if (data.StartsWith("<"))
		{
			if (IsCloseTag(data) == false)
				return true;
		}
		return false;
	}
	private bool IsCloseTag(string data)
	{
		if (data.StartsWith("</"))
		{
			return true;
		}
		return false;
	}
	private TagType ParseTag(string data)
	{
		if (data.Equals("<stage>"))
		{
			return TagType.stage;
		}
		if (data.Equals("<objects>"))
		{
			return TagType.objects;
		}
		if (data.Equals("<monsters>"))
		{
			return TagType.monsters;
		}
		if (data.Equals("<shop>"))
		{
			return TagType.shop;
		}
		return TagType.NONE;
	}
	private bool ProcessShopLine(string data)
	{
		string[] oneData = data.Split(new char[] { ','});
		
		{
			int itemID = 0;
			int goodShopPrice = 0;
			if (int.TryParse(oneData[0], out itemID) == false)
			{
				ParseError("shop ", " itemID Error");
			}
			if (int.TryParse(oneData[1], out goodShopPrice) == false)
			{
				ParseError("shop ", " item price Error");
			}
			worldScript_.addShopItem(itemID, goodShopPrice);
		}
		return true;
	}

	private bool ProcessObjectLine(string data)
	{
		string[] oneData = data.Split(new char[] { ',' });
		int distance = 0;
		int hPos = 0;
		if (int.TryParse(oneData[0], out distance) == false)
		{
			ParseError("object_distance error", "> 0 && <= stage distance");
		}
		if (int.TryParse(oneData[2], out hPos) == false)
		{
			ParseError("object_horizonal postion error", "must be integer");
		}
		worldScript_.addObject(distance, oneData[1], hPos);
		return true;
	}
	private bool ProcessMonsterLine(string data)
	{
		string[] oneData = data.Split(new char[] { ',' });
		int distance = 0;
		int hPos = 0;
		if (int.TryParse(oneData[0], out distance) == false)
		{
			ParseError("monster_distance error", "> 0 && <= stage distance");
		}
		if (int.TryParse(oneData[2], out hPos) == false)
		{
			ParseError("monster_horizonal postion error", "must be integer");
		}
		worldScript_.addMonster(distance, oneData[1], hPos);
		return true;
	}
	private bool ProcessStageLine(string data)
	{
		string[] oneData = data.Split(new char[] {'='});
		if (oneData[0].Equals("tile"))
		{
			if (worldScript_.SetStageStyle(oneData[1]) == false)
				ParseError(data, "unknown tile type");
		}
		if (oneData[0].Equals("time"))
		{
			int stageTime = 0;
			if (int.TryParse(oneData[1], out stageTime))
			{
				worldScript_.stageMaxTime_ = stageTime;
			}
		}
		if (oneData[0].Equals("distance"))
		{
			int stageDistance = 0;
			if (int.TryParse(oneData[1], out stageDistance))
			{
				worldScript_.stageMaxDistance_ = stageDistance;
			}
		}
		if (oneData[0].Equals("boss"))
		{
			int bossLevel = 0;
			if (int.TryParse(oneData[1], out bossLevel))
			{
				worldScript_.isBossStage_ = true;
				//bossLevel todo
			}
		}
		return true;

	}
	private void GetObjectTag(string data)
	{
		//MapObjec
		//if (data.Equals)
		return;
	}
	private void ParseError(string data, string errorStr)
	{
		Debug.Log("Load Stage Error : " + data);
		Debug.Log("Error : " + errorStr);
		Debug.Log("file name : " + loadingFilePath_ + ", line : " + parcingLineNum_.ToString());
	}
}
