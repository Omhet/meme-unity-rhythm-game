using UnityEngine;
using Cinemachine;

public class CameraPulse : MonoBehaviour
{
    [SerializeField] private float pulseFOV = 60f; // Target FOV for the pulse
    [SerializeField] private float returnSpeed = 5f; // Speed at which FOV returns to normal
    private CinemachineVirtualCamera virtualCamera;
    private float startFOV;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        startFOV = virtualCamera.m_Lens.FieldOfView;
    }

    void Update()
    {
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, startFOV, Time.deltaTime * returnSpeed);
    }

    public void Pulse()
    {
        virtualCamera.m_Lens.FieldOfView = pulseFOV;
    }
}
