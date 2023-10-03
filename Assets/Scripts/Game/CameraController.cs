using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform currentPosition;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCameraPosition;
    void Awake()
    {
        MainCamera.transform.SetPositionAndRotation(playerCameraPosition.position, playerCameraPosition.rotation);
        currentPosition = player;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, currentPosition.position.y, currentPosition.position.z);
    }
}
