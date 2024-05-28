using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCamera;
    public CinemachineVirtualCamera thirdPersonCamera;
    private bool isThirdPerson = false;

    void Start()
    {
        // 초기 상태를 1인칭 카메라로 설정
        firstPersonCamera.Priority = 10;
        thirdPersonCamera.Priority = 5;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        if (isThirdPerson)
        {
            firstPersonCamera.Priority = 10;
            thirdPersonCamera.Priority = 5;
        }
        else
        {
            firstPersonCamera.Priority = 5;
            thirdPersonCamera.Priority = 10;
        }
        isThirdPerson = !isThirdPerson;
    }
}
