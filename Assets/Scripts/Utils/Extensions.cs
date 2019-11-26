using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector2Int RandomVec2(Vector2Int _min, Vector2Int _max)
    {
        return RandomVec2(_min.x, _max.x, _min.y, _max.y);
    }
    public static Vector2Int RandomVec2(int _minX, int _minY, Vector2Int _max)
    {
        return RandomVec2(_minX, _max.x, _minY, _max.y);
    }
    public static Vector2Int RandomVec2(Vector2Int _min, int _maxX, int _maxY)
    {
        return RandomVec2(_min.x, _maxX, _min.y, _maxY);
    }
    public static Vector2Int RandomVec2 (int _minX, int _minY, int _maxX, int _maxY)
    {
        return new Vector2Int(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY));
    }

    public static bool IsInRange(this int _value, int _min, int _max)
    {
        return Mathf.Clamp(_value, _min, _max) == _value;
    }
    public static bool IsInRange(this Vector2Int _value, Vector2Int _min, Vector2Int _max)
    {
        return (_value.x.IsInRange(_min.x, _max.x) && _value.y.IsInRange(_min.y, _max.y));
    }

    public static int Clamp(this int _value, int _min, int _max)
    {
        return Mathf.Clamp(_value, _min, _max);
    }
    public static int Clamp01(this int _value)
    {
        return _value.Clamp(0, 1);
    }
    public static float Clamp(this float _value, float _min, float _max)
    {
        return Mathf.Clamp(_value, _min, _max);
    }
    public static float Clamp01(this float _value)
    {
        return _value.Clamp(0.0f, 1.0f);
    }
}
