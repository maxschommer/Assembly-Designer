using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour {

    // Use this for initialization
    void Start () {
        gameObject.transform.position = new Vector3(minScroll, gameObject.transform.position.y, gameObject.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

    }

    public float GetScrollPercent()
    {
        return (gameObject.transform.position.x - minScroll) / (maxScroll - minScroll);
    }

    public float getMinScroll()
    {
        return minScroll;
    }

    public float getMaxScroll()
    {
        return maxScroll;
    }

    //public float minScroll = -0.058f;
    //public float maxScroll = 0.07f;

    private float minScroll = -0.2f;
    private float maxScroll = 0.3f;
}
