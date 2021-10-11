using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//move control
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            GetComponent<PlayerScript>().onMoveKey(true);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            GetComponent<PlayerScript>().onMoveKey(false);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            GetComponent<PlayerScript>().onMoveKeyUp(true);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            GetComponent<PlayerScript>().onMoveKeyUp(false);
        }
		
        ////////////////////////////////////////////////////////////////////
        // speed control
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetComponent<PlayerScript>().onSpeedKey(true);
            //Debug.Log("PI : " + Mathf.Sin(Mathf.PI / 4).ToString());
            //Debug.Log("PI * 2: " + Mathf.Sin(Mathf.PI / 2).ToString());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GetComponent<PlayerScript>().onSpeedKey(false);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            GetComponent<PlayerScript>().onSpeedKeyUp(true);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            GetComponent<PlayerScript>().onSpeedKeyUp(false);
        }

        // jump / height control
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<PlayerScript>().onJumpKeyDown();
        }
		if (Input.GetKeyDown(KeyCode.V)
			|| Input.GetKeyDown(KeyCode.M))
		{
			GetComponent<PlayerScript>().onShotKey();
		}

		//GM
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			GetComponent<PlayerScript>().OnLeftBlockKey();
		}
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			GetComponent<PlayerScript>().OnRightBlockKey();
		}
	}
    public bool GetKeyState(KeyCode keyCode)
    {
        return Input.GetKey(keyCode);
    }
}
