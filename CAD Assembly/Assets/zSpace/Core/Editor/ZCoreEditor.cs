//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace zSpace.Core
{
    [CustomEditor(typeof(ZCore))]
    public class ZCoreEditor : Editor
    {
        //////////////////////////////////////////////////////////////////
        // Serialized Properties
        //////////////////////////////////////////////////////////////////

        SerializedProperty ShowLabelsProperty;
        SerializedProperty ShowViewportProperty;
        SerializedProperty ShowCCZoneProperty;
        SerializedProperty ShowUCZoneProperty;
        SerializedProperty ShowDisplayProperty;
        SerializedProperty ShowRealWorldUpProperty;
        SerializedProperty ShowGroundPlaneProperty;
        SerializedProperty ShowGlassesProperty;
        SerializedProperty ShowStylusProperty;

        SerializedProperty CurrentCameraObjectProperty;
        SerializedProperty EnableAutoStereoProperty;
        SerializedProperty IpdProperty;
        SerializedProperty ViewerScaleProperty;
        SerializedProperty AutoStereoDelayProperty;
        SerializedProperty AutoStereoDurationProperty;

        SerializedProperty EnableMouseEmulationProperty;
        SerializedProperty EnableMouseAutoHideProperty;
        SerializedProperty MouseAutoHideDelayProperty;


        //////////////////////////////////////////////////////////////////
        // Unity Callbacks
        //////////////////////////////////////////////////////////////////

        void OnEnable()
        {
            this.LoadIconTextures();
            this.FindSerializedProperties();
            this.LoadSerializedPropertyValues();

            // Ensure only one callback has been registered.
            EditorApplication.update -= OnEditorApplicationUpdate;
            EditorApplication.update += OnEditorApplicationUpdate;
        }

        void OnDisable()
        {
            this.LoadSerializedPropertyValues();

            EditorApplication.update -= OnEditorApplicationUpdate;
        }

        public override void OnInspectorGUI()
        {
            this.InitializeGUIStyles();

            this.serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            this.CheckCoreInitialized();
            this.CheckVirtualRealitySupported();
            this.DrawInfoSection();
            this.DrawDebugSection();
            this.DrawStereoRigSection();
            this.DrawGlassesSection();
            this.DrawStylusSection();
            this.DrawDisplaySection();

            this.serializedObject.ApplyModifiedProperties();
        }


        //////////////////////////////////////////////////////////////////
        // Section Draw Helpers
        //////////////////////////////////////////////////////////////////

        private void CheckCoreInitialized()
        {
            ZCore zCore = (ZCore)this.target;

            if (!zCore.IsInitialized())
            {
                EditorGUILayout.HelpBox(
                    "Failed to properly initialize the zSpace Core SDK. As a result, most zSpace " +
                        "Core functionality will be disabled. Please make sure that the zSpace System " +
                        "Software has been properly installed on your machine.",
                    MessageType.Error);
                EditorGUILayout.Space();
            }
        }

        private void CheckVirtualRealitySupported()
        {
            if (!PlayerSettings.virtualRealitySupported)
            {
                EditorGUILayout.HelpBox(
                    "zSpace has detected that virtual reality support is currently disabled. " +
                        "As a result, standalone player builds will be unable to support " +
                        "stereoscopic 3D rendering for the zSpace device. " +
                        "To enable stereoscopic 3D rendering support, go to:\n\n" +
                        "Edit > Project Settings > Player > PC, Mac & Linux Standalone > Other Settings\n\n" +
                        "Check \"Virtual Reality Supported\" " +
                        "and manually add \"Stereo Display (non head-mounted)\" to the list of Virtual Reality SDKs.",
                    MessageType.Warning);
                EditorGUILayout.Space();
            }
        }

        private void DrawInfoSection()
        {
            ZCore zCore = (ZCore)this.target;

            _isInfoSectionExpanded = this.DrawSectionHeader("General Info", _infoIconTexture, _isInfoSectionExpanded);
            if (_isInfoSectionExpanded)
            {
                string pluginVersion = zCore.GetPluginVersion();
                string runtimeVersion = zCore.IsInitialized() ? zCore.GetRuntimeVersion() : "Unknown";

                EditorGUILayout.LabelField("Plugin Version: " + pluginVersion);
                EditorGUILayout.LabelField("Runtime Version: " + runtimeVersion);
                EditorGUILayout.Space();
            }
        }

        private void DrawDebugSection()
        {
            _isDebugSectionExpanded = this.DrawSectionHeader("Debug", _debugIconTexture, _isDebugSectionExpanded);
            if (_isDebugSectionExpanded)
            {
                this.DrawToggleLeft("Show Labels", this.ShowLabelsProperty);
                this.DrawToggleLeft("Show Viewport (Zero Parallax)", this.ShowViewportProperty);
                this.DrawToggleLeft("Show Comfort Zone (Negative Parallax)", this.ShowCCZoneProperty);
                this.DrawToggleLeft("Show Comfort Zone (Positive Parallax)", this.ShowUCZoneProperty);
                this.DrawToggleLeft("Show Display", this.ShowDisplayProperty);
                this.DrawToggleLeft("Show Real-World Up", this.ShowRealWorldUpProperty);
                this.DrawToggleLeft("Show Ground Plane", this.ShowGroundPlaneProperty);
                this.DrawToggleLeft("Show Glasses", this.ShowGlassesProperty);
                this.DrawToggleLeft("Show Stylus", this.ShowStylusProperty);
                EditorGUILayout.Space();

                GUI.enabled = ZCore.IsPreviewWindowInitialized();
                if (!ZCore.IsPreviewWindowOpen())
                {
                    if (GUILayout.Button("Open Preview Window"))
                    {
                        ZCore.SetPreviewWindowEnabled(Application.isPlaying);
                        ZCore.OpenPreviewWindow();
                    }
                }
                else
                {
                    if (GUILayout.Button("Close Preview Window"))
                    {
                        ZCore.ClosePreviewWindow();
                    }
                }
                GUI.enabled = true;
                EditorGUILayout.Space();

                if (!ZCore.IsPreviewWindowInitialized())
                {
                    EditorGUILayout.HelpBox(
                        "The zSpace preview  window is only available for Unity's D3D11 rendering pipeline.",
                        MessageType.Warning);
                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawStereoRigSection()
        {
            ZCore zCore = (ZCore)this.target;

            _isStereoRigSectionExpanded = this.DrawSectionHeader("Stereo Camera", _cameraIconTexture, _isStereoRigSectionExpanded);
            if (_isStereoRigSectionExpanded)
            {
                // Grab the previous stereo property values.
                Vector3 previousViewportCenter = _viewportCenter;
                Vector3 previousViewportRotation = _viewportRotation;
                float previousViewerScale = zCore.ViewerScale;

                EditorGUILayout.PropertyField(this.CurrentCameraObjectProperty, new GUIContent("Current Camera"));
                EditorGUILayout.Space();

                _viewportCenter = EditorGUILayout.Vector3Field(new GUIContent("Viewport Center", VIEWPORT_CENTER_TOOLTIP), _viewportCenter);
                _viewportRotation = EditorGUILayout.Vector3Field(new GUIContent("Viewport Rotation", VIEWPORT_ROTATION_TOOLTIP), _viewportRotation);
                EditorGUILayout.Space();

                this.DrawToggleLeft("Enable Auto-Transition to Mono", this.EnableAutoStereoProperty);
                EditorGUILayout.Space();

                EditorGUILayout.Slider(this.IpdProperty, 0.0f, 0.1f, new GUIContent("IPD", IPD_TOOLTIP));
                EditorGUILayout.Slider(this.ViewerScaleProperty, 0.001f, 500.0f, new GUIContent("Viewer Scale", VIEWER_SCALE_TOOLTIP));
                EditorGUILayout.Slider(this.AutoStereoDelayProperty, 0.0f, 60.0f, new GUIContent("Auto Stereo Delay", AUTO_STEREO_DELAY_TOOLTIP));
                EditorGUILayout.Slider(this.AutoStereoDurationProperty, 0.0f, 60.0f, new GUIContent("Auto Stereo Duration", AUTO_STEREO_DURATION_TOOLTIP));
                EditorGUILayout.Space();

                // Update the viewport's transform.
                if (_viewportCenter != previousViewportCenter ||
                    _viewportRotation != previousViewportRotation ||
                    this.ViewerScaleProperty.floatValue != previousViewerScale)
                {
                    try
                    {
                        zCore.SetViewportWorldTransform(
                            _viewportCenter,
                            Quaternion.Euler(_viewportRotation),
                            this.ViewerScaleProperty.floatValue);

                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    catch
                    {
                        Debug.LogWarning("Failed to set viewport transform.");
                    }
                }
            }
        }

        private void DrawGlassesSection()
        {
            ZCore zCore = (ZCore)this.target;

            _isGlassesSectionExpanded = this.DrawSectionHeader("Glasses", _glassesIconTexture, _isGlassesSectionExpanded);
            if (_isGlassesSectionExpanded)
            {
                // Display pose information (readonly).
                if (zCore.IsInitialized())
                {
                    this.DrawPoseInfo("Tracker-Space Pose:", zCore.GetTargetPose(ZCore.TargetType.Head, ZCore.CoordinateSpace.Tracker));
                    this.DrawPoseInfo("World-Space Pose:", zCore.GetTargetPose(ZCore.TargetType.Head, ZCore.CoordinateSpace.World));
                }
                else
                {
                    EditorGUILayout.LabelField("Tracker-Space Pose: Unknown");
                    EditorGUILayout.LabelField("World-Space Pose: Unknown");
                }
            }
        }

        private void DrawStylusSection()
        {
            ZCore zCore = (ZCore)this.target;

            _isStylusSectionExpanded = this.DrawSectionHeader("Stylus", _stylusIconTexture, _isStylusSectionExpanded);
            if (_isStylusSectionExpanded)
            {
                this.DrawToggleLeft("Enable Mouse Emulation", this.EnableMouseEmulationProperty);
                this.DrawToggleLeft("Enable Mouse Auto-Hide", this.EnableMouseAutoHideProperty);
                EditorGUILayout.Space();

                EditorGUILayout.Slider(this.MouseAutoHideDelayProperty, 0.0f, 60.0f, new GUIContent("Mouse Auto-Hide Delay"));
                EditorGUILayout.Space();

                // Display pose information (readonly).
                if (zCore.IsInitialized())
                {
                    this.DrawPoseInfo("Tracker-Space Pose:", zCore.GetTargetPose(ZCore.TargetType.Primary, ZCore.CoordinateSpace.Tracker));
                    this.DrawPoseInfo("World-Space Pose:", zCore.GetTargetPose(ZCore.TargetType.Primary, ZCore.CoordinateSpace.World));
                }
                else
                {
                    EditorGUILayout.LabelField("Tracker-Space Pose: Unknown");
                    EditorGUILayout.LabelField("World-Space Pose: Unknown");
                }
            }
        }

        private void DrawDisplaySection()
        {
            ZCore zCore = (ZCore)this.target;

            int numDisplays = zCore.IsInitialized() ? zCore.GetNumDisplays() : 0;
            string sectionName = (numDisplays == 1) ? "Display" : "Displays";

            _isDisplaySectionExpanded = this.DrawSectionHeader(sectionName, _displayIconTexture, _isDisplaySectionExpanded);
            if (_isDisplaySectionExpanded)
            {
                for (int i = 0; i < numDisplays; ++i)
                {
                    IntPtr displayHandle = zCore.GetDisplay(i);
                    string displayName = string.Format("{0}. {1}{2} ({3})",
                                            zCore.GetDisplayNumber(displayHandle),
                                            zCore.GetDisplayAttributeString(displayHandle, ZCore.DisplayAttribute.ManufacturerName),
                                            zCore.GetDisplayAttributeString(displayHandle, ZCore.DisplayAttribute.ProductCode),
                                            zCore.GetDisplayType(displayHandle));

                    EditorGUILayout.LabelField(displayName);
                    EditorGUI.indentLevel++;
                    {
                        GUI.enabled = false;
                        EditorGUILayout.Vector2Field(new GUIContent("Position"), zCore.GetDisplayPosition(displayHandle));
                        EditorGUILayout.Vector2Field(new GUIContent("Size"), zCore.GetDisplaySize(displayHandle));
                        EditorGUILayout.Vector2Field(new GUIContent("Resolution"), zCore.GetDisplayNativeResolution(displayHandle));
                        EditorGUILayout.Vector3Field(new GUIContent("Angle"), zCore.GetDisplayAngle(displayHandle));
                        GUI.enabled = true;
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Helper Methods
        //////////////////////////////////////////////////////////////////

        private void OnEditorApplicationUpdate()
        {
            // Only force the inspector to update/repaint if a section
            // with dynamically changing data is expanded (i.e. glasses,
            // stylus, or display).
            if (_isGlassesSectionExpanded ||
                _isStylusSectionExpanded ||
                _isDisplaySectionExpanded)
            {
                if (_updateFrameCount >= 60)
                {
                    EditorUtility.SetDirty(this.target);
                    _updateFrameCount = 0;
                }

                ++_updateFrameCount;
            }
        }

        private void LoadIconTextures()
        {
            if (_infoIconTexture == null)
            {
                _infoIconTexture = this.LoadIconTexture("InfoIcon.png");
            }

            if (_debugIconTexture == null)
            {
                _debugIconTexture = this.LoadIconTexture("DebugIcon.png");
            }

            if (_cameraIconTexture == null)
            {
                _cameraIconTexture = this.LoadIconTexture("CameraIcon.png");
            }

            if (_glassesIconTexture == null)
            {
                _glassesIconTexture = this.LoadIconTexture("GlassesIcon.png");
            }

            if (_stylusIconTexture == null)
            {
                _stylusIconTexture = this.LoadIconTexture("StylusIcon.png");
            }

            if (_displayIconTexture == null)
            {
                _displayIconTexture = this.LoadIconTexture("DisplayIcon.png");
            }
        }

        private Texture2D LoadIconTexture(string iconName)
        {
            return AssetDatabase.LoadAssetAtPath(INSPECTOR_ICON_PATH + iconName, typeof(Texture2D)) as Texture2D;
        }

        private void FindSerializedProperties()
        {
            // Visual Debugging Properties:
            this.ShowLabelsProperty = this.serializedObject.FindProperty("ShowLabels");
            this.ShowViewportProperty = this.serializedObject.FindProperty("ShowViewport");
            this.ShowCCZoneProperty = this.serializedObject.FindProperty("ShowCCZone");
            this.ShowUCZoneProperty = this.serializedObject.FindProperty("ShowUCZone");
            this.ShowDisplayProperty = this.serializedObject.FindProperty("ShowDisplay");
            this.ShowRealWorldUpProperty = this.serializedObject.FindProperty("ShowRealWorldUp");
            this.ShowGroundPlaneProperty = this.serializedObject.FindProperty("ShowGroundPlane");
            this.ShowGlassesProperty = this.serializedObject.FindProperty("ShowGlasses");
            this.ShowStylusProperty = this.serializedObject.FindProperty("ShowStylus");

            // Stereo Rig Properties:
            this.CurrentCameraObjectProperty = this.serializedObject.FindProperty("CurrentCameraObject");
            this.EnableAutoStereoProperty = this.serializedObject.FindProperty("EnableAutoStereo");
            this.IpdProperty = this.serializedObject.FindProperty("Ipd");
            this.ViewerScaleProperty = this.serializedObject.FindProperty("ViewerScale");
            this.AutoStereoDelayProperty = this.serializedObject.FindProperty("AutoStereoDelay");
            this.AutoStereoDurationProperty = this.serializedObject.FindProperty("AutoStereoDuration");

            // Stylus Properties:
            this.EnableMouseEmulationProperty = this.serializedObject.FindProperty("EnableMouseEmulation");
            this.EnableMouseAutoHideProperty = this.serializedObject.FindProperty("EnableMouseAutoHide");
            this.MouseAutoHideDelayProperty = this.serializedObject.FindProperty("MouseAutoHideDelay");
        }

        private void LoadSerializedPropertyValues()
        {
            ZCore zCore = (ZCore)this.target;

            _viewportCenter = this.Round(zCore.GetViewportWorldCenter());
            _viewportRotation = this.Round(zCore.GetViewportWorldRotation().eulerAngles);
        }

        private void InitializeGUIStyles()
        {
            if (_foldoutStyle == null)
            {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout);
                _foldoutStyle.fontStyle = FontStyle.Bold;
                _foldoutStyle.fixedWidth = 2000.0f;
            }

            if (_lineStyle == null)
            {
                _lineStyle = new GUIStyle(GUI.skin.box);
                _lineStyle.border.top = 1;
                _lineStyle.border.bottom = 1;
                _lineStyle.margin.top = 1;
                _lineStyle.margin.bottom = 1;
                _lineStyle.padding.top = 1;
                _lineStyle.padding.bottom = 1;
            }
        }

        private bool DrawSectionHeader(string name, Texture2D icon, bool isExpanded)
        {
            // Create the divider line.
            GUILayout.Box(GUIContent.none, _lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1.0f));

            // Create the foldout (AKA expandable section).
            Rect position = GUILayoutUtility.GetRect(40.0f, 2000.0f, 16.0f, 16.0f, _foldoutStyle);
            isExpanded = EditorGUI.Foldout(position, isExpanded, new GUIContent(" " + name, icon), true, _foldoutStyle);

            return isExpanded;
        }

        private void DrawToggleLeft(string label, SerializedProperty property)
        {
            property.boolValue = EditorGUILayout.ToggleLeft(new GUIContent(" " + label), property.boolValue);
        }

        private void DrawPoseInfo(string label, ZCore.Pose pose)
        {
            EditorGUILayout.LabelField(new GUIContent(label));

            // Readonly.
            GUI.enabled = false;
            EditorGUILayout.Vector3Field(new GUIContent("Position"), (pose != null) ? pose.Position : Vector3.zero);
            EditorGUILayout.Vector3Field(new GUIContent("Rotation"), (pose != null) ? pose.Rotation.eulerAngles : Vector3.zero);
            EditorGUILayout.Space();
            GUI.enabled = true;
        }

        private Vector3 Round(Vector3 v)
        {
            Vector3 result = v;

            for (int i = 0; i < 3; ++i)
            {
                result[i] = (float)Math.Round((Decimal)v[i], 5);
            }

            return result;
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private static readonly string INSPECTOR_ICON_PATH = "Assets/zSpace/Core/Editor/Icons/";

        private static readonly string VIEWPORT_CENTER_TOOLTIP =
            "The center position of the primary viewport in world space.\n\n" +
            "Note: Modifying the viewport center will update the current camera's transform.";

        private static readonly string VIEWPORT_ROTATION_TOOLTIP =
            "The rotation of the primary viewport in world space.\n\n" +
            "Note: Modifying the viewport rotation will update the current camera's transform.";

        private static readonly string IPD_TOOLTIP =
            "The physical separation, or inter-pupillary distance, between the eyes in meters.";

        private static readonly string VIEWER_SCALE_TOOLTIP =
            "Adjusts the scale of the stereo camera's frustums. Use larger values for scenes " +
            "with large models and smallers values for smaller models. The default value of " +
            "1.0 denotes that all content will be displayed at real-world scale in meters.\n\n" +
            "Note: Modifying viewer scale here will update the current camera's transform in order " +
            "to maintain the specified viewport world center and rotation.";

        private static readonly string AUTO_STEREO_DELAY_TOOLTIP =
            "The delay in seconds before the automatic transition from stereo to mono begins.";

        private static readonly string AUTO_STEREO_DURATION_TOOLTIP =
            "The duration in seconds of the automatic transition from stereo to mono.";

        private int _updateFrameCount = 0;

        private Texture2D _infoIconTexture = null;
        private Texture2D _debugIconTexture = null;
        private Texture2D _cameraIconTexture = null;
        private Texture2D _glassesIconTexture = null;
        private Texture2D _stylusIconTexture = null;
        private Texture2D _displayIconTexture = null;

        private GUIStyle _foldoutStyle = null;
        private GUIStyle _lineStyle = null;

        private bool _isInfoSectionExpanded = true;
        private bool _isDebugSectionExpanded = true;
        private bool _isStereoRigSectionExpanded = true;
        private bool _isGlassesSectionExpanded = false;
        private bool _isStylusSectionExpanded = false;
        private bool _isDisplaySectionExpanded = false;

        private Vector3 _viewportCenter = Vector3.zero;
        private Vector3 _viewportRotation = Vector3.zero;
    }
}

