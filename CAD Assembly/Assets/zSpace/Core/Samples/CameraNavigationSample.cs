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
    public class CameraNavigationSample : MonoBehaviour
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

        void Update()
        {
            this.UpdateTweens();
        }

        void OnGUI()
        {
            // Navigate to object:
            if (GUILayout.Button("Navigate to Cube"))
            {
                this.NavigateTo(GameObject.Find("Cube").transform.position, Quaternion.identity, 1.0f);
            }
            if (GUILayout.Button("Navigate to Sphere"))
            {
                this.NavigateTo(GameObject.Find("Sphere").transform.position, Quaternion.Euler(45.0f, 0.0f, 0.0f), 0.5f);
            }
            if (GUILayout.Button("Navigate to Capsule"))
            {
                this.NavigateTo(GameObject.Find("Capsule").transform.position, Quaternion.Euler(0.0f, -45.0f, 0.0f), 2.0f);
            }

            // Perspective:
            if (GUILayout.Button("Left Perspective"))
            {
                this.NavigateTo(Quaternion.Euler(0.0f, 90.0f, 0.0f));
            }
            if (GUILayout.Button("Right Perspective"))
            {
                this.NavigateTo(Quaternion.Euler(0.0f, -90.0f, 0.0f));
            }
            if (GUILayout.Button("Top Perspective"))
            {
                this.NavigateTo(Quaternion.Euler(90.0f, 0.0f, 0.0f));
            }
            if (GUILayout.Button("Bottom Perspective"))
            {
                this.NavigateTo(Quaternion.Euler(-90.0f, 0.0f, 0.0f));
            }
            if (GUILayout.Button("Front Perspective"))
            {
                this.NavigateTo(Quaternion.Euler(0.0f, 0.0f, 0.0f));
            }
            if (GUILayout.Button("Back Perspective"))
            {
                this.NavigateTo(Quaternion.Euler(180.0f, 0.0f, -180.0f));
            }

            // Zoom in based on relative scale factor:
            GUILayout.BeginHorizontal();
            _zoomInTextField = GUILayout.TextField(_zoomInTextField, GUILayout.Width(50.0f));
            if (GUILayout.Button("Zoom In"))
            {
                try
                {
                    float scaleFactor = float.Parse(_zoomInTextField);
                    this.Zoom(scaleFactor);
                }
                catch
                {
                    Debug.LogError("Invalid scale factor.");
                }
            }
            GUILayout.EndHorizontal();

            // Zoom out based on scale factor:
            GUILayout.BeginHorizontal();
            _zoomOutTextField = GUILayout.TextField(_zoomOutTextField, GUILayout.Width(50.0f));
            if (GUILayout.Button("Zoom Out"))
            {
                try
                {
                    float scaleFactor = float.Parse(_zoomOutTextField);
                    this.Zoom(1.0f / scaleFactor);
                }
                catch
                {
                    Debug.LogError("Invalid scale factor.");
                }
            }
            GUILayout.EndHorizontal();

            // Zoom in based on absolute scale:
            GUILayout.BeginHorizontal();
            _zoomInAbsoluteTextField = GUILayout.TextField(_zoomInAbsoluteTextField, GUILayout.Width(50.0f));
            if (GUILayout.Button("Zoom In Absolute"))
            {
                try
                {
                    float scale = float.Parse(_zoomInAbsoluteTextField);
                    float scaleFactor = _zCore.ViewerScale * scale;
                    this.Zoom(scaleFactor);
                }
                catch
                {
                    Debug.LogError("Invalid absolute scale.");
                }
            }
            GUILayout.EndHorizontal();

            // Zoom out based on absolute scale:
            GUILayout.BeginHorizontal();
            _zoomOutAbsoluteTextField = GUILayout.TextField(_zoomOutAbsoluteTextField, GUILayout.Width(50.0f));
            if (GUILayout.Button("Zoom Out Absolute"))
            {
                try
                {
                    float scale = float.Parse(_zoomOutAbsoluteTextField);
                    float scaleFactor = _zCore.ViewerScale / scale;
                    this.Zoom(scaleFactor);
                }
                catch
                {
                    Debug.LogError("Invalid absolute scale.");
                }
            }
            GUILayout.EndHorizontal();
        }


        //////////////////////////////////////////////////////////////////
        // Private Methods
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Navigate the stereo rig such that the center of the viewport will be positioned
        /// at the specified viewport center.
        /// </summary>
        /// <param name="viewportCenter">The viewport center position in world space.</param>
        /// <returns>Tween info corresponding to the current navigation.</returns>
        private TweenInfo NavigateTo(Vector3 viewportCenter)
        {
            return this.NavigateTo(viewportCenter, _zCore.GetViewportWorldRotation(), _zCore.ViewerScale);
        }

        /// <summary>
        /// Navigate the stereo rig such that the viewport will be oriented based on the specified
        /// viewport rotation.
        /// </summary>
        /// <param name="viewportRotation">The viewport rotation in world space.</param>
        /// <returns>Tween info corresponding to the current navigation.</returns>
        private TweenInfo NavigateTo(Quaternion viewportRotation)
        {
            return this.NavigateTo(_zCore.GetViewportWorldCenter(), viewportRotation, _zCore.ViewerScale);
        }

        /// <summary>
        /// Navigate the stereo rig such that the viewport will be scaled based on the
        /// the specifed viewer scale.
        /// </summary>
        /// <param name="viewerScale">The viewer scale.</param>
        /// <returns>Tween info corresponding to the current navigation.</returns>
        private TweenInfo NavigateTo(float viewerScale)
        {
            return this.NavigateTo(_zCore.GetViewportWorldCenter(), _zCore.GetViewportWorldRotation(), viewerScale);
        }

        /// <summary>
        /// Navigate the stereo rig such that the viewport is positioned, oriented, and scaled
        /// based on the specified viewport center, viewport rotation, and viewer scale.
        /// </summary>
        /// <param name="viewportCenter">The viewport center position in world space.</param>
        /// <param name="viewportRotation">The viewport rotation in world space.</param>
        /// <param name="viewerScale">The viewer scale.</param>
        /// <returns>Tween info corresponding to the current navigation.</returns>
        private TweenInfo NavigateTo(Vector3 viewportCenter, Quaternion viewportRotation, float viewerScale)
        {
            if (_zCore.CurrentCameraObject == null)
            {
                return null;
            }

            if (_cameraTweenInfo != null)
            {
                this.CancelTween(_cameraTweenInfo);
                _cameraTweenInfo = null;
            }

            // Grab the current viewport center, viewport rotation, and viewer scale.
            Vector3 currentViewportCenter = _zCore.GetViewportWorldCenter();
            Quaternion currentViewportRotation = _zCore.GetViewportWorldRotation();
            float currentViewerScale = _zCore.ViewerScale;

            Action<float> onUpdate = (t) =>
            {
                // Interpolate between the current and final values.
                Vector3 p = Vector3.Lerp(currentViewportCenter, viewportCenter, t);
                Quaternion r = Quaternion.Lerp(currentViewportRotation, viewportRotation, t);
                float v = Mathf.Lerp(currentViewerScale, viewerScale, t);

                _zCore.SetViewportWorldTransform(p, r, v);
            };

            _cameraTweenInfo = this.StartTween(onUpdate, 1.5f).SetEase(EaseType.EaseOutExpo);
            return _cameraTweenInfo;
        }

        /// <summary>
        /// Perform a zoom based on the specified relative scale factor.
        /// </summary>
        /// <param name="scaleFactor">The relative scale factor.</param>
        /// <returns>Tween info corresponding to the current zoom.</returns>
        private TweenInfo Zoom(float scaleFactor)
        {
            if (_zCore == null)
            {
                return null;
            }

            if (_cameraTweenInfo != null)
            {
                this.CancelTween(_cameraTweenInfo);
                _cameraTweenInfo = null;
            }

            Vector3 currentViewportCenter = _zCore.GetViewportWorldCenter();
            Quaternion currentViewportRotation = _zCore.GetViewportWorldRotation();
            float currentViewerScale = _zCore.ViewerScale;

            float viewerScale = currentViewerScale / scaleFactor;

            Action<float> onUpdate = (t) =>
            {
                // Interpolate between the current and final values.
                float v = Mathf.Lerp(currentViewerScale, viewerScale, t);

                // Update Core's current camera position and viewer scale.
                _zCore.SetViewportWorldTransform(currentViewportCenter, currentViewportRotation, v);
            };

            _cameraTweenInfo = this.StartTween(onUpdate, 1.5f).SetEase(EaseType.EaseOutExpo);
            return _cameraTweenInfo;
        }

        /// <summary>
        /// Computes a normalized time (between 0.0 and 1.0) based on a current time,
        /// duration, and ease type.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="d">Duration in seconds.</param>
        /// <param name="ease">Ease type.</param>
        /// <returns>Normalized time between 0.0 and 1.0 (inclusive).</returns>
        private float ComputeNormalizedTime(float t, float d, EaseType ease)
        {
            if (t <= 0)
            {
                return 0;
            }

            float a = Mathf.Clamp01(t / d);

            switch (ease)
            {
                case EaseType.Linear:
                    return a;

                case EaseType.EaseInQuad:
                    return Mathf.Pow(a, 2);

                case EaseType.EaseOutQuad:
                    return -a * (a - 2);

                case EaseType.EaseInExpo:
                    return Mathf.Pow(2, 10 * (a - 1));

                case EaseType.EaseOutExpo:
                    return (-Mathf.Pow(2, -10 * a) + 1);

                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Starts a tween.
        /// </summary>
        /// <param name="onUpdate">Update callback invoked evert frame the tween is active.</param>
        /// <param name="duration">Duration of the tween in seconds.</param>
        /// <returns></returns>
        private TweenInfo StartTween(Action<float> onUpdate, float duration)
        {
            TweenInfo info = new TweenInfo(onUpdate);
            info.Duration = duration;

            _tweenInfos.Add(info);

            return info;
        }

        /// <summary>
        /// Cancels an existing tween that is being updated.
        /// </summary>
        /// <param name="info">Reference to the tween.</param>
        private void CancelTween(TweenInfo info)
        {
            if (_tweenInfos.Contains(info))
            {
                _tweenInfos.Remove(info);
            }
        }

        /// <summary>
        /// Updates all existing tweens.
        /// </summary>
        private void UpdateTweens()
        {
            for (int i = _tweenInfos.Count - 1; i >= 0; i--)
            {
                TweenInfo info = _tweenInfos[i];

                if (info.Delay > 0.0f)
                {
                    info.Delay -= Time.unscaledDeltaTime;
                    continue;
                }

                // Advance the time.
                info.Time += Time.unscaledDeltaTime;

                // Compute the normalized time.
                float t = this.ComputeNormalizedTime(info.Time, info.Duration, info.Ease);

                // Invoke custom updaters.
                if (info.OnUpdate != null)
                {
                    info.OnUpdate(t);
                }

                // Check if the tween has finished.
                if (info.Time >= info.Duration)
                {
                    if (info.OnComplete != null)
                    {
                        info.OnComplete();
                    }

                    _tweenInfos.RemoveAt(i);
                }
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Enums
        //////////////////////////////////////////////////////////////////

        private enum EaseType
        {
            Linear      = 0,
            EaseInQuad  = 1,
            EaseOutQuad = 2,
            EaseInExpo  = 3,
            EaseOutExpo = 4,
        }


        //////////////////////////////////////////////////////////////////
        // Private Compound Types
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Class providing very basic tweening support.
        /// </summary>
        private class TweenInfo
        {
            public Action<float> OnUpdate { get; private set; }
            public Action OnComplete { get; private set; }
            public float Delay { get; set; }
            public float Duration { get; set; }
            public float Time { get; set; }
            public EaseType Ease { get; set; }

            public TweenInfo()
            {
                this.OnUpdate = null;
                this.OnComplete = null;
                this.Delay = 0.0f;
                this.Duration = 0.0f;
                this.Time = 0.0f;
                this.Ease = EaseType.Linear;
            }

            public TweenInfo(Action<float> onUpdate)
            {
                this.OnUpdate = onUpdate;
                this.OnComplete = null;
                this.Delay = 0.0f;
                this.Duration = 0.0f;
                this.Time = 0.0f;
                this.Ease = EaseType.Linear;
            }

            public TweenInfo SetDelay(float delay)
            {
                this.Delay = delay;
                return this;
            }

            public TweenInfo SetTime(float time)
            {
                this.Time = time;
                return this;
            }

            public TweenInfo SetDuration(float duration)
            {
                this.Duration = duration;
                return this;
            }

            public TweenInfo SetOnComplete(Action onComplete)
            {
                this.OnComplete = onComplete;
                return this;
            }

            public TweenInfo SetEase(EaseType ease)
            {
                this.Ease = ease;
                return this;
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private ZCore _zCore = null;

        private List<TweenInfo> _tweenInfos = new List<TweenInfo>();
        private TweenInfo _cameraTweenInfo  = null;

        private string _zoomInTextField          = "2.0";
        private string _zoomOutTextField         = "2.0";
        private string _zoomInAbsoluteTextField  = "4.0";
        private string _zoomOutAbsoluteTextField = "4.0";
    }
}
