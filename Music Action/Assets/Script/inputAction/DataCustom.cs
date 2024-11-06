using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCustom : MonoBehaviour
{
    public static float CustomData(float data, float Sensitive)
    {
        return (data <= 1f && data >= -1f) ? data * Sensitive : data / Sensitive;
    }

    public static float CheckAngle(float angle)
    {
        float ret = angle - 180;
        return ret > 0 ? ret - 180 : ret + 180;
    }

    public static float LerpForFixMouseDelta(float start, float end, float t)
    {
        return (1 - t) * start + t * end;
    }
}
