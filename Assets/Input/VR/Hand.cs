using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Hand : MonoBehaviour
{
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    [SerializeField] private Transform palm;
    [SerializeField] float reachDistance = 0.025f, joinDistance = 0.025f;
    [SerializeField] private LayerMask grabbableLayer;

    private Transform _followTarget;
    private Rigidbody _body;

    private bool _isGrabbing;
    private List<GameObject> _heldObjects = new List<GameObject>();
    private Transform _grabPoint;
    private List<FixedJoint> joints = new List<FixedJoint>();

    Animator animator;
    private float triggerTarget;
    private float triggerCurrent;
    public float animationSpeed;

    private void Start()
    {
        animator = GetComponent<Animator>();

        _followTarget = controller.gameObject.transform;
        _body = GetComponent<Rigidbody>();
        _body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _body.mass = 20f;
        _body.maxAngularVelocity = 20f;

        controller.activateAction.action.started += Grab;
        controller.activateAction.action.canceled += Release;

        _body.position = _followTarget.position;
        _body.rotation = _followTarget.rotation;
    }

    private void Update()
    {
        PhysicsMove();
        AnimateHand();
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

    void AnimateHand()
    {
        triggerTarget = controller.activateAction.action.ReadValue<float>();

        if (triggerCurrent != triggerTarget)
        {
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * animationSpeed);
            animator.SetFloat("Trigger", triggerCurrent);
        }
    }

    private void Release(InputAction.CallbackContext obj)
    {
        foreach (FixedJoint joint in joints)
        {
            Destroy(joint);
        }
        if (_grabPoint != null)
            Destroy(_grabPoint.gameObject);

        for (int i = 0; i < _heldObjects.Count(); i++)
        {
            if (_heldObjects[i] != null)
            {
                var targetBody = _heldObjects[i].GetComponent<Rigidbody>();
                targetBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                targetBody.interpolation = RigidbodyInterpolation.None;
                targetBody.isKinematic = true;
            }
        }

        _heldObjects = new List<GameObject>();

        _isGrabbing = false;
        _followTarget = controller.gameObject.transform;
    }

    private void Grab(InputAction.CallbackContext obj)
    {
        if (_isGrabbing || _heldObjects.Any()) return;

        Collider[] colliders = Physics.OverlapSphere(palm.position, reachDistance, grabbableLayer);
        if (colliders.Length < 1 || colliders.Length > 5) return;

        foreach (Collider collider in colliders)
        {
            var objectToGrab = collider.transform.gameObject;
            var objectBody = objectToGrab.GetComponent<Rigidbody>();

            if (objectBody != null)
            {
                _heldObjects.Add(objectBody.gameObject);
            }
            else
            {
                objectBody = objectToGrab.GetComponentInParent<Rigidbody>();
                if (objectBody != null)
                {
                    _heldObjects.Add(objectBody.gameObject);
                }
                else
                {
                    return;
                }
            }

            StartCoroutine(GrabObject(collider, objectBody, objectBody.gameObject));
        }
    }

    private IEnumerator GrabObject(Collider collider, Rigidbody targetBody, GameObject heldObject)
    {
        _isGrabbing = true;

        if (_grabPoint == null) 
        {
            _grabPoint = new GameObject().transform;
            _grabPoint.position = collider.ClosestPoint(palm.position);
            _grabPoint.parent = _heldObjects[0].transform;

            _followTarget = _grabPoint;
        }

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

        var joint1 = gameObject.AddComponent<FixedJoint>();
        joint1.connectedBody = targetBody;
        joint1.breakForce = float.PositiveInfinity;
        joint1.breakTorque = float.PositiveInfinity;

        joint1.connectedMassScale = 1;
        joint1.massScale = 1;
        joint1.enableCollision = false;
        joint1.enablePreprocessing = false;

        var joint2 = heldObject.AddComponent<FixedJoint>();
        joint2.connectedBody = _body;
        joint2.breakForce = float.PositiveInfinity;
        joint2.breakTorque = float.PositiveInfinity;

        joint2.connectedMassScale = 1;
        joint2.massScale = 1;
        joint2.enableCollision = false;
        joint2.enablePreprocessing = false;

        joints.Add(joint1);
        joints.Add(joint2);

        _followTarget = controller.gameObject.transform;
    }
}
