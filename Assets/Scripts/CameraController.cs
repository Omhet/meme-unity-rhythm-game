using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    static public CameraController Instance;

    public List<CinemachineVirtualCamera> virtualCameras;
    private int currentActiveCameraIndex = -1; // -1 indicates no camera is currently set as active

    private void Start()
    {
        Instance = this;
    }

    public void SetActiveCamera(int cameraIndex)
    {
        if (cameraIndex < 0 || cameraIndex >= virtualCameras.Count)
        {
            Debug.LogError("CameraController: Invalid camera index.");
            return;
        }

        // Deactivate all cameras
        foreach (var cam in virtualCameras)
        {
            if (cam != null)
                cam.Priority = 0;
        }

        // Activate the selected camera
        if (virtualCameras[cameraIndex] != null)
        {
            virtualCameras[cameraIndex].Priority = 10; // Set a priority higher than the inactive state
            currentActiveCameraIndex = cameraIndex;
        }
    }

    public void SwitchToNextCamera()
    {
        int nextCameraIndex = (currentActiveCameraIndex + 1) % virtualCameras.Count;
        SetActiveCamera(nextCameraIndex);
    }
}
