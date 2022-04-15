using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Hand : MonoBehaviour
{
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float rotateSpeed = 100f;

    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    [SerializeField] private Transform palm;
    [SerializeField] float reachDistance = 0.1f, joinDistance = 0.05f;
    [SerializeField] private LayerMask grabbableLayer;

    private Transform _followTarget;
    private Rigidbody _body;

    private bool _isGrabbing;
    private GameObject _heldObject;
    private Transform _grabPoint;
    private FixedJoint _joint1, _joint2;

    private void Start()
    {
        _followTarget = controller.gameObject.transform;
        _body = GetComponent<Rigidbody>();
        _body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _body.mass = 20f;
        _body.maxAngularVelocity = 20f;

        controller.selectAction.action.started += Grab;
        controller.selectAction.action.canceled += Release;

        _body.position = _followTarget.position;
        _body.rotation = _followTarget.rotation;
    }

    private void Update()
    {
        PhysicsMove();
    }

    private void PhysicsMove()
    {
        var positionWithOffset = _followTarget.TransformPoint(positionOffset);
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        _body.velocity = (_followTarget.position - transform.position).normalized * followSpeed * distance;

        var rotationWithOffset = _followTarget.rotation * Quaternion.Euler(rotationOffset);
        var q = _followTarget.rotation * Quaternion.Inverse(_body.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        _body.angularVelocity = angle * axis * Mathf.Deg2Rad * rotateSpeed;
    }

    private void Release(InputAction.CallbackContext obj)
    {
        if (_joint1 != null)
            Destroy(_joint1);
        if (_joint2 != null)
            Destroy(_joint2);
        if (_grabPoint != null)
            Destroy(_grabPoint.gameObject);

        if (_heldObject != null)
        {
            var targetBody = _heldObject.GetComponent<Rigidbody>();
            targetBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            targetBody.interpolation = RigidbodyInterpolation.None;
            targetBody.isKinematic = true;
            _heldObject = null;
        }

        _isGrabbing = false;
        _followTarget = controller.gameObject.transform;
    }

    private void Grab(InputAction.CallbackContext obj)
    {
        if (_isGrabbing || _heldObject) return;

        Collider[] colliders = Physics.OverlapSphere(palm.position, reachDistance, grabbableLayer);
        if (colliders.Length < 1) return;

        var objectToGrab = colliders[0].transform.gameObject;

        var objectBody = objectToGrab.GetComponent<Rigidbody>();

        if (objectBody != null)
        {
            _heldObject = objectBody.gameObject;
        }
        else
        {
            objectBody = objectToGrab.GetComponentInParent<Rigidbody>();
            if (objectBody != null)
            {
                _heldObject = objectBody.gameObject;
            }
            else
            {
                return;
            }
        }

        StartCoroutine(GrabObject(colliders[0], objectBody));
    }

    private IEnumerator GrabObject(Collider collider, Rigidbody targetBody)
    {
        _isGrabbing = true;

        _grabPoint = new GameObject().transform;
        _grabPoint.position = collider.ClosestPoint(palm.position);
        _grabPoint.parent = _heldObject.transform;

        _followTarget = _grabPoint;

        while (Vector3.Distance(_grabPoint.position, palm.position) > joinDistance && _isGrabbing)
        {
            yield return new WaitForEndOfFrame();
        }

        _body.velocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;
        targetBody.velocity = Vector3.zero;
        targetBody.angularVelocity = Vector3.zero;

        targetBody.isKinematic = false;
        targetBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        targetBody.interpolation = RigidbodyInterpolation.Interpolate;

        _joint1 = gameObject.AddComponent<FixedJoint>();
        _joint1.connectedBody = targetBody;
        _joint1.breakForce = float.PositiveInfinity;
        _joint1.breakTorque = float.PositiveInfinity;

        _joint1.connectedMassScale = 1;
        _joint1.massScale = 1;
        _joint1.enableCollision = false;
        _joint1.enablePreprocessing = false;

        _joint2 = _heldObject.AddComponent<FixedJoint>();
        _joint2.connectedBody = _body;
        _joint2.breakForce = float.PositiveInfinity;
        _joint2.breakTorque = float.PositiveInfinity;

        _joint2.connectedMassScale = 1;
        _joint2.massScale = 1;
        _joint2.enableCollision = false;
        _joint2.enablePreprocessing = false;

        _followTarget = controller.gameObject.transform;
    }
}
