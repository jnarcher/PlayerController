using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbCameraFollow : MonoBehaviour
{
    public Transform Player;

    private float _camZ;

    private void Start() => _camZ = transform.position.z;

    private void Update()
    {
        transform.position = new Vector3(Player.position.x, Player.position.y, _camZ);
    }
}
