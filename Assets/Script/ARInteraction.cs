using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Experimental.XR;

public class ARInteraction : MonoBehaviour
{

    public Camera camera;
    public ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void UpdateLayers()
    {
        if (placementPoseIsValid)
        {
            gameObject.SetActive(true);
            transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            gameObject.SetActive(false);
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

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdateLayers();
    }
}
