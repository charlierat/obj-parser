using System;

/// <summary>
/// Axis-Aligned Bounding Box for spatial acceleration structures.
/// Used in BVH construction to quickly reject rays that don't intersect a region.
/// </summary>
public class AABB
{
    public Vector Min { get; set; }
    public Vector Max { get; set; }

    public AABB()
    {
        Min = new Vector(double.MaxValue, double.MaxValue, double.MaxValue);
        Max = new Vector(double.MinValue, double.MinValue, double.MinValue);
    }

    /// <summary>
    /// Expands this bounding box to include a point.
    /// </summary>
    public void ExpandToInclude(Vector point)
    {
        Min = new Vector(
            Math.Min(Min.X, point.X),
            Math.Min(Min.Y, point.Y),
            Math.Min(Min.Z, point.Z)
        );

        Max = new Vector(
            Math.Max(Max.X, point.X),
            Math.Max(Max.Y, point.Y),
            Math.Max(Max.Z, point.Z)
        );
    }

    /// <summary>
    /// Expands this bounding box to include another bounding box.
    /// </summary>
    public void ExpandToInclude(AABB other)
    {
        ExpandToInclude(other.Min);
        ExpandToInclude(other.Max);
    }

    /// <summary>
    /// Fast ray-box intersection test using slab method.
    /// Returns true if the ray intersects this bounding box.
    /// </summary>
    public bool Intersect(Ray ray)
    {
        double tMin = 0;
        double tMax = double.MaxValue;

        if (Math.Abs(ray.Direction.X) > 1e-8)
        {
            double t1 = (Min.X - ray.Origin.X) / ray.Direction.X;
            double t2 = (Max.X - ray.Origin.X) / ray.Direction.X;

            tMin = Math.Max(tMin, Math.Min(t1, t2));
            tMax = Math.Min(tMax, Math.Max(t1, t2));
        }
        else
        {
            if (ray.Origin.X < Min.X || ray.Origin.X > Max.X)
                return false;
        }

        if (Math.Abs(ray.Direction.Y) > 1e-8)
        {
            double t1 = (Min.Y - ray.Origin.Y) / ray.Direction.Y;
            double t2 = (Max.Y - ray.Origin.Y) / ray.Direction.Y;

            tMin = Math.Max(tMin, Math.Min(t1, t2));
            tMax = Math.Min(tMax, Math.Max(t1, t2));
        }
        else
        {
            if (ray.Origin.Y < Min.Y || ray.Origin.Y > Max.Y)
                return false;
        }

        if (Math.Abs(ray.Direction.Z) > 1e-8)
        {
            double t1 = (Min.Z - ray.Origin.Z) / ray.Direction.Z;
            double t2 = (Max.Z - ray.Origin.Z) / ray.Direction.Z;

            tMin = Math.Max(tMin, Math.Min(t1, t2));
            tMax = Math.Min(tMax, Math.Max(t1, t2));
        }
        else
        {
            if (ray.Origin.Z < Min.Z || ray.Origin.Z > Max.Z)
                return false;
        }

        return tMax >= tMin && tMax > 0;
    }

    /// <summary>
    /// Returns which axis is longest (0=X, 1=Y, 2=Z).
    /// </summary>
    public int LongestAxis()
    {
        double dx = Max.X - Min.X;
        double dy = Max.Y - Min.Y;
        double dz = Max.Z - Min.Z;

        if (dx > dy && dx > dz) return 0;
        if (dy > dz) return 1;
        return 2;
    }
}