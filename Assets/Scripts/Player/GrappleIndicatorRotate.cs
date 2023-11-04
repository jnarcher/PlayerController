using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleIndicatorRotate : MonoBehaviour
{
    public Transform PlayerTransform;
    public SpriteRenderer _sprite;

    private void Update()
    {
        // This object isn't a parent of the player object so that it doesn't get flipped when the player turns.
        transform.position = PlayerTransform.position;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _sprite.enabled = true;
            Vector2 ipt = context.ReadValue<Vector2>();
            if (ipt == Vector2.zero) return;

            float angle = Mathf.Rad2Deg * Mathf.Atan2(ipt.y, ipt.x);
            transform.localEulerAngles = Vector3.zero;
            transform.localEulerAngles = new Vector3(transform.rotation.x, transform.rotation.z, angle);
        }
        else if (context.canceled)
        {
            transform.localEulerAngles = Vector3.zero;
            _sprite.enabled = false;
        }
    }
}
