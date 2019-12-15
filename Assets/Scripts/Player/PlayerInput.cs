using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PlayerInput
{
    public static float X;
    public static float Y;
    public static Vector2 XY;
    public static Vector2 XYNormalized;
    public static Vector3 XYZ;
    public static Vector3 XYZNormalized;

    public static bool Jump;
    public static bool Crouch;
    public static bool Walk;
    public static bool Interact;
    public static bool Hide;
    public static bool UsePowder;

    public static Vector2 MouseVector;

    public static bool PowderWheel;
}