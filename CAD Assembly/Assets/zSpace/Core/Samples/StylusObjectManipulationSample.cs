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
    public class StylusObjectManipulationSample : MonoBehaviour
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

            //// Create the stylus beam's GameObject.
            //_stylusBeamObject = new GameObject("StylusBeam");
            //_stylusBeamRenderer = _stylusBeamObject.AddComponent<MeshRenderer>();
            //_stylusBeamRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

            _stylusBeamObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _stylusBeamObject.name = "Stylus Beam";

            // Disable shadows from and on the beam
            MeshRenderer beamRenderer = _stylusBeamObject.GetComponent<MeshRenderer>();
            beamRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            beamRenderer.receiveShadows = false;

            // Disable beam collision
            Collider beamCollider = _stylusBeamObject.GetComponent<Collider>();
            beamCollider.enabled = false;

            // Scrollbar
            GameObject scrollBar = GameObject.Find("_Scrollbar");
            minScrollBarPos = scrollBar.GetComponent<Scrolling>().getMinScroll();
            maxScrollBarPos = scrollBar.GetComponent<Scrolling>().getMaxScroll();


#if UNITY_5_4
            _stylusBeamRenderer.SetColors(Color.white, Color.white);
#else
            // TODO: Change Color
            //_stylusBeamObject.material.color = Color.white;

            //_stylusBeamRenderer.startColor = Color.white; // LineRenderer
            //_stylusBeamRenderer.endColor = Color.white; 
