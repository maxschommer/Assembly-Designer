//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2017 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace zSpace.Core
{
    public partial class ZCore
    {
        //////////////////////////////////////////////////////////////////
        // Menu Items
        //////////////////////////////////////////////////////////////////

    #if UNITY_EDITOR
        [MenuItem("zSpace/Open Preview Window")]
        public static void OpenPreviewWindow()
        {
            // Check if the preview window was opened at any point in the
            // Unity session.
            bool wasPreviewWindowOpened = WasPreviewWindowOpened();

            // If the preview window was never opened, position it to be
            // on the zSpace display.
            if (!wasPreviewWindowOpened)
            {
                InitializePreviewWindowPosition();
            }

            zcuOpenPreviewWindow();
        }

        [MenuItem("zSpace/Open Preview Window", true)]
        public static bool ValidateOpenPreviewWindow()
        {
            bool isInitialized = IsPreviewWindowInitialized();
            bool isOpen = IsPreviewWindowOpen();

            return isInitialized && !isOpen;
        }
    #endif

        public static void ClosePreviewWindow()
        {
            zcuClosePreviewWindow();
        }

        public static void SetPreviewWindowEnabled(bool isEnabled)
        {
            zcuSetPreviewWindowEnabled(isEnabled);
        }

        public static bool IsPreviewWindowInitialized()
        {
            bool isInitialized = false;
            zcuIsPreviewWindowInitialized(out isInitialized);

            return isInitialized;
        }

        public static bool IsPreviewWindowOpen()
        {
            bool isOpen = false;
            zcuIsPreviewWindowOpen(out isOpen);

            return isOpen;
        }

        public static bool WasPreviewWindowOpened()
        {
            bool wasOpened = false;
            zcuWasPreviewWindowOpened(out wasOpened);

            return wasOpened;
        }

        private static void InitializePreviewWindowPosition()
        {
            IntPtr displayHandle = IntPtr.Zero;

            // Grab the handle to the zSpace display.
            PluginError error = zcuGetDisplayByType(GlobalState.Instance.Context, DisplayType.zSpace, 0, out displayHandle);
            if (error != PluginError.Ok)
            {
                Debug.LogWarning("zSpace display not found. Unable to position preview window on zSpace display.");
                return;
            }

            int x = 0;
            int y = 0;

            // Grab the display's position.
            error = zcuGetDisplayPosition(displayHandle, out x, out y);
            if (error != PluginError.Ok)
            {
                return;
            }

            // Move the preview window.
            zcuMovePreviewWindow(x, y);
        }
    

        //////////////////////////////////////////////////////////////////
        // Private Types
        //////////////////////////////////////////////////////////////////

        private enum PreviewRenderMode
        {
            LRPattern = 0,
            Default = 1,
        }


        //////////////////////////////////////////////////////////////////
        // Private Methods
        //////////////////////////////////////////////////////////////////

        private void CreatePreviewResources()
        {
            // Create the render texture for the left eye.
            this.CreateRenderTexture(
                "LeftPreviewRenderTexture",
                out _leftPreviewRenderTexture,
                out _leftPreviewNativeTexturePtr);

            // Create the render texture for the right eye.
            this.CreateRenderTexture(
                "RightPreviewRenderTexture",
                out _rightPreviewRenderTexture,
                out _rightPreviewNativeTexturePtr);
        }

        private void CreateRenderTexture(string name, out RenderTexture renderTexture, out IntPtr nativeTexturePtr)
        {
            renderTexture = new RenderTexture(_windowWidth, _windowHeight, 24, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.name = name;
            renderTexture.Create();

            // Cache the render texture's native texture pointer. Per Unity documentation,
            // calling GetNativeTexturePtr() when using multi-threaded rendering will
            // synchronize with the rendering thread (which is a slow operation). So, only
            // call and cache once upon initialization.
            nativeTexturePtr = renderTexture.GetNativeTexturePtr();
        }

        private void DestroyPreviewResources()
        {
            // Reset the render textures' native texture pointers.
            _leftPreviewNativeTexturePtr = IntPtr.Zero;
            _rightPreviewNativeTexturePtr = IntPtr.Zero;

            // Clean up existing render textures.
            if (_leftPreviewRenderTexture != null)
            {
                UnityEngine.Object.Destroy(_leftPreviewRenderTexture);
                _leftPreviewRenderTexture = null;
            }

            if (_rightPreviewRenderTexture != null)
            {
                UnityEngine.Object.Destroy(_rightPreviewRenderTexture);
                _rightPreviewRenderTexture = null;
            }
        }

        private void RenderPreview(PreviewRenderMode renderMode)
        {
            // If the current camera isn't valid, early out.
            if (_currentCamera == null)
            {
                return;
            }

            // Check if the window size has changed. If it has,
            // recreate the preview window's associated resources
            // based on the new size.
            if (_windowWidth != _previousPreviewWindowWidth || _windowHeight != _previousPreviewWindowHeight)
            {
                this.DestroyPreviewResources();
                this.CreatePreviewResources();
            }

            // Disable the camera render callbacks while rendering the left
            // and right frames for the preview window.
            if (_cameraRenderCallbacks != null)
            {
                _cameraRenderCallbacks.enabled = false;
            }

            // Perform the render.
            switch (renderMode)
            {
                case PreviewRenderMode.LRPattern:
                    this.RenderPreviewLRPattern();
                    break;
                case PreviewRenderMode.Default:
                    this.RenderPreviewDefault();
                    break;
                default:
                    break;
            }

            // Re-enable the camera render callbacks so that the center eye view
            // is rendered for the GameView window.
            if (_cameraRenderCallbacks != null)
            {
                _cameraRenderCallbacks.enabled = true;
            }

            // Set the stereo frame textures for the preview window.
            zcuSetPreviewStereoFrame(_leftPreviewNativeTexturePtr, _rightPreviewNativeTexturePtr);

            // Issue a plugin render event to copy the stereo frame textures to
            // underlying shared texture resources, which are available to the
            // preview window.
            ZCore.IssuePluginEvent(PluginEvent.CopyPreviewStereoFrame);

            // Update the previous window width and height in order to determine if they
            // have changed.
            _previousPreviewWindowWidth = _windowWidth;
            _previousPreviewWindowHeight = _windowHeight;
        }

        private void RenderPreviewLRPattern()
        {
            // Cache the camera's original properties.
            int originalCullingMask = _currentCamera.cullingMask;
            CameraClearFlags originalClearFlags = _currentCamera.clearFlags;
            Color originalBackgroundColor = _currentCamera.backgroundColor;

            // Update the culling mask to ignore every layer and set the clear
            // flags to solid color in order to render solid white/black for
            // the left/right eyes respectively.
            _currentCamera.cullingMask = 0;
            _currentCamera.clearFlags = CameraClearFlags.SolidColor;

            // Render white into the left eye.
            _currentCamera.targetTexture = _leftPreviewRenderTexture;
            _currentCamera.backgroundColor = Color.white;
            _currentCamera.Render();

            // Render black into the right eye.
            _currentCamera.targetTexture = _rightPreviewRenderTexture;
            _currentCamera.backgroundColor = Color.black;
            _currentCamera.Render();

            // Restore the camera's properties.
            _currentCamera.cullingMask = originalCullingMask;
            _currentCamera.clearFlags = originalClearFlags;
            _currentCamera.backgroundColor = originalBackgroundColor;
            _currentCamera.targetTexture = null;
        }

        private void RenderPreviewDefault()
        {
            // Cache the current camera's properties.
            Vector3 originalCameraPosition = _currentCamera.transform.position;
            Quaternion originalCameraRotation = _currentCamera.transform.rotation;
            Matrix4x4 originalCameraProjectionMatrix = _currentCamera.projectionMatrix;
            Matrix4x4 originalCameraLocalToWorldMatrix = _currentCamera.transform.localToWorldMatrix;

            // Render the preview for each eye.
            this.RenderPreviewDefaultForEye(Eye.Left, originalCameraLocalToWorldMatrix);
            this.RenderPreviewDefaultForEye(Eye.Right, originalCameraLocalToWorldMatrix);

            // Restore the current camera's properties.
            _currentCamera.transform.position = originalCameraPosition;
            _currentCamera.transform.rotation = originalCameraRotation;
            _currentCamera.projectionMatrix = originalCameraProjectionMatrix;
            _currentCamera.targetTexture = null;
        }

        private void RenderPreviewDefaultForEye(Eye eye, Matrix4x4 localToWorldMatrix)
        {
            if (_currentCamera == null)
            {
                return;
            }

            // Set the camera's target texture.
            switch (eye)
            {
                case Eye.Left:
                    _currentCamera.targetTexture = _leftPreviewRenderTexture;
                    break;
                case Eye.Right:
                    _currentCamera.targetTexture = _rightPreviewRenderTexture;
                    break;
                default:
                    break;
            }

            // Set the camera's transform based on it's original
            // local-to-world matrix and the frustum's view matrix.
            //
            // NOTE: Since we no longer internally flip the handedness
            //       of the view matrices from right to left (in order
            //       to support Unity's new Camera.SetStereoViewMatrix()
            //       API call), we need to explicity flip from right to 
            //       left here.
            Matrix4x4 viewMatrix = this.FlipHandedness(this.GetFrustumViewMatrix(eye));
            Matrix4x4 cameraMatrix = localToWorldMatrix * viewMatrix.inverse;

            _currentCamera.transform.position = cameraMatrix.GetColumn(3);
            _currentCamera.transform.rotation = 
                Quaternion.LookRotation(
                    cameraMatrix.GetColumn(2), 
                    cameraMatrix.GetColumn(1));

            // Set the camera's projection matrix.
            _currentCamera.projectionMatrix = this.GetFrustumProjectionMatrix(eye);

            // Render the frame.
            _currentCamera.Render();
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private int _previousPreviewWindowWidth = 0;
        private int _previousPreviewWindowHeight = 0;

        private RenderTexture _leftPreviewRenderTexture = null;
        private RenderTexture _rightPreviewRenderTexture = null;

        private IntPtr _leftPreviewNativeTexturePtr = IntPtr.Zero;
        private IntPtr _rightPreviewNativeTexturePtr = IntPtr.Zero;
    }
}