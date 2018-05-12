//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

using UnityEngine;


namespace zSpace.Core
{
    public partial class ZCore
    {
        //////////////////////////////////////////////////////////////////
        // Enumerations
        //////////////////////////////////////////////////////////////////

        public enum PluginError
        {
            Unknown             = -1,
            Ok                  = 0,
            NotImplemented      = 1,
            NotInitialized      = 2,
            AlreadyInitialized  = 3,
            InvalidParameter    = 4,
            InvalidContext      = 5,
            InvalidHandle       = 6,
            RuntimeIncompatible = 7,
            RuntimeNotFound     = 8,
            SymbolNotFound      = 9,
            DisplayNotFound     = 10,
            DeviceNotFound      = 11,
            TargetNotFound      = 12,
            CapabilityNotFound  = 13,
            BufferTooSmall      = 14,
            SyncFailed          = 15,
            OperationFailed     = 16,
            InvalidAttribute    = 17,   
        }

        public enum PluginEvent
        {
            CopyPreviewStereoFrame = 10000,
        }


        //////////////////////////////////////////////////////////////////
        // Public API
        //////////////////////////////////////////////////////////////////

        public static void IssuePluginEvent(PluginEvent pluginEvent)
        {
        #if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
            GL.IssuePluginEvent((int)pluginEvent);
        #else
            IntPtr renderEventFunc = zcuGetRenderEventFunc();
            if (renderEventFunc != IntPtr.Zero)
            {
                GL.IssuePluginEvent(renderEventFunc, (int)pluginEvent);
            }
        #endif
        }


        //////////////////////////////////////////////////////////////////
        // Compound Types
        //////////////////////////////////////////////////////////////////

        [StructLayout(LayoutKind.Explicit, Pack = 8)]
        private struct ZSVector3
        {
            [FieldOffset(0)]
            public float x;

            [FieldOffset(4)]
            public float y;

            [FieldOffset(8)]
            public float z;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 8)]
        private struct ZSMatrix4
        {
            [FieldOffset(0)]
            public float m00;

            [FieldOffset(4)]
            public float m10;

            [FieldOffset(8)]
            public float m20;

            [FieldOffset(12)]
            public float m30;

            [FieldOffset(16)]
            public float m01;

            [FieldOffset(20)]
            public float m11;

            [FieldOffset(24)]
            public float m21;

            [FieldOffset(28)]
            public float m31;

            [FieldOffset(32)]
            public float m02;

            [FieldOffset(36)]
            public float m12;

            [FieldOffset(40)]
            public float m22;

            [FieldOffset(44)]
            public float m32;

            [FieldOffset(48)]
            public float m03;

            [FieldOffset(52)]
            public float m13;

            [FieldOffset(56)]
            public float m23;

