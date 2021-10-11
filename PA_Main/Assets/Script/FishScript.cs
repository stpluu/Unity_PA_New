using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Constant
{
    public const float Time_Fish = 1.5f;
    public const float Height_FishJump = 6.8f;

    public const float Speed_FishXMove = 2.76f;
	public const float Speed_FishXMoveToOutSide = 1.56f;
	public const float Speed_FishZMove = -8.0f;
	public const float Speed_FishJump = 10.5f;
}
public class FishScript : MonoBehaviour {
    public float createTime_;
    public int movingDirection_;    //x축 이동 방향
    private void Awake()
    {
        
    }
    // Use this for initialization
    void Start () {
      
    }
    private void OnEnable()
    {
        GetComponent<BoxCollider>().enabled = false;
        createTime_ = Time.time;
        // StartCoroutine("UpdateFishPosition");
        //Debug.Log("Fish Enabled");

        Vector3 moveVector = new Vector3();
		bool isOutSide = false;
		if (transform.position.x < 0
			&& GameObject.FindGameObjectWithTag("Player").transform.position.x
				< transform.position.x)
		{
			isOutSide = true;
		}
		else if (transform.position.x > 0
			&& GameObject.FindGameObjectWithTag("Player").transform.position.x
				> transform.position.x)
		{
			isOutSide = true;
		}
		if (isOutSide)
			moveVector.x = Constant.Speed_FishXMoveToOutSide;
		else
			moveVector.x = Constant.Speed_FishXMove;
        if (GameObject.FindGameObjectWithTag("Player").transform.position.x
            < transform.position.x)
        {
            moveVector.x *= -1.0f;
        }
        moveVector.y = Constant.Speed_FishJump;
		moveVector.z = Constant.Speed_FishZMove;
        // if ()
		
        GetComponent<Rigidbody>().AddForce(moveVector, ForceMode.Impulse);
    }
    private void OnDisable()
    {
		//GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.Force);
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.position = Vector3.zero;
	}
    // Update is called once per frame
    void Update () {
        //if (transform.position.y < -1.0f)
        // {
        //    StopCoroutine("UpdateFishPosition");
        //}
		if (transform.position.z < 0.0f)
		{
			Vector3 pos = new Vector3();
			pos.x = transform.position.x;
			pos.y = transform.position.y;
			pos.z = 0.0f;
			transform.position = pos;
		}
        if (transform.position.y > 3.5f
            && GetComponent<BoxCollider>().enabled == false)
        {
            //Debug.Log("Fish-Col Enabled");
            GetComponent<BoxCollider>().enabled = true;
		}
        if (transform.position.y < -0.3f)
        {
            //Debug.Log("Fish - deactivated");
            gameObject.SetActive(false);
           
        }

    }
	/*
    IEnumerator UpdateFishPosition()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.02f);
            Vector3 fishPos = new Vector3();
            fishPos.y = calcCurrentYPos();
            fishPos.z = calcCurrentZPos();
            fishPos.x = transform.position.x + movingDirection_ * 0.01f;
            if (fishPos.x < Constant.Position_VerticalMoveLimit_Left)
                fishPos.x = Constant.Position_VerticalMoveLimit_Left;
            else if (fishPos.x > Constant.Position_VerticalMoveLimit_Right)
                fishPos.x = Constant.Position_VerticalMoveLimit_Right;
            transform.position = fishPos;
        }
    }
    
    float calcCurrentYPos()
    {
        
        float pastTime = Time.time - createTime_;
        if (Constant.Time_Fish / 2 > pastTime)
        {
            return Mathf.Sin(Mathf.PI / 2 * (pastTime * 2)) * Constant.Height_FishJump;
        }
        else
        {
            return Mathf.Cos(Mathf.PI / 2 * (pastTime - (Constant.Time_Fish / 2)) * 2) * Constant.Height_FishJump;
        }
        //  return Mathf.Sin((Mathf.PI * 2 * jumpDeltaTime) / jumpTime) * jumpHeight;
    }

    float calcCurrentZPos()
    {
        float pastTime = Time.time - createTime_;
        if (Constant.Time_Fish / 2 > pastTime)
        {
            return Constant.Distance_FishCreate - (Constant.Distance_FishCreate - pastTime / (Constant.Time_Fish / 2));
        }
        else
        {
            return 0.0f;
        }
    }
	*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Fish : Player col ");
            //GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideLargeHole(gameObject);
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().OnCollideFish(gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
