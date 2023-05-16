using UnityEngine;

namespace BCLib
{
    public static class Lib
    {
        public static bool NearZero(this float f)
        {
            return Mathf.Abs(f) < 0.0001;
        }

        public static float Lerp(this float t, float t0, float t1, float x0, float x1)
        {
            return x0 + (x1 - x0) * (t - t0) / (t1 - t0);
        }

        public static float NormalizeAngle(this float angle)
        {
            var ang = angle;
            while (ang > 180.0f)
                ang -= 360.0f;
            while (ang <= -180.0f)
                ang += 360.0f;
            return ang;
        }

        public static float GetAngleDegrees(this Vector2 val)
        {
            var v = val.normalized;
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }
    }
}
