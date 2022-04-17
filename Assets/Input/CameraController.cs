using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Positioning")]
    public Vector2 cameraOffset = new Vector2(0f, 0f);
    public float lookAtOffset = 0f;

    [Header("Move Controls")]
    public float inOutSpeed = 5f;
    public float lateralSpeed = 5f;
    public float rotateSpeed = 45f;

    [Header("Move Bounds")]
    public Vector2 minBounds, maxBounds;

    Vector3 frameMove;
    float frameRotate;
    Camera cam;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(0f, Mathf.Abs(cameraOffset.y), -Mathf.Abs(cameraOffset.x));

        cam.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
    }

    private void OnEnable()
    {
        //KeyboardController.onMoveInput += UpdateFrameMove;
        //KeyboardController.onRotateInput += UpdateFrameRotate;
    }

    private void OnDisable()
    {
        //KeyboardController.onMoveInput -= UpdateFrameMove;
        //KeyboardController.onRotateInput -= UpdateFrameRotate;
    }

    private void UpdateFrameMove(Vector3 movement)
    {
        frameMove += movement;
    }

    private void UpdateFrameRotate(float rotation)
    {
        frameRotate += rotation;
    }

    private void LateUpdate()
    {
        if (frameMove != Vector3.zero)
        {
            Vector3 speedModFrameMove = new Vector3(frameMove.x * lateralSpeed, frameMove.y * lateralSpeed, frameMove.z * lateralSpeed);
            transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
            LockPositionInBounds();
            frameMove = Vector3.zero;
        }

        if (frameRotate != 0f)
        {
            transform.Rotate(Vector3.up, frameRotate * Time.deltaTime * rotateSpeed);
            frameRotate = 0f;
        }
    }

    private void LockPositionInBounds()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y));
    }
}
