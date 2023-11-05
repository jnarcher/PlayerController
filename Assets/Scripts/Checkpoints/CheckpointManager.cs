using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private GameObject _player;

    public List<GameObject> _activeCheckpoints = new();

    private GameObject _lastCheckpoint;

    public Mode CheckpointMode;
    public enum Mode
    {
        Nearest,
        Last
    }

    private void Awake()
    {
        // set up singleton
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private GameObject GetNearestActiveCheckpoint()
    {
        if (_activeCheckpoints.Count == 0)
        {
            Debug.LogWarning("No active checkpoints found.");
            return null;
        }

        float closestDist = float.MaxValue;
        GameObject closestCheckpoint = null;

        foreach (var cp in _activeCheckpoints)
        {
            float dist = Vector2.Distance(_player.transform.position, cp.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestCheckpoint = cp;
            }
        }

        return closestCheckpoint;
    }

    public GameObject GetRespawnCheckpoint()
    {
        return CheckpointMode switch
        {
            Mode.Nearest => GetNearestActiveCheckpoint(),
            Mode.Last => _activeCheckpoints.Last(),
            _ => null,
        };
    }

    public void AddActiveCheckpoint(GameObject checkpoint)
    {
        if (_activeCheckpoints.Contains(checkpoint)) return;
        _activeCheckpoints.Add(checkpoint);
    }

    public void RemoveActiveCheckpoint(GameObject checkpoint)
    {
        if (!_activeCheckpoints.Contains(checkpoint)) return;
        _activeCheckpoints.Remove(checkpoint);
    }
}
