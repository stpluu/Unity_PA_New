using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static partial class Constant
{
    //public const float Width_INTO_LargeHole = 2.5f;
	public const int needDepth = 3;
}
public class BossPinScript : MonoBehaviour {

    public int currentDepth_;
	private Sprite[] pinSpriteSet_;
    private bool isPinDrop_;
	// Use this for initialization
	void Start () {
        
    }
    private void Awake()
    {
        pinSpriteSet_ = Resources.LoadAll<Sprite>("Sprites/Boss/boss_point");
    }
    // Update is called once per frame
    void Update () {
        if (transform.position.y <= 0.0f && isPinDrop_ == true)
		{
            isPinDrop_ = false;
            OnPinLanding();
        }
	}
    private void OnEnable()
    {
        currentDepth_ = 0;
        
        
        
    }
	private void OnDisable()
	{
        isPinDrop_ = false;
    }
	private void OnTriggerEnter(Collider other)
    {
        
    }
   /*
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Large Hole : col ");
        if (col.CompareTag("Player"))
        {
            Debug.Log("Large Hole : Player col ");
            GameObject.Find("Player").GetComponent<PlayerScript>().OnCollideLargeHole(gameObject);
        }
    }
    */
    public void StartDrop(Vector3 startPos)
	{
        GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Boss/sprite_set")[33];
        transform.position = startPos;
        GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, -8.0f, 0.0f), ForceMode.VelocityChange);
        isPinDrop_ = true;
    }
    public void OnCharacterLanding()
    {
        if (currentDepth_ < Constant.needDepth)
        {
            currentDepth_++;
           
        }
        
        if (currentDepth_ < pinSpriteSet_.Length)
        {
            string spriteName = string.Format("Sprites/Boss/boss_point_{0:D1}", (Constant.needDepth - currentDepth_));
            GetComponent<SpriteRenderer>().sprite = pinSpriteSet_[currentDepth_];
        }

    }

    private void OnPinLanding()
	{
        GetComponent<SpriteRenderer>().sprite = pinSpriteSet_[0];
        Vector3 pos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        transform.position = pos;

        GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, 8.0f, 0.0f), ForceMode.VelocityChange);
    }
}
