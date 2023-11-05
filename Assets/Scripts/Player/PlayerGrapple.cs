using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerGrapple : MonoBehaviour
{
    private List<GameObject> _activeGrapplePoints = new();

    private PlayerStats _stats;

    private void Awake()
    {
        _stats = GetComponent<PlayerController>().Stats;
    }

    public GameObject FindGrappleFromInput(Vector2 aimInput)
    {
        foreach (var point in _activeGrapplePoints)
        {
            // get angle of grapple point from player
            Vector2 pointDirection = point.transform.position - transform.position;
            float grapplePointAngle = Mathf.Atan2(pointDirection.y, pointDirection.x);

            // get angle of aim input
            float aimAngle = Mathf.Atan2(aimInput.y, aimInput.x);

            float difference = Mathf.Rad2Deg * Mathf.Abs(aimAngle - grapplePointAngle);
            if (difference <= _stats.GrappleAssistAngle)
                return point;
        }
        return null;
    }

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
