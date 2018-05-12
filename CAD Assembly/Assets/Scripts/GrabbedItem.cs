using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbedItem : MonoBehaviour
{

    public bool isTriggered;

    void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }

    // Use this for initialization
    void Start()
    {
        isTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isTriggered);
    }
}
