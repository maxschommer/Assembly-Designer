using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBoxScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Canvas childCanvas = gameObject.GetComponent<Canvas>();


        // How do you size the button box prefab? Maybe just resize the collider?
        Rect buttonRect = gameObject.GetComponent<Rect>();
        //buttonRect = childCanvas.GetComponent<Rect>();
        buttonRect.width += 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
