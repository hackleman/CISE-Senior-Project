using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Surface: System.IDisposable {

    NativeArray<ControlPoint> cps;
    int order, olx, oly, nlx, nly;
    int nidx(int x, int y) => x + y * nlx;
    int oidx(int x, int y) => x + y * olx;
    public NativeArray<ControlPoint> CPs => cps;

    public Surface(List<ControlPoint> originalCps, int order, int olx, int oly) 
    {
        this.olx = olx;
        this.oly = oly;
        this.order = order;

        nlx = olx + 2 * order;
        nly = oly + 2 * order;

        cps = new NativeArray<ControlPoint>(nlx * nly, Allocator.Persistent);

        for (int y = 0; y < order; y++)
        {
            for (int x = 0; x < order; x++) 
            {
                var ix = nlx - 1 - x;
                var iy = nly - 1 - y;
                cps[nidx(x, y)]   = originalCps[oidx(0, 0)];
                cps[nidx(ix, y)]  = originalCps[oidx(olx - 1, 0)];
                cps[nidx(x, iy)]  = originalCps[oidx(0, oly - 1)];
                cps[nidx(ix, iy)] = originalCps[oidx(olx - 1, oly - 1)];
            }
        }

        for (int y = 0; y < oly; y++) 
        {
            for (int x = 0; x < order; x++) 
            {
                var yo = y + order;
                var ix = nlx - 1 - x;
                cps[nidx(x, yo)]  = originalCps[oidx(0, y)];
                cps[nidx(ix, yo)] = originalCps[oidx(olx - 1, y)];
            }
        }

        for (int x = 0; x < olx; x++) 
        {
            for (int y = 0; y < order; y++) 
            {
                var xo = x + order;
                var iy = nly - 1 - y;
                cps[nidx(xo, y)]  = originalCps[oidx(x, 0)];
                cps[nidx(xo, iy)] = originalCps[oidx(x, oly - 1)];
            }
        }

        for (int y = 0; y < oly; y++)
        {
            for (int x = 0; x < olx; x++) 
            {
                cps[nidx(x + order, y + order)] = originalCps[oidx(x, y)];
            }
        }

    }

    public void UpdateControlPoint(Vector2Int i, ControlPoint cp) 
    {
        var cl = new Vector2Int(nlx, nly);
        var ol = cl - order * 2 * Vector2.one;
        
        if (i.x == 0 && i.y == 0) 
        {
            for (int x = 0; x <= order; x++) 
            {
                for (int y = 0; y <= order; y++) 
                {
                    cps[nidx(x, y)] = cp;
                }
            }
        }
        else if (i.x == 0 && i.y == ol.y - 1) 
        {
            for (int x = 0; x <= order; x++) 
            {
                for (int y = 0; y <= order; y++)
                {
                    cps[nidx(x, cl.y - y - 1)] = cp;
                } 
            }
        }      
        else if (i.x == ol.x - 1 && i.y == 0) 
        {
            for (int x = 0; x <= order; x++) 
            {
                for (int y = 0; y <= order; y++) 
                {
                    cps[nidx(cl.x - x - 1, y)] = cp;
                }
            }
        }       
        else if (i.x == ol.x - 1 && i.y == ol.y - 1)
        {
            for (int x = 0; x <= order; x++) 
            {
                for (int y = 0; y <= order; y++) 
                {
                    cps[nidx(cl.x - x - 1, cl.y - y - 1)] = cp;
                }
            }
        } 
        else if (i.x == 0) 
        {
            for (int x = 0; x <= order; x++) 
            {
                cps[nidx(x, i.y + order)] = cp;
            }
        }       
        else if (i.y == 0)  
        {
            for (int y = 0; y <= order; y++) 
            {
                cps[nidx(i.x + order, y)] = cp;
            }
        }      
        else if (i.x == ol.x - 1) 
        {
            for (int j = 0; j <= order; j++) 
            {
                cps[nidx(cl.x - j - 1, i.y + order)] = cp;
            }
        }
        else if (i.y == ol.y - 1) 
        {
            for (int j = 0; j <= order; j++) 
            {
                cps[nidx(i.x + order, cl.y - j - 1)] = cp;
            }
        }
        else 
        {
            cps[nidx(i.x + order, i.y + order)] = cp;
        }
    }

    public Vector3 GetCurve(float tx, float ty) => SurfaceHelper.GetCurve(cps, tx, ty, order, olx, oly);
    public void Dispose() => cps.Dispose();
}

