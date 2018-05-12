using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;



namespace zSpace.Core
{
    public class CanvasScript : MonoBehaviour
    {
        public GameObject ButtonBox;
        private ZCore _zCore;
        private List<GameObject> assemblyParts = new List<GameObject>();
        private List<GameObject> buttonBoxes = new List<GameObject>();

        void Start()
        {
            // Find and link all assembly parts
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                Reference goRef = go.GetComponent<Reference>();

                if (goRef != null && goRef.isAssemblyPart)
                {
                    // Keep track of parts
                    assemblyParts.Add(go); // Might need to pass by reference or pointer?

                    // Keep track of button boxes

                    //new button box, set text, set position
                    GameObject newButtonBox = Instantiate(ButtonBox, gameObject.transform);

                    newButtonBox.GetComponent<Text>().text = go.name;

                    


                    //Text newPartText = gameObject.AddComponent<Text>();
                    //newPartText.text = go.name;

                    //Debug.Log("Adding :" + go.name);

                    //if (textUI.Count == 0)
                    //{
                    //    newPartText.rectTransform.position = new Vector3(-335, 175, 0); // TODO: Be smart and use gameObject.rectTransform to avoid magic numbers
                    //}
                    //else
                    //{
                    //    newPartText.rectTransform.position = textUI[-1].rectTransform.position + new Vector3(0, -20, 0); // TODO: Another magic number
                    //}

                    //textUI.Add(newPartText);

                    //// Keep track of button boxes
                    
                }
            }
        }


        void Update()
        {

        }
    }
}