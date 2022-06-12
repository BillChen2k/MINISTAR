using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Experimental.XR;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject layers;
    public GameObject placementIndicator;
    public Camera camera;

    public ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    void Start()
    {

    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        Touchscreen touchScreen = Touchscreen.current;
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            if (placementPoseIsValid && mouse.leftButton.wasPressedThisFrame)
            {
                PlaceObject();
            }
        }
        else if (touchScreen != null)
        {
            ReadOnlyArray<TouchControl> touches = touchScreen.touches;
            if (placementPoseIsValid && touches.Count > 0 && touches[0].phase.ReadValue() == TouchPhase.Began)
            {
                PlaceObject();
            }
        }

    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        Instantiate(layers, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
