//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace zSpace.Core.Samples
{
    public class StylusLedSample : MonoBehaviour
    {
        //////////////////////////////////////////////////////////////////
        // Unity Monobehaviour Callbacks
        //////////////////////////////////////////////////////////////////

        void Start()
        {
            _zCore = GameObject.FindObjectOfType<ZCore>();
            if (_zCore == null)
            {
                Debug.LogError("Unable to find reference to zSpace.Core.Core Monobehaviour.");
                this.enabled = false;
                return;
            }

            // Initialize the stylus LED color to red.
            _zCore.SetTargetLedColor(ZCore.TargetType.Primary, Color.red);
        }

        void OnGUI()
        {
            bool isLedEnabled = _zCore.IsTargetLedEnabled(ZCore.TargetType.Primary);

            // Capture the stylus LED color from user:
            GUILayout.BeginHorizontal();
            GUILayout.Label("R ");
            _rTextField = GUILayout.TextField(_rTextField, GUILayout.Width(50.0f));

            GUILayout.Label("G ");
            _gTextField = GUILayout.TextField(_gTextField, GUILayout.Width(50.0f));

            GUILayout.Label("B ");
            _bTextField = GUILayout.TextField(_bTextField, GUILayout.Width(50.0f));
            GUILayout.EndHorizontal();

            // Update the stylus LED color:
            if (GUILayout.Button("Update LED Color"))
            {
                try
                {
                    // Parse the text fields and convert the (r, g, b) string values to floats.
                    float r = float.Parse(_rTextField);
                    float g = float.Parse(_gTextField);
                    float b = float.Parse(_bTextField);

                    // Set the stylus LED color.
                    _zCore.SetTargetLedColor(ZCore.TargetType.Primary, new Color(r, g, b));
                }
                catch
                {
                    Debug.LogError("Invalid color value.");
                }
            }

            // Enable the stylus LED:
            GUI.enabled = !isLedEnabled;
            if (GUILayout.Button("Turn LED On"))
            {
                _zCore.SetTargetLedEnabled(ZCore.TargetType.Primary, true);
            }
            GUI.enabled = true;

            // Disable the stylus LED:
            GUI.enabled = isLedEnabled;
            if (GUILayout.Button("Turn LED Off"))
            {
                _zCore.SetTargetLedEnabled(ZCore.TargetType.Primary, false);
            }
            GUI.enabled = false;
        }

        void OnDestroy()
        {
            // Turn the stylus LED off if it is currently on.
            if (_zCore.IsTargetLedEnabled(ZCore.TargetType.Primary))
            {
                _zCore.SetTargetLedEnabled(ZCore.TargetType.Primary, false);
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private ZCore _zCore = null;

        private string _rTextField = "1.0";
        private string _gTextField = "0.0";
        private string _bTextField = "0.0";
    }
}