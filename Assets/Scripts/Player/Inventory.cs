using UnityEngine;

[CreateAssetMenu(menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    [Header("Collectables")]
    public int Currency = 0;
    public int Keys = 0;

    [Header("Abilities")]
    public bool Slide = false;
    public bool AirLaunch = false;
    public bool AirDash = false;
    public bool WallSlideAndJump = false;
    public bool Grapple = false;
    public int AirJumps = 0;
}
