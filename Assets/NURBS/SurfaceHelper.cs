using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class SurfaceHelper {
    public static Vector3 GetCurve(NativeArray<ControlPoint> cps, float tx, float ty, int order, int olx, int oly) 
    {
        var frac = Vector3.zero;
        var deno = 0f;
        var nlx = olx + 2 * order;
        var nly = oly + 2 * order;

        tx = Mathf.Min(tx, 1f - 1e-5f);
        ty = Mathf.Min(ty, 1f - 1e-5f);

        for (int y = 0; y < nly; y++) 
        {
            for (int x = 0; x < nlx; x++) 
            {
                var bf = BasisFunc(x, order, order, tx, nlx) * BasisFunc(y, order, order, ty, nly);
                var cp = cps[x + y * nlx];
                frac += cp.pos * bf * cp.weight;
                deno += bf * cp.weight;
            }
        }

        return frac / deno;
    }

    public static float BasisFunc(int j, int k, int order, float t, int l) 
    {
        if (k == 0) 
        {
            return (t >= KnotVector(j, order, l) && t < KnotVector(j + 1, order, l)) ? 1 : 0;
        }
        else 
        {
            var d1 = KnotVector(j + k, order, l) - KnotVector(j, order, l);
            var d2 = KnotVector(j + k + 1, order, l) - KnotVector(j + 1, order, l);
            var c1 = d1 != 0 ? (t - KnotVector(j, order, l)) / d1 : 0;
            var c2 = d2 != 0 ? (KnotVector(j + k + 1, order, l) - t) / d2 : 0;
            return c1 * BasisFunc(j, k - 1, order, t, l) + c2 * BasisFunc(j + 1, k - 1, order, t, l);
        }
    }

    static float KnotVector(int j, int order, int len) 
    {
        if (j < order) return 0;
        if (j > len - order) return 1;
        return Mathf.Clamp01((j - order) / (float)(len - 2 * order));
    }
}
