using System.Numerics;

/// <summary>
/// Represents a plane in 3D space.
/// </summary>
/// <remarks>
/// A plane is defined by a point on the plane (<see cref="A"/>) and a normal vector (<see cref="N"/>).
/// This class provides methods for ray-plane intersection testing and retrieving the surface normal.
/// </remarks>
public class Plane : Shape
{
    // A point located on the plane.
    private Vector _point = null!;

    // The normal vector of the plane (perpendicular to its surface).
    private Vector _n = null!;

    /// <summary>
    /// Gets or sets a point on the plane.
    /// </summary>
    public Vector Point
    {
        get { return _point; }
        set { _point = value; }
    }

    /// <summary>
    /// Gets or sets the normal vector of the plane.
    /// </summary>
    /// <remarks>
    /// The normal vector should typically be normalized to ensure accurate intersection and lighting calculations.
    /// </remarks>
    public Vector N
    {
        get { return _n; }
        set { _n = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plane"/> class
    /// with a default point at the origin and an upward-facing normal vector.
    /// </summary>
    public Plane()
    {
        Point = new Vector(0, 0, 0);
        N = new Vector(0, 1, 0);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plane"/> class
    /// with the specified normal vector and point on the plane.
    /// </summary>
    /// <param name="normal">The normal vector of the plane.</param>
    /// <param name="point">A point located on the plane.</param>
    public Plane(Vector normal, Vector point)
    {
        Point = new Vector(point.X, point.Y, point.Z);
        N = new Vector(normal.X, normal.Y, normal.Z);
    }

    /// <summary>
    /// Determines whether the given ray intersects with the plane.
    /// </summary>
    /// <param name="r">The ray to test for intersection.</param>
    /// <returns>
    /// The distance <c>t</c> from the ray origin to the intersection point.
    /// Returns <see cref="float.PositiveInfinity"/> if the ray is parallel to the plane.
    /// </returns>
    public override float Hit(Ray r)
    {
        // Calculate the denominator of the plane intersection formula: dot(rayDir, planeNormal)
        float denominator = (float)Vector.Dot(r.Direction, N);

        // If denominator == 0, the ray is parallel to the plane (no intersection)
        if (denominator == 0)
        {
            return float.PositiveInfinity;
        }

        // Compute the distance t along the ray to the intersection point
        float t = (float)Vector.Dot(Point - r.Origin, N) / denominator;
        return t;
    }

    /// <summary>
    /// Returns the surface normal of the plane at the given point.
    /// </summary>
    /// <param name="p">A point on the plane (unused for planes, as the normal is constant).</param>
    /// <returns>
    /// The plane's normal vector (<see cref="N"/>).
    /// </returns>
    public override Vector Normal(Vector p)
    {
        return N;
    }
}
