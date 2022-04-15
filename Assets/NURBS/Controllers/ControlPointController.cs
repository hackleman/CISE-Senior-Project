using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using Unity.Collections;

public class ControlPointController : MonoBehaviour {
    private Vector3 mouseOffset;
    private Vector3 lastPosition;
    private float mouseZCoordinate;
    private SurfaceController surface;
    private int index;

    void Start()
    {
        surface = transform.parent.gameObject.GetComponent<SurfaceController>();
    }

    void Update() 
    {
        if (transform.position != this.lastPosition) 
        {
            var cps = surface.Data.cps;
            var cp = cps[index];
            var wp = surface.transform.TransformPoint(cp.pos);

            surface.surface.UpdateControlPoint(
                new Vector2Int(index % surface.Data.count.x, Mathf.FloorToInt(index / (float)surface.Data.count.x)), 
                new ControlPoint(surface.transform.position + cp.pos, cp.weight)
            );

            cp.pos = surface.transform.InverseTransformPoint(transform.position);
            cps[index] = cp;
            
            surface.UpdateMesh();
        }

        this.lastPosition = transform.position;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    void OnMouseDown() 
    {
        mouseZCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        mouseOffset = transform.position - getMouseWorldPosition();
    }

    void OnMouseDrag() 
    {
        transform.position = getMouseWorldPosition() + mouseOffset;
    }

    private Vector3 getMouseWorldPosition() 
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZCoordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
