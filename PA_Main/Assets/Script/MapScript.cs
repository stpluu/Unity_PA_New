using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public partial class Constant
{
	public enum MapOpenType
	{
		Closed = 0,
		NextStage = 1,
		Pause = 2,
		Etc = 3,
	}
}
public class MapScript : MonoBehaviour {
	//private GameObject mapObject_;

	string loadingFilePath_;
	int parcingLineNum_;

	private enum TagType
	{
		mode_info,
		map_pos,
		NONE,
	}

	[System.Serializable]
	struct stageCharacterPosition
	{
		public int x;
		public int y;
		public int direction;
	}
	private Dictionary<int, stageCharacterPosition> mapPosDic_;
	private void Awake()
	{
		//mapObject_ = GameObject.Find("UI").transform.Find("Map") as GameObject;
		//mapObject_.transform.FindChild("MapImage").GetComponent<Image>().overrideSprite
		mapPosDic_ = new Dictionary<int, stageCharacterPosition>();
		LoadMapData(StageLoader.GameMode.orignal);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnEnable()
	{
		int currentStage = GameManagerScript.getCurrentStage();
		if (currentStage > 0)
		{
			gameObject.transform.Find("MapImage").transform.Find("MapCharacter").transform.position = new Vector3(mapPosDic_[currentStage].x, mapPosDic_[currentStage].y, 0.0f);
			if (mapPosDic_[currentStage].direction == -1)
			{

			}
		}

	}
	
	private void OnDisable()
	{
		
	}
	
	public bool LoadMapData(StageLoader.GameMode gameMode)
	{
		string stageFolder;
		switch (gameMode)
		{
			case StageLoader.GameMode.orignal:
				stageFolder = "Assets/Resources/StageData/OriginalStages/";
				break;
			case StageLoader.GameMode.hard:
				stageFolder = "Assets/Resources/StageData/HardStages/";
				break;
			case StageLoader.GameMode.custom:
				stageFolder = "Assets/Resources/StageData/CustomStages/";
				break;
			default:
				stageFolder = "Assets/Resources/StageData/OriginalStages/";
				break;
		}
		loadingFilePath_ = stageFolder + "ModeData.txt";
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
					case TagType.mode_info:
						ProcessModeInfoLine(currentLine);
						break;
					case TagType.map_pos:
						ProcessMapPosLine(currentLine);
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
		if (data.Equals("<mode_info>"))
		{
			return TagType.mode_info;
		}
		if (data.Equals("<map_pos>"))
		{
			return TagType.map_pos;
		}
		return TagType.NONE;
	}
	private bool ProcessMapPosLine(string data)
	{
		string[] oneData = data.Split(new char[] { ',' });
		int stageNum = 0;
		int xPos = 0;
		int yPos = 0;
		int direction = 0;
		
		if (int.TryParse(oneData[0], out stageNum) == false)
		{
			ParseError("MapPos_stageNum error", "> 0 && <= stage distance");
		}
		if (int.TryParse(oneData[1], out xPos) == false)
		{
			ParseError("MapPos_xPos error", "> 0 && <= stage distance");
		}
		if (int.TryParse(oneData[2], out yPos) == false)
		{
			ParseError("MapPos_yPos postion error", "must be integer");
		}
		if (int.TryParse(oneData[3], out direction) == false)
		{
			ParseError("MapPos_direction postion error", "must be integer");
		}
		stageCharacterPosition posInfo = new stageCharacterPosition();
		posInfo.x = xPos;
		posInfo.y = yPos;
		posInfo.direction = direction;
		mapPosDic_.Add(stageNum, posInfo);
		return true;
	}
	
	private bool ProcessModeInfoLine(string data)
	{
		string[] oneData = data.Split(new char[] { '=' });
		if (oneData[0].Equals("total_stage"))
		{
			int totalStage = 0;
			if (int.TryParse(oneData[1], out totalStage))
			{

			}
		}
		if (oneData[0].Equals("start_lives"))
		{
			int start_lives = 0;
			if (int.TryParse(oneData[1], out start_lives))
			{
				
			}
		}
		if (oneData[0].Equals("map_background_image_name"))
		{
			
		}
		return true;

	}
	
	private void ParseError(string data, string errorStr)
	{
		Debug.Log("Load Stage Error : " + data);
		Debug.Log("Error : " + errorStr);
		Debug.Log("file name : " + loadingFilePath_ + ", line : " + parcingLineNum_.ToString());
	}

	public void ChangeMapSprite(StageLoader.GameMode mode)
	{
		switch(mode)
		{
			case StageLoader.GameMode.orignal:
			case StageLoader.GameMode.hard:
				{
					gameObject.transform.Find("MapImage").GetComponent<Image>().overrideSprite
						= Resources.Load<Sprite>("Sprites/MapImages/Map_Original");
				}
				break;
			case StageLoader.GameMode.custom:
			default:
				{
					gameObject.transform.Find("MapImage").GetComponent<Image>().overrideSprite
						= Resources.Load<Sprite>("Sprites/MapImages/Map_Original");
				}
				break;
		}
		
	}

	private Constant.MapOpenType openType_;

	private Time openTime_; 
}
