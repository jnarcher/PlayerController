using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleIndicatorRotate : MonoBehaviour
{
    public GameObject Player;
    public SpriteRenderer _sprite;
    public float SmoothTime = 0.2f;

    private PlayerGrapple _playerGrapple;

    private float _angle;

    private void Awake()
    {
        _playerGrapple = Player.GetComponent<PlayerGrapple>();
    }

    private void Update()
    {
        // This object isn't a parent of the player object so that it doesn't get flipped when the player turns.
        transform.position = Player.transform.position;

        if (_aimInput == Vector2.zero)
        {
            _sprite.enabled = false;
            return;
        }
        _sprite.enabled = true;

        GameObject snappedGrapplePoint = _playerGrapple.FindGrappleFromInput(_aimInput);

        float targetAngle;
        if (snappedGrapplePoint == null)
        {
            targetAngle = Mathf.Rad2Deg * Mathf.Atan2(_aimInput.y, _aimInput.x);
        }
        else
        {
            Vector2 dir = snappedGrapplePoint.transform.position - Player.transform.position;
            targetAngle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        }

        _angle = GetSmoothedAngle(targetAngle);

        transform.localEulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, _angle);
    }

    private float GetSmoothedAngle(float targetAngle)
    {
        // TODO: figure out how to smooth around a circle dealing with the discontinuity at 180 and -180
        return targetAngle;
    }

    private Vector2 _aimInput;

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 ipt = context.ReadValue<Vector2>();
            _aimInput = ipt;
        }
        else if (context.canceled)
        {
            transform.localEulerAngles = Vector3.zero;
            _aimInput = Vector2.zero;
        }
    }
}
