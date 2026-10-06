using UnityEngine;

public struct NormalizedFloat
{
    public static float NormalizeFloat(float value)
    {
        return value == 0 ? 0f : value / Mathf.Abs(value);
    }
}
