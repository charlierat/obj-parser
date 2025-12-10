using System;

/// <summary>
/// Represents a three-dimensional mathematical vector with basic vector operations.
/// </summary>
public class Vector
{
    private double _x;
    private double _y;
    private double _z;

    /// <summary>
    /// Gets or sets the X component of the vector.
    /// </summary>
    public double X
    {
        get { return _x; }
        set { _x = value; }
    }

    /// <summary>
    /// Gets or sets the Y component of the vector.
    /// </summary>
    public double Y
    {
        get { return _y; }
        set { _y = value; }
    }

    /// <summary>
    /// Gets or sets the Z component of the vector.
    /// </summary>
    public double Z
    {
        get { return _z; }
        set { _z = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector"/> class with zero values.
    /// </summary>
    public Vector()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector"/> class with specified values.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    public Vector(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Unary plus operator. Returns a copy of the vector.
    /// </summary>
    public static Vector operator +(Vector v) => new Vector(v.X, v.Y, v.Z);

    /// <summary>
    /// Unary minus operator. Returns the negated vector.
    /// </summary>
    public static Vector operator -(Vector v) => new Vector(-v.X, -v.Y, -v.Z);

    /// <summary>
    /// Adds two vectors component-wise.
    /// </summary>
    public static Vector operator +(Vector a, Vector b) =>
        new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>
    /// Subtracts one vector from another component-wise.
    /// </summary>
    public static Vector operator -(Vector a, Vector b) =>
        new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    /// <summary>
    /// Multiplies a scalar and a vector.
    /// </summary>
    public static Vector operator *(double k, Vector v) =>
        new Vector(k * v.X, k * v.Y, k * v.Z);

    /// <summary>
    /// Multiplies a vector and a scalar.
    /// </summary>
    public static Vector operator *(Vector v, double k) =>
        new Vector(v.X * k, v.Y * k, v.Z * k);

    /// <summary>
    /// Computes the magnitude (length) of the vector.
    /// </summary>
    public static double operator ~(Vector v) =>
        Math.Sqrt(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2) + Math.Pow(v.Z, 2));

    /// <summary>
    /// Computes the dot product of two vectors.
    /// </summary>
    public static double Dot(Vector a, Vector b) =>
        a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    /// <summary>
    /// Computes the cross product of two vectors.
    /// </summary>
    public static Vector Cross(Vector a, Vector b) =>
        new Vector(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );

    /// <summary>
    /// Computes the angle (in radians) between two vectors.
    /// </summary>
    public static double GetAngle(Vector a, Vector b)
    {
        double magnitudeA = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        double magnitudeB = Math.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z);

        return Math.Acos(Dot(a, b) / (magnitudeA * magnitudeB));
    }

    /// <summary>
    /// Normalizes a vector so that it has a magnitude of 1.
    /// </summary>
    /// <param name="v">The vector to normalize.</param>
    public static void Normalize(ref Vector v)
    {
        double magnitude = Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        if (magnitude > 0)
        {
            v.X /= magnitude;
            v.Y /= magnitude;
            v.Z /= magnitude;
        }
    }

    /// <summary>
    /// Converts all components of a vector to their absolute values.
    /// </summary>
    /// <param name="v">The vector to modify.</param>
    public static void Abs(ref Vector v)
    {
        v.X = Math.Abs(v.X);
        v.Y = Math.Abs(v.Y);
        v.Z = Math.Abs(v.Z);
    }

    /// <summary>
    /// Returns a string representation of the vector in (X, Y, Z) format.
    /// </summary>
    public override string ToString() => $"({X}, {Y}, {Z})";

    /// <summary>
    /// Determines whether two vectors are equal by comparing their components.
    /// </summary>
    public bool Equals(Vector v)
    {
        return X == v.X && Y == v.Y && Z == v.Z;
    }
}
