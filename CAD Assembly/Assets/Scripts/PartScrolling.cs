using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartScrolling : MonoBehaviour {
    private GameObject _scrollbar = null;
    public float iconSize = 0.05f;
    public float yOffset = 0f;
    public float zOffset = 0f;
    // Use this for initialization
    void Start () {
        gameObject.AddComponent<DivideAssembly>();
        gameObject.GetComponent<DivideAssembly>().Initialize(iconSize);

        _scrollbar = GameObject.Find("_Scrollbar");

        // Figure out how much scrolling is needed to see all the parts
    }
	
	// Update is called once per frame
	void Update () {
        float scrollPercent = _scrollbar.GetComponent<Scrolling>().GetScrollPercent();
        //Debug.Log("scroll percent: " + scrollPercent);

        gameObject.transform.position = new Vector3(-scrollPercent * iconSize * gameObject.transform.childCount, yOffset, zOffset);
	}



}
