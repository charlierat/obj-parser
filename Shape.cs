using System.Drawing;
using System.Numerics;
using System;

/// <summary>
/// Represents an abstract base class for all geometric shapes.
/// </summary>
/// <remarks>
/// This class defines common properties and methods that all shapes must implement,
/// such as position, color, and intersection logic.
/// </remarks>
public abstract class Shape
{
    // The position of the shape in 3D space.
    private Vector _center = null!;

    // The diffuse color of the shape, used for shading calculations.
    private Vector _diffuseColor = null!;

    private Vector _a = null!;
    private Vector _d = null!;
    private Vector _s = null!;
    private double _shiny;

    /// <summary>
    /// Gets or sets the center position of the shape in 3D space.
    /// </summary>
    public Vector Center
    {
        get { return _center; }
        set { _center = value; }
    }

    /// <summary>
    /// Gets or sets the diffuse color of the shape.
    /// </summary>
    /// <remarks>
    /// This color is typically used in lighting models to determine how the shape
    /// reflects light diffusely (matte reflection).
    /// </remarks>
    public Vector DiffuseColor
    {
        get { return _diffuseColor; }
        set { _diffuseColor = value; }
    }

    public Vector A
    {
        get { return _a; }
        set { _a = value; }
    }

    public Vector D
    {
        get { return _d; }
        set { _d = value; }
    }

    public Vector S
    {
        get { return _s; }
        set { _s = value; }
    }

    public double Shiny
    {
        get { return _shiny; }
        set { _shiny = value; }
    }

    /// <summary>
    /// Determines whether the shape is intersected by the given ray.
    /// </summary>
    /// <param name="r">The ray to test for intersection.</param>
    /// <returns>
    /// The distance from the ray origin to the intersection point.
    /// Returns <see cref="float.PositiveInfinity"/> if there is no intersection.
    /// </returns>
    public abstract float Hit(Ray r);

    /// <summary>
    /// Calculates the surface normal vector of the shape at a given point.
    /// </summary>
    /// <param name="p">A point on the surface of the shape.</param>
    /// <returns>
    /// A normalized <see cref="Vector"/> representing the surface normal at the specified point.
    /// </returns>
    public abstract Vector Normal(Vector p);
}