#endif
        }

        void Update()
        {
            // Grab the latest stylus pose and button state information.
            ZCore.Pose pose = _zCore.GetTargetPose(ZCore.TargetType.Primary, ZCore.CoordinateSpace.World);
            bool isButtonPressed = _zCore.IsTargetButtonPressed(ZCore.TargetType.Primary, 0);

            switch (_stylusState)
            {
                case StylusState.Idle:
                    {
                        _stylusBeamLength = DEFAULT_STYLUS_BEAM_LENGTH;

                        // Perform a raycast on the entire scene to determine what the
                        // stylus is currently colliding with.
                        // Ignore the floor. 
                        RaycastHit hit;
                        if (Physics.Raycast(pose.Position, pose.Direction, out hit))
                        {
                            // Update the stylus beam length.
                            _stylusBeamLength = hit.distance / _zCore.ViewerScale;

                            // If the front stylus button was pressed, initiate a grab.
                            if (isButtonPressed && !_wasButtonPressed && hit.collider.gameObject.ToString() != "_Floor (UnityEngine.GameObject)") // TODO: Make this suck less? Do by ID?
                            {
                                // Begin the grab.
                                this.BeginGrab(hit.collider.gameObject, hit.distance, pose.Position, pose.Rotation);

                                _stylusState = StylusState.Grab;
                            }
                        }
                    }
                    break;

                case StylusState.Grab:
                    {
                        if (_grabObject.name == "_Scrollbar")
                        {
                            Vector3 inputPosition = pose.Position;
                            Quaternion inputRotation = pose.Rotation;

                            // Update the grab. TODO: Make this suck less
                            Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * _initialGrabDistance));
                            Quaternion objectRotation = inputRotation * _initialGrabRotation;

                            // Update the grab object's position. Only change the x component.
                            Vector3 barPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
                            barPosition.y = _grabObject.transform.position.y;
                            barPosition.z = _grabObject.transform.position.z;

                            // Cap horizontal position
                            if (barPosition.x < minScrollBarPos)
                            {
                                barPosition.x = minScrollBarPos;
                            }
                            else if (maxScrollBarPos < barPosition.x)
                            {
                                barPosition.x = maxScrollBarPos;
                            }

                            _grabObject.transform.position = barPosition;
                        }
                        else // Assembly item
                        {
                            Quaternion axisRotation = Quaternion.identity; // For axial snapping

                            // Update the grab.
                            this.UpdateGrab(pose.Position, pose.Rotation);

                            // Vibrate on collision
                            if (_grabObject.GetComponent<GrabbedItem>().isTriggered)
                                _zCore.StartTargetVibration(ZCore.TargetType.Primary, 0.02f, 0.02f, 10, 1f);


                            // Get the ghost object form the grab object
                            _ghostObject = _grabObject.GetComponent<Reference>().GetGhostObject();

                            // Check if the object is close to the ghost's origin
                            if (Vector3.Distance(_grabObject.transform.position, _ghostObject.transform.position) < 0.01f)
                            {
                                _grabObject.transform.position = _ghostObject.transform.position;
                            }

                            float axisThreshold = 10f;

                            if ((Vector3.Angle(_grabObject.transform.up, _ghostObject.transform.up)) < axisThreshold)
                            {
                                if ((Vector3.Angle(_grabObject.transform.up, _ghostObject.transform.up)) > .001f)
                                {
                                    _grabObject.transform.RotateAround(_grabObject.transform.position, Vector3.Cross(_grabObject.transform.up, _ghostObject.transform.up), Vector3.Angle(_grabObject.transform.up, _ghostObject.transform.up));
                                }
                            }

                            if ((Vector3.Angle(_grabObject.transform.right, _ghostObject.transform.right)) < axisThreshold)
                            {
                                if ((Vector3.Angle(_grabObject.transform.right, _ghostObject.transform.right)) > .001f)
                                {
                                    _grabObject.transform.RotateAround(_grabObject.transform.position, Vector3.Cross(_grabObject.transform.right, _ghostObject.transform.right), Vector3.Angle(_grabObject.transform.right, _ghostObject.transform.right));
                                }
                            }

                            if ((Vector3.Angle(_grabObject.transform.forward, _ghostObject.transform.forward)) < axisThreshold)
                            {
                                if ((Vector3.Angle(_grabObject.transform.forward, _ghostObject.transform.forward)) > .001f)
                                {
                                    _grabObject.transform.RotateAround(_grabObject.transform.position, Vector3.Cross(_grabObject.transform.forward, _ghostObject.transform.forward), Vector3.Angle(_grabObject.transform.forward, _ghostObject.transform.forward));
                                }
                            }


                            _grabObject.transform.rotation *= axisRotation;
                        }

                        // End the grab if the front stylus button was released.
                        if (!isButtonPressed && _wasButtonPressed)
                        {
                            _stylusState = StylusState.Idle;
                        }
                    }
                    break;

                default:
                    break;
            }

            // Update the stylus beam.
            this.UpdateStylusBeam(pose.Position, pose.Direction);

            // Cache state for next frame.
            _wasButtonPressed = isButtonPressed;
        }


        //////////////////////////////////////////////////////////////////
        // Private Helpers
        //////////////////////////////////////////////////////////////////

        private void BeginGrab(GameObject hitObject, float hitDistance, Vector3 inputPosition, Quaternion inputRotation)
        {
            Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * hitDistance));
            //ghostObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            if (hitObject.name != "_Scrollbar")
            {
                hitObject.transform.localScale = new Vector3(1f, 1f, 1f);
                hitObject.transform.parent = null;
            }
            // Cache the initial grab state.
            _grabObject = hitObject;
            _initialGrabOffset = Quaternion.Inverse(hitObject.transform.rotation) * (hitObject.transform.position - inputEndPosition);
            _initialGrabRotation = Quaternion.Inverse(inputRotation) * hitObject.transform.rotation;
            _initialGrabDistance = hitDistance;
        }

        private void UpdateGrab(Vector3 inputPosition, Quaternion inputRotation)
        {
            Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * _initialGrabDistance));

            // Update the grab object's rotation.
            Quaternion objectRotation = inputRotation * _initialGrabRotation;
            _grabObject.transform.rotation = objectRotation;

            // Update the grab object's position.
            Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
            _grabObject.transform.position = objectPosition;
        }

        private void UpdateStylusBeam(Vector3 stylusPosition, Vector3 stylusDirection)
        {
            if (_stylusBeamObject != null)
            {
                float stylusBeamWidth = DEFAULT_STYLUS_BEAM_WIDTH * _zCore.ViewerScale;
                float stylusBeamLength = _stylusBeamLength * _zCore.ViewerScale;

                //#if UNITY_5_4
                //                _stylusBeamRenderer.SetWidth(stylusBeamWidth, stylusBeamWidth);
                //#else
                //                _stylusBeamRenderer.startWidth = stylusBeamWidth;
                //                _stylusBeamRenderer.endWidth = stylusBeamWidth;
                //#endif
                //                _stylusBeamRenderer.SetPosition(0, stylusPosition);
                //                _stylusBeamRenderer.SetPosition(1, stylusPosition + (stylusDirection * stylusBeamLength));

                _stylusBeamObject.transform.position = stylusPosition;
                _stylusBeamObject.transform.localScale = new Vector3(stylusBeamWidth, stylusBeamLength, stylusBeamWidth);

                _stylusOrientation.SetFromToRotation(Vector3.up, stylusDirection);
                _stylusBeamObject.transform.rotation = _stylusOrientation;


            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Enumerations
        //////////////////////////////////////////////////////////////////

        private enum StylusState
        {
            Idle = 0,
            Grab = 1,
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private static readonly float DEFAULT_STYLUS_BEAM_WIDTH = 0.0002f;
        private static readonly float DEFAULT_STYLUS_BEAM_LENGTH = 0.3f;

        private ZCore _zCore = null;
        private bool _wasButtonPressed = false;

        private GameObject _stylusBeamObject = null;
        private float _stylusBeamLength = DEFAULT_STYLUS_BEAM_LENGTH;
        private Quaternion _stylusOrientation;

        private StylusState _stylusState = StylusState.Idle;
        private GameObject _grabObject = null;
        private Vector3 _initialGrabOffset = Vector3.zero;
        private Quaternion _initialGrabRotation = Quaternion.identity;
        private float _initialGrabDistance = 0.0f;

        private GameObject _ghostObject = null;

        private float minScrollBarPos;
        private float maxScrollBarPos;
    }
}