//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;


namespace zSpace.Core
{
    public partial class ZCore
    {
        private class GlobalState
        {
            /// <summary>
            /// Returns a reference to the Core.GlobalState instance.
            /// </summary>
            public static GlobalState Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalState();
                    }

                    return _instance;
                }
            }

            /// <summary>
            /// Destroy the Core.GlobalState instance.
            /// </summary>
            public static void DestroyInstance()
            {
                _instance = null;
            }

            /// <summary>
            /// Returns a reference to the zSpace SDK's context.
            /// </summary>
            public IntPtr Context 
            {
                get
                {
                    return _context;
                }
            }

            /// <summary>
            /// Returns a reference to tracker target handles.
            /// </summary>
            public IntPtr[] TargetHandles
            {
                get
                {
                    return _targetHandles;
                }
            }

            /// <summary>
            /// Returns a reference to the stereo buffer handle.  This is
            /// used to perform LR detect.
            /// </summary>
            public IntPtr StereoBufferHandle
            {
                get
                {
                    return _stereoBufferHandle;
                }
            }

            /// <summary>
            /// Returns a reference to the stereo viewport handle.
            /// </summary>
            public IntPtr ViewportHandle
            {
                get
                {
                    return _viewportHandle;
                }
            }

            /// <summary>
            /// Returns a reference to the stereo frustum handle.
            /// </summary>
            public IntPtr FrustumHandle
            {
                get
                {
                    return _frustumHandle;
                }
            }

            /// <summary>
            /// Returns whether the zSpace Core SDK was properly initialized.
            /// </summary>
            public bool IsInitialized
            {
                get
                {
                    return _isInitialized;
                }
            }

            public AutoStereoState AutoStereoState { get; set; }

            private GlobalState()
            {
                // Initialize the zSpace Unity plugin.
                PluginError error = zcuInitialize(out _context);
                if (error == PluginError.Ok)
                {
                    // Create the stereo buffer for LR detect.
                    error = zcuCreateStereoBuffer(_context, ZCRenderer.Custom, IntPtr.Zero, out _stereoBufferHandle);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to create stereo buffer: ({0})", error));
                    }

                    // Create the viewport.
                    error = zcuCreateViewport(_context, out _viewportHandle);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to create viewport: ({0})", error));
                    }

                    // Grab a reference to the viewport's frustum.
                    error = zcuGetFrustum(_viewportHandle, out _frustumHandle);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to find frustum: ({0})", error));
                    }
                    else
                    {
                        // Set portal mode to "Angle" by default.
                        zcuSetFrustumPortalMode(_frustumHandle, (int)PortalMode.Angle);
                    }

                    // Grab a reference to the default head target (glasses).
                    error = zcuGetTargetByType(_context, TargetType.Head, 0, out _targetHandles[(int)TargetType.Head]);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to find head target: ({0})", error));
                        _targetHandles[(int)TargetType.Head] = IntPtr.Zero;
                    }

                    // Grab a reference to the default primary target (stylus).
                    error = zcuGetTargetByType(_context, TargetType.Primary, 0, out _targetHandles[(int)TargetType.Primary]);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to find primary target: ({0})", error));
                        _targetHandles[(int)TargetType.Primary] = IntPtr.Zero;
                    }
                    else
                    {
                        // Set stylus vibrations to be enabled by default.
                        zcuSetTargetVibrationEnabled(_targetHandles[(int)TargetType.Primary], true);
                    }

                    // Initalize mouse emulation target and button mappings.
                    error = zcuSetMouseEmulationTarget(_context, _targetHandles[(int)TargetType.Primary]);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to initialize mouse emulation target: ({0})", error));
                    }

                    error = zcuSetMouseEmulationButtonMapping(_context, 0, MouseButton.Left);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to map target button 0 to left mouse button: ({0})", error));
                    }

                    error = zcuSetMouseEmulationButtonMapping(_context, 1, MouseButton.Right);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to map target button 1 to right mouse button: ({0})", error));
                    }

                    error = zcuSetMouseEmulationButtonMapping(_context, 2, MouseButton.Center);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to map target button 2 to center mouse button: ({0})", error));
                    }

                    _isInitialized = true;
                }
                else
                {
                    Debug.LogError(string.Format("Failed to initialize zSpace Core SDK: ({0})", error));
                    _isInitialized = false;
                }

                this.AutoStereoState = ZCore.AutoStereoState.IdleStereo;

                // Register the logger callback function.
                LoggerCallbackDelegate callbackDelegate = new LoggerCallbackDelegate(LoggerCallback);
                IntPtr callbackDelegatePtr = Marshal.GetFunctionPointerForDelegate(callbackDelegate);
                zcuSetLoggerFunction(callbackDelegatePtr);
            }

            ~GlobalState()
            {
                // Shutdown the zSpace Unity plugin.
                PluginError error = zcuShutDown(_context);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to shut down zSpace Core SDK: ({0})", error));
                }
                    
                // Clear out context and all handles.
                _context            = IntPtr.Zero;
                _stereoBufferHandle = IntPtr.Zero;
                _viewportHandle     = IntPtr.Zero;
                _frustumHandle      = IntPtr.Zero;

                for (int i = 0; i < (int)TargetType.NumTypes; ++i)
                {
                    _targetHandles[i] = IntPtr.Zero;
                }

                _isInitialized = false;
            }

            private static GlobalState _instance;

            private IntPtr   _context;
            private IntPtr   _stereoBufferHandle;
            private IntPtr   _viewportHandle;
            private IntPtr   _frustumHandle;
            private IntPtr[] _targetHandles = new IntPtr[(int)TargetType.NumTypes];
            private bool     _isInitialized = false;
        }
    }
}