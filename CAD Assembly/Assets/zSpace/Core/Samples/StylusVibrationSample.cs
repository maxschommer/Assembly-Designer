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
    public class StylusVibrationSample : MonoBehaviour
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
        }

        void OnGUI()
        {
            // Capture on period in seconds:
            GUILayout.BeginHorizontal();
            GUILayout.Label("On Period (seconds)");
            _onPeriodTextField = GUILayout.TextField(_onPeriodTextField, GUILayout.Width(50.0f));
            GUILayout.EndHorizontal();

            // Capture off period in seconds:
            GUILayout.BeginHorizontal();
            GUILayout.Label("Off Period (seconds)");
            _offPeriodTextField = GUILayout.TextField(_offPeriodTextField, GUILayout.Width(50.0f));
            GUILayout.EndHorizontal();

            // Capture number of times to repeat the vibration:
            GUILayout.BeginHorizontal();
            GUILayout.Label("Repeat Count");
            _repeatCountTextField = GUILayout.TextField(_repeatCountTextField, GUILayout.Width(50.0f));
            GUILayout.EndHorizontal();

            // Capture intensity (0.0 - 1.0):
            GUILayout.BeginHorizontal();
            GUILayout.Label("Intensity (0.0 - 1.0)");
            _intensityTextField = GUILayout.TextField(_intensityTextField, GUILayout.Width(50.0f));
            GUILayout.EndHorizontal();

            // Start a vibration.
            if (GUILayout.Button("Start Vibration"))
            {
                try
                {
                    // Parse text fields.
                    float onPeriod = float.Parse(_onPeriodTextField);
                    float offPeriod = float.Parse(_offPeriodTextField);
                    int numTimes = int.Parse(_repeatCountTextField);
                    float intensity = float.Parse(_intensityTextField);

                    // Trigger a new vibration.
                    _zCore.StartTargetVibration(ZCore.TargetType.Primary, onPeriod, offPeriod, numTimes, intensity);
                }
                catch
                {
                    Debug.LogError("Invalid vibration parameter.");
                }
            }

            // Stop the vibration.
            GUI.enabled = _zCore.IsTargetVibrating(ZCore.TargetType.Primary);
            if (GUILayout.Button("Stop Vibration"))
            {
                _zCore.StopTargetVibration(ZCore.TargetType.Primary);
            }
            GUI.enabled = true;
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private ZCore _zCore = null;

        private string _onPeriodTextField    = "1.0";
        private string _offPeriodTextField   = "1.0";
        private string _repeatCountTextField = "1";
        private string _intensityTextField   = "1.0";
    }
}