            [FieldOffset(60)]
            public float m33;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 8)]
        private struct ZCBoundingBox
        {
            [FieldOffset(0)]
            public ZSVector3 lower;

            [FieldOffset(12)]
            public ZSVector3 upper;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct ZCDisplayIntersectionInfo
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool hit;
            public int x;
            public int y;
            public int nx;
            public int ny;
            public float distance;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 8)]
        private struct ZCFrustumBounds
        {
            [FieldOffset(0)]
            public float left;

            [FieldOffset(4)]
            public float right;

            [FieldOffset(8)]
            public float bottom;

            [FieldOffset(12)]
            public float top;

            [FieldOffset(16)]
            public float nearClip;

            [FieldOffset(20)]
            public float farClip;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct ZCTrackerPose
        {
            public double timestamp;
            public ZSMatrix4 matrix;
        }

        private enum ZCRenderer
        {
            Custom = -1,
        }


        //////////////////////////////////////////////////////////////////
        // General API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr zcuGetRenderEventFunc();

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuGetPluginVersion(
            out int major,
            out int minor,
            out int patch);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuGetWindowPosition(
            out int x,
            out int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuInitialize(
            out IntPtr context);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuUpdate(
            IntPtr context);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuShutDown(
            IntPtr context);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetRuntimeVersion(
            IntPtr context,
            out int major,
            out int minor,
            out int patch);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTrackingEnabled(
            IntPtr context,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTrackingEnabled(
            IntPtr context,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);


        //////////////////////////////////////////////////////////////////
        // Display API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuRefreshDisplays(
            IntPtr context);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetNumDisplays(
            IntPtr context,
            out int numDisplays);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetNumDisplaysByType(
            IntPtr context,
            DisplayType displayType,
            out int numDisplays);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplay(
            IntPtr context, 
            int x,
            int y,
            out IntPtr displayHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayByIndex(
            IntPtr context,
            int index,
            out IntPtr displayHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayByType(
            IntPtr context,
            DisplayType displayType, 
            int index,
            out IntPtr displayHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayType(
            IntPtr displayHandle,
            out DisplayType displayType);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayNumber(
            IntPtr displayHandle,
            out int displayNumber);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetAdapterIndex(
            IntPtr displayHandle,
            out int adapterIndex);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetMonitorIndex(
            IntPtr displayHandle,
            out int monitorIndex);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern PluginError zcuGetDisplayAttributeStr(
            IntPtr displayHandle,
            DisplayAttribute attribute,
            [param: MarshalAs(UnmanagedType.LPStr), Out()]
            StringBuilder buffer,
            int bufferSize);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayAttributeStrSize(
            IntPtr displayHandle,
            DisplayAttribute attribute,
            out int size);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplaySize(
            IntPtr displayHandle, 
            out float width,
            out float height);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayPosition(
            IntPtr displayHandle, 
            out int x,
            out int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayNativeResolution(
            IntPtr displayHandle, 
            out int x,
            out int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayAngle(
            IntPtr displayHandle, 
            out float x,
            out float y,
            out float z);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetDisplayVerticalRefreshRate(
            IntPtr displayHandle,
            out float refreshRate);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsDisplayHardwarePresent(
            IntPtr displayHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isHardwarePresent);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIntersectDisplay(
            IntPtr displayHandle,
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZCTrackerPose pose,
            out ZCDisplayIntersectionInfo intersectionInfo);


        //////////////////////////////////////////////////////////////////
        // StereoBuffer API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuCreateStereoBuffer(
            IntPtr context,
            ZCRenderer renderer,
            IntPtr reserved,
            out IntPtr bufferHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuDestroyStereoBuffer(
            IntPtr bufferHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuBeginStereoBufferPatternDetection(
            IntPtr bufferHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isPatternDetectionEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuEndStereoBufferPatternDetection(
            IntPtr bufferHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isPatternDetected);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsStereoBufferPatternDetected(
            IntPtr bufferHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isPatternDetected);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsStereoBufferSyncRequested(
            IntPtr bufferHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isSyncRequested);


        //////////////////////////////////////////////////////////////////
        // Viewport API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuCreateViewport(
            IntPtr context,
            out IntPtr viewportHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuDestroyViewport(
            IntPtr viewportHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetViewportPosition(
            IntPtr viewportHandle,
            int x,
            int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetViewportPosition(
            IntPtr viewportHandle,
            out int x,
            out int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetViewportSize(
            IntPtr viewportHandle,
            int width,
            int height);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetViewportSize(
            IntPtr viewportHandle,
            out int width,
            out int height);


        //////////////////////////////////////////////////////////////////
        // Coordinate Space API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetCoordinateSpaceTransform(
            IntPtr viewportHandle,
            CoordinateSpace a,
            CoordinateSpace b,
            out ZSMatrix4 transform);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuTransformMatrix(
            IntPtr viewportHandle,
            CoordinateSpace a,
            CoordinateSpace b,
            ref ZSMatrix4 matrix);


        //////////////////////////////////////////////////////////////////
        // Frustum API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustum(
            IntPtr viewportHandle,
            out IntPtr frustumHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetFrustumAttributeF32(
            IntPtr frustumHandle,
            FrustumAttribute attribute,
            float value);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumAttributeF32(
            IntPtr frustumHandle,
            FrustumAttribute attribute,
            out float value);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetFrustumAttributeB(
            IntPtr frustumHandle,
            FrustumAttribute attribute,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool value);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumAttributeB(
            IntPtr frustumHandle,
            FrustumAttribute attribute,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool value);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetFrustumPortalMode(
            IntPtr frustumHandle,
            int portalModeFlags);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumPortalMode(
            IntPtr frustumHandle,
            out int portalModeFlags);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetFrustumCameraOffset(
            IntPtr frustumHandle,
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZSVector3 cameraOffset);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumCameraOffset(
            IntPtr frustumHandle,
            out ZSVector3 cameraOffset);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetFrustumHeadPose(
            IntPtr frustumHandle,
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZCTrackerPose headPose);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumHeadPose(
            IntPtr frustumHandle,
            out ZCTrackerPose headPose);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumViewMatrix(
            IntPtr frustumHandle,
            Eye eye,
            out ZSMatrix4 viewMatrix);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumProjectionMatrix(
            IntPtr frustumHandle,
            Eye eye,
            out ZSMatrix4 projectionMatrix);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumBounds(
            IntPtr frustumHandle,
            Eye eye,
            out ZCFrustumBounds bounds);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumEyePosition(
            IntPtr frustumHandle,
            Eye eye,
            CoordinateSpace coordinateSpace,
            out ZSVector3 eyePosition);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetFrustumCoupledBoundingBox(
            IntPtr frustumHandle,
            out ZCBoundingBox boundingBox);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuCalculateFrustumFit(
            IntPtr frustumHandle,
            ZCBoundingBox boundingBox,
            out float viewerScale,
            out ZSMatrix4 lookAtMatrix);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuCalculateFrustumDisparity(
            IntPtr frustumHandle,
            ZSVector3 point,
            out float disparity);


        //////////////////////////////////////////////////////////////////
        // Tracker Device API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetNumTrackerDevices(
            IntPtr context,
            out int numDevices);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTrackerDevice(
            IntPtr context,
            int index,
            out IntPtr deviceHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern PluginError zcuGetTrackerDeviceByName(
            IntPtr context,
            [param: MarshalAs(UnmanagedType.LPStr)]
            string deviceName,
            out IntPtr deviceHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTrackerDeviceEnabled(
            IntPtr deviceHandle,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTrackerDeviceEnabled(
            IntPtr deviceHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern PluginError zcuGetTrackerDeviceName(
            IntPtr deviceHandle,
            [param: MarshalAs(UnmanagedType.LPStr), Out()]
            StringBuilder buffer,
            int bufferSize);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTrackerDeviceNameSize(
            IntPtr deviceHandle,
            out int size);


        //////////////////////////////////////////////////////////////////
        // Tracker Target API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetNumTargets(
            IntPtr deviceHandle,
            out int numTargets);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetNumTargetsByType(
            IntPtr context,
            TargetType targetType,
            out int numTargets);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTarget(
            IntPtr deviceHandle,
            int index,
            out IntPtr targetHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern PluginError zcuGetTargetByName(
            IntPtr deviceHandle,
            [param: MarshalAs(UnmanagedType.LPStr)]
            string targetName,
            out IntPtr targetHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetByType(
            IntPtr context,
            TargetType targetType,
            int index,
            out IntPtr targetHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern PluginError zcuGetTargetName(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.LPStr), Out()]
            StringBuilder buffer,
            int bufferSize);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetNameSize(
            IntPtr targetHandle,
            out int size);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTargetEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);
        
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetVisible(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTargetMoveEventThresholds(
            IntPtr targetHandle,
            float time,
            float distance,
            float angle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetMoveEventThresholds(
            IntPtr targetHandle,
            out float time,
            out float distance,
            out float angle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetPose(
            IntPtr targetHandle,
            out ZCTrackerPose pose);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTargetPoseBufferingEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isPoseBufferingEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetPoseBufferingEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isPoseBufferingEnabled);

        // TODO: Validate.
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetPoseBuffer(
            IntPtr targetHandle,
            float minDelta,
            float maxDelta,
            ZCTrackerPose[] buffer,
            out int bufferSize);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuResizeTargetPoseBuffer(
            IntPtr targetHandle,
            int capacity);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetPoseBufferCapacity(
            IntPtr targetHandle,
            out int capacity);


        //////////////////////////////////////////////////////////////////
        // Tracker Target Button API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetNumTargetButtons(
            IntPtr targetHandle,
            out int numButtons);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetButtonPressed(
            IntPtr targetHandle,
            int buttonId,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isButtonPressed);


        //////////////////////////////////////////////////////////////////
        // Tracker Target LED API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTargetLedEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isLedEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetLedEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isLedEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTargetLedColor(
            IntPtr targetHandle,
            float r,
            float g,
            float b);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetLedColor(
            IntPtr targetHandle,
            out float r,
            out float g,
            out float b);


        //////////////////////////////////////////////////////////////////
        // Tracker Target Vibration API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTargetVibrationEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isVibrationEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetVibrationEnabled(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isVibrationEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetVibrating(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isVibrating);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuStartTargetVibration(
            IntPtr targetHandle,
            float onPeriod,
            float offPeriod,
            int numTimes,
            float intensity);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuStopTargetVibration(
            IntPtr targetHandle);


        //////////////////////////////////////////////////////////////////
        // Tracker Target Tap API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsTargetTapPressed(
            IntPtr targetHandle,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isTapPressed);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetTargetTapThreshold(
            IntPtr targetHandle,
            float seconds);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetTargetTapThreshold(
            IntPtr targetHandle,
            out float seconds);


        //////////////////////////////////////////////////////////////////
        // Mouse Emulation API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetMouseEmulationEnabled(
            IntPtr context,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool isEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuIsMouseEmulationEnabled(
            IntPtr context,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetMouseEmulationTarget(
            IntPtr context,
            IntPtr targetHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetMouseEmulationTarget(
            IntPtr context,
            out IntPtr targetHandle);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetMouseEmulationMovementMode(
            IntPtr context,
            MovementMode movementMode);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetMouseEmulationMovementMode(
            IntPtr context,
            out MovementMode movementMode);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetMouseEmulationMaxDistance(
            IntPtr context,
            float maxDistance);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetMouseEmulationMaxDistance(
            IntPtr context,
            out float maxDistance);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuSetMouseEmulationButtonMapping(
            IntPtr context,
            int buttonId,
            MouseButton mouseButton);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zcuGetMouseEmulationButtonMapping(
            IntPtr context,
            int buttonId,
            out MouseButton mouseButton);


        //////////////////////////////////////////////////////////////////
        // Stereo Preview Window API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuIsPreviewWindowInitialized(
            out bool isInitialized);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuIsPreviewWindowOpen(
            out bool isOpen);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuWasPreviewWindowOpened(
            out bool wasOpened);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuOpenPreviewWindow();

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuClosePreviewWindow();

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuSetPreviewWindowEnabled(
            bool isEnabled);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuSetPreviewWindowToForeground();

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuMovePreviewWindow(
            int x,
            int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuResizePreviewWindow(
            int width,
            int height);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuGetPreviewWindowPosition(
            out int x,
            out int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuGetPreviewWindowSize(
            out int width,
            out int height);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuGetPreviewWindowViewportPosition(
            out int x,
            out int y);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuGetPreviewWindowViewportSize(
            out int width,
            out int height);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuSetPreviewStereoFrame(
            IntPtr leftFrame,
            IntPtr rightFrame);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zcuSetLoggerFunction(
            IntPtr fp);


        //////////////////////////////////////////////////////////////////
        // Win32 Compound Types
        //////////////////////////////////////////////////////////////////

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;   // x position of upper-left corner
            public int Top;    // y position of upper-left corner
            public int Right;  // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }


        //////////////////////////////////////////////////////////////////
        // Win32 API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        //////////////////////////////////////////////////////////////////
        // Private Helpers
        //////////////////////////////////////////////////////////////////

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void LoggerCallbackDelegate(int level, string message);

        private static void LoggerCallback(int level, string message)
        {
            message = "[zCoreUnity] " + message;
            switch (level)
            {
                case 0:
                    Debug.Log(message);
                    break;
                case 1:
                    Debug.LogWarning(message);
                    break;
                case 2:
                    Debug.LogError(message);
                    break;
                default:
                    break;
            }
        }

        private PluginException NewPluginException(PluginError error)
        {
            switch (error)
            {
                case PluginError.NotImplemented:
                    return new NotImplementedException();
                case PluginError.NotInitialized:
                    return new NotInitializedException();
                case PluginError.InvalidParameter:
                    return new InvalidParameterException();
                case PluginError.InvalidContext:
                    return new InvalidContextException();
                case PluginError.InvalidHandle:
                    return new InvalidHandleException();
                case PluginError.RuntimeIncompatible:
                    return new RuntimeIncompatibleException();
                case PluginError.RuntimeNotFound:
                    return new RuntimeNotFoundException();
                case PluginError.SymbolNotFound:
                    return new SymbolNotFoundException();
                case PluginError.DisplayNotFound:
                    return new DisplayNotFoundException();
                case PluginError.DeviceNotFound:
                    return new DeviceNotFoundException();
                case PluginError.TargetNotFound:
                    return new TargetNotFoundException();
                case PluginError.CapabilityNotFound:
                    return new CapabilityNotFoundException();
                case PluginError.BufferTooSmall:
                    return new BufferTooSmallException();
                case PluginError.SyncFailed:
                    return new SyncFailedException();
                case PluginError.OperationFailed:
                    return new OperationFailedException();
                case PluginError.InvalidAttribute:
                    return new InvalidAttributeException();
                default:
                    return new PluginException(PluginError.Unknown);
            }
        }
    }

    [Serializable]
    public class PluginException : Exception
    {
        public ZCore.PluginError PluginError { get; private set; }

        public PluginException(ZCore.PluginError pluginError)
            : base()
        {
            this.PluginError = pluginError;
        }

        public PluginException(ZCore.PluginError pluginError, string message)
            : base(message)
        {
            this.PluginError = pluginError;
        }

        protected PluginException(ZCore.PluginError pluginError, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.PluginError = pluginError;
        }
    }

    [Serializable]
    public class NotImplementedException : PluginException
    {
        public NotImplementedException()
            : base(ZCore.PluginError.NotImplemented)
        {
        }

        public NotImplementedException(string message)
            : base(ZCore.PluginError.NotImplemented, message)
        {
        }

        protected NotImplementedException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.NotImplemented, info, context)
        {
        }
    }

    [Serializable]
    public class NotInitializedException : PluginException
    {
        public NotInitializedException()
            : base(ZCore.PluginError.NotInitialized)
        {
        }

        public NotInitializedException(string message)
            : base(ZCore.PluginError.NotInitialized, message)
        {
        }

        protected NotInitializedException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.NotInitialized, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidParameterException : PluginException
    {
        public InvalidParameterException()
            : base(ZCore.PluginError.InvalidParameter)
        {
        }

        public InvalidParameterException(string message)
            : base(ZCore.PluginError.InvalidParameter, message)
        {
        }

        protected InvalidParameterException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.InvalidParameter, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidContextException : PluginException
    {
        public InvalidContextException()
            : base(ZCore.PluginError.InvalidContext)
        {
        }

        public InvalidContextException(string message)
            : base(ZCore.PluginError.InvalidContext, message)
        {
        }

        protected InvalidContextException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.InvalidContext, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidHandleException : PluginException
    {
        public InvalidHandleException()
            : base(ZCore.PluginError.InvalidHandle)
        {
        }

        public InvalidHandleException(string message)
            : base(ZCore.PluginError.InvalidHandle, message)
        {
        }

        protected InvalidHandleException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.InvalidHandle, info, context)
        {
        }
    }

    [Serializable]
    public class RuntimeIncompatibleException : PluginException
    {
        public RuntimeIncompatibleException()
            : base(ZCore.PluginError.RuntimeIncompatible)
        {
        }

        public RuntimeIncompatibleException(string message)
            : base(ZCore.PluginError.RuntimeIncompatible, message)
        {
        }

        protected RuntimeIncompatibleException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.RuntimeIncompatible, info, context)
        {
        }
    }

    [Serializable]
    public class RuntimeNotFoundException : PluginException
    {
        public RuntimeNotFoundException()
            : base(ZCore.PluginError.RuntimeNotFound)
        {
        }

        public RuntimeNotFoundException(string message)
            : base(ZCore.PluginError.RuntimeNotFound, message)
        {
        }

        protected RuntimeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.RuntimeNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class SymbolNotFoundException : PluginException
    {
        public SymbolNotFoundException()
            : base(ZCore.PluginError.SymbolNotFound)
        {
        }

        public SymbolNotFoundException(string message)
            : base(ZCore.PluginError.SymbolNotFound, message)
        {
        }

        protected SymbolNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.SymbolNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class DisplayNotFoundException : PluginException
    {
        public DisplayNotFoundException()
            : base(ZCore.PluginError.DisplayNotFound)
        {
        }

        public DisplayNotFoundException(string message)
            : base(ZCore.PluginError.DisplayNotFound, message)
        {
        }

        protected DisplayNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.DisplayNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class DeviceNotFoundException : PluginException
    {
        public DeviceNotFoundException()
            : base(ZCore.PluginError.DeviceNotFound)
        {
        }

        public DeviceNotFoundException(string message)
            : base(ZCore.PluginError.DeviceNotFound, message)
        {
        }

        protected DeviceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.DeviceNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class TargetNotFoundException : PluginException
    {
        public TargetNotFoundException()
            : base(ZCore.PluginError.TargetNotFound)
        {
        }

        public TargetNotFoundException(string message)
            : base(ZCore.PluginError.TargetNotFound, message)
        {
        }

        protected TargetNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.TargetNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class CapabilityNotFoundException : PluginException
    {
        public CapabilityNotFoundException()
            : base(ZCore.PluginError.CapabilityNotFound)
        {
        }

        public CapabilityNotFoundException(string message)
            : base(ZCore.PluginError.CapabilityNotFound, message)
        {
        }

        protected CapabilityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.CapabilityNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class BufferTooSmallException : PluginException
    {
        public BufferTooSmallException()
            : base(ZCore.PluginError.BufferTooSmall)
        {
        }

        public BufferTooSmallException(string message)
            : base(ZCore.PluginError.BufferTooSmall, message)
        {
        }

        protected BufferTooSmallException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.BufferTooSmall, info, context)
        {
        }
    }

    [Serializable]
    public class SyncFailedException : PluginException
    {
        public SyncFailedException()
            : base(ZCore.PluginError.SyncFailed)
        {
        }

        public SyncFailedException(string message)
            : base(ZCore.PluginError.SyncFailed, message)
        {
        }

        protected SyncFailedException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.SyncFailed, info, context)
        {
        }
    }

    [Serializable]
    public class OperationFailedException : PluginException
    {
        public OperationFailedException()
            : base(ZCore.PluginError.OperationFailed)
        {
        }

        public OperationFailedException(string message)
            : base(ZCore.PluginError.OperationFailed, message)
        {
        }

        protected OperationFailedException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.OperationFailed, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidAttributeException : PluginException
    {
        public InvalidAttributeException()
            : base(ZCore.PluginError.InvalidAttribute)
        {
        }

        public InvalidAttributeException(string message)
            : base(ZCore.PluginError.InvalidAttribute, message)
        {
        }

        protected InvalidAttributeException(SerializationInfo info, StreamingContext context)
            : base(ZCore.PluginError.InvalidAttribute, info, context)
        {
        }
    }
}