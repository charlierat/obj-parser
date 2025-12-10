using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a geometric ray in 3D space, defined by an origin point and a direction vector.
/// Commonly used in ray tracing, collision detection, and geometric calculations.
/// Author: Charlie Ratliff
/// Date: 9/23/25
/// </summary>
public class Ray
{
    private Vector _origin = null!;
    private Vector _direction = null!;

    /// <summary>
    /// Gets or sets the origin point of the ray in 3D space.
    /// This is the starting position from which the ray extends.
    /// </summary>
    /// <value>A Vector representing the 3D coordinates of the ray's origin point.</value>
    public Vector Origin
    {
        get { return _origin; }
        set { _origin = value; }
    }

    /// <summary>
    /// Gets or sets the direction vector of the ray.
    /// This vector defines the direction in which the ray extends from its origin.
    /// </summary>
    /// <value>A Vector representing the direction of the ray. Should typically be normalized for consistent behavior.</value>
    /// <remarks>
    /// While the direction vector is not automatically normalized, many ray-related calculations
    /// work best when the direction vector has unit length (magnitude of 1).
    /// </remarks>
    public Vector Direction
    {
        get { return _direction; }
        set
        {
            double sum = value.X + value.Y + value.Z;
            if (sum == 1)
            {
                _direction = value;
            }
            else
            {
                double magnitude = Math.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
                _direction = new Vector(value.X / magnitude, value.Y / magnitude, value.Z / magnitude);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray"/> class with default values.
    /// Creates a ray positioned at the origin (0, 0, 0) pointing in the negative Z direction.
    /// </summary>
    /// <remarks>
    /// The default ray points down the negative Z-axis, which is a common convention
    /// in computer graphics where the camera looks down the negative Z direction.
    /// </remarks>
    public Ray()
    {
        Origin = new Vector(0, 0, 0);
        Direction = new Vector(0, 0, -1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray"/> class with the specified origin and direction.
    /// Creates copies of the provided vectors to ensure independence from the original vector objects.
    /// </summary>
    /// <param name="origin">The starting point of the ray in 3D space.</param>
    /// <param name="direction">The direction vector defining which way the ray extends from the origin.</param>
    /// <remarks>
    /// This constructor creates deep copies of the input vectors, so modifications to the original
    /// vectors after construction will not affect the ray's properties.
    /// </remarks>
    public Ray(Vector origin, Vector direction)
    {
        Origin = new Vector(origin.X, origin.Y, origin.Z);

        double sum = direction.X + direction.Y + direction.Z;
        if (sum == 1)
        {
            Direction = new Vector(direction.X, direction.Y, direction.Z);
        }
        else
        {
            double magnitude = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
            Direction = new Vector(direction.X / magnitude, direction.Y / magnitude, direction.Z / magnitude);
        }
    }

    /// <summary>
    /// Calculates a point along the ray at the specified parametric distance.
    /// Uses the ray equation: P(t) = Origin + t * Direction, where t is the parameter.
    /// </summary>
    /// <param name="t">The parametric distance along the ray. 
    /// A value of 0 returns the origin, positive values extend in the direction of the ray,
    /// and negative values extend in the opposite direction.</param>
    /// <returns>A Vector representing the 3D coordinates of the point at parameter t along the ray.</returns>
    /// <remarks>
    /// <para>The parameter t represents the distance along the ray if the direction vector is normalized (unit length).</para>
    /// <para>If the direction vector is not normalized, t represents a scaled distance.</para>
    /// <para>Common usage patterns:</para>
    /// <list type="bullet">
    /// <item><description>t = 0: Returns the ray's origin point</description></item>
    /// <item><description>t > 0: Points along the ray's forward direction</description></item>
    /// <item><description>t &lt; 0: Points behind the ray's origin (opposite direction)</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// Ray ray = new Ray(new Vector(0, 0, 0), new Vector(1, 0, 0));
    /// Vector point = ray.At(5.0); // Returns (5, 0, 0)
    /// </code>
    /// </example>
    public Vector At(double t)
    {
        return Origin + t * Direction;
    }
}