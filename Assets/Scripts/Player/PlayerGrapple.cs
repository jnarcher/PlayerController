using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    private List<GameObject> _activeGrapplePoints = new();

    public void AddActiveGrapplePoint(GameObject grapplePoint)
    {
        if (_activeGrapplePoints.Contains(grapplePoint)) return;
        _activeGrapplePoints.Add(grapplePoint);
    }

    public void RemoveActiveGrapplePoint(GameObject grapplePoint)
    {
        if (!_activeGrapplePoints.Contains(grapplePoint)) return;
        _activeGrapplePoints.Remove(grapplePoint);
    }
}
