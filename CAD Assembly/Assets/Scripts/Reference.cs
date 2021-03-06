﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reference : MonoBehaviour
{
    public GameObject ghostObject = null;
    public Vector3 positionDefault = new Vector3();
    public Quaternion rotationDefault = new Quaternion();
    private bool isGhost = false;
    public bool isAssemblyPart = true; // Should maybe be private?

    // Use this for initialization
    void Start()
    {
        if (!gameObject.GetComponent<Reference>().IsGhost())
        {
            // Make ghostObject transparent and fixed
            ghostObject = Instantiate(gameObject);
            ghostObject.transform.position = positionDefault;
            ghostObject.transform.rotation = rotationDefault;
            ghostObject.GetComponent<Reference>().SetGhostStatus(true);
            ghostObject.GetComponent<Reference>().isAssemblyPart = false;
            ghostObject.GetComponent<Renderer>().material = Resources.Load("Ghost") as Material; // TODO: avoiding the Resources folder might help with performance
            ghostObject.GetComponent<MeshCollider>().enabled = false;
            
            ghostObject.transform.localScale = new Vector3(1f,1f,1f);
        }

        // Set position off screen
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsGhost()
    {
        return isGhost;
    }

    public void SetGhostStatus(bool ghostStatus)
    {
        isGhost = ghostStatus;
    }

    public GameObject GetGhostObject()
    {
        return ghostObject;
    }
}
