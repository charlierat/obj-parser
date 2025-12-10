using System;
using System.Numerics;

/// <summary>
/// Represents a spherical shape in 3D space.
/// </summary>
/// <remarks>
/// A sphere is defined by its center position and radius.  
/// This class provides methods for ray-sphere intersection testing and normal calculation.
/// </remarks>
public class Sphere : Shape
{
    // The radius of the sphere.
    private float _r;

    /// <summary>
    /// Gets or sets the radius of the sphere.
    /// </summary>
    public float R
    {
        get { return _r; }
        set { _r = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sphere"/> class
    /// with a center at the origin and a radius of 0.
    /// </summary>
    public Sphere()
    {
        Center = new Vector(0, 0, 0);
        R = 0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sphere"/> class
    /// with the specified center position and radius.
    /// </summary>
    /// <param name="center">The center point of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    public Sphere(Vector center, float radius)
    {
        Center = new Vector(center.X, center.Y, center.Z);
        R = radius;
    }

    /// <summary>
    /// Determines whether the given ray intersects with the sphere.
    /// </summary>
    /// <param name="r">The ray to test for intersection.</param>
    /// <returns>
    /// The distance <c>t</c> from the ray origin to the intersection point.  
    /// Returns <see cref="float.PositiveInfinity"/> if there is no intersection.
    /// </returns>
    /// <remarks>
    /// Uses the quadratic formula to solve for intersection points where the ray
    /// intersects the sphere's surface. If the discriminant is negative, the ray
    /// misses the sphere.
    /// </remarks>
    public override float Hit(Ray r)
    {
        // Ray-sphere intersection:
        // (o + td - c) • (o + td - c) = R²
        // Expands into a quadratic equation in t.

        Vector d = r.Direction;
        Vector o = r.Origin;
        Vector o_minus_c = o - Center;

        float a = (float)Vector.Dot(d, d);
        float b = (float)Vector.Dot(d, o_minus_c);
        float c = (float)Vector.Dot(o_minus_c, o_minus_c) - R * R;

        float discriminant = b * b - a * c;

        // If discriminant < 0, the ray misses the sphere
        if (discriminant < 0)
        {
            return float.PositiveInfinity;
        }

        // Compute intersection distances (t values)
        float sqrtD = (float)Math.Sqrt(discriminant);
        float t_plus = (-b + sqrtD) / a;
        float t_minus = (-b - sqrtD) / a;

        // Return the smallest positive t (nearest intersection)
        if (t_minus > 0)
        {
            return t_minus < t_plus ? t_minus : t_plus;
        }

        if (t_plus > 0)
        {
            return t_plus;
        }

        // No valid intersection in the forward ray direction
        return float.PositiveInfinity;
    }

    /// <summary>
    /// Calculates the surface normal vector of the sphere at the given point.
    /// </summary>
    /// <param name="p">A point on the sphere's surface.</param>
    /// <returns>
    /// A normalized <see cref="Vector"/> representing the surface normal at <paramref name="p"/>.
    /// </returns>
    /// <remarks>
    /// The normal is calculated as the normalized vector from the sphere's center to the point.
    /// </remarks>
    public override Vector Normal(Vector p)
    {
        // Normal = (p - center) / radius
        Vector p_minus_c = p - Center;
        return new Vector(p_minus_c.X / R, p_minus_c.Y / R, p_minus_c.Z / R);
    }
}
