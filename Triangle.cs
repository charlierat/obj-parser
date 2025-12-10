
public class Triangle : Shape
{
    private Vector _v1 = null!;
    private Vector _v2 = null!;
    private Vector _v3 = null!;
    private Vector _n = null!;

    // UV coordinates for texture mapping
    private Vector _uv1 = null!;
    private Vector _uv2 = null!;
    private Vector _uv3 = null!;

    // Vertex normals for smooth shading
    private Vector? _n1;
    private Vector? _n2;
    private Vector? _n3;

    public Material? Material { get; set; }

    public Vector V1
    {
        get { return _v1; }
        set { _v1 = value; }
    }

    public Vector V2
    {
        get { return _v2; }
        set { _v2 = value; }
    }

    public Vector V3
    {
        get { return _v3; }
        set { _v3 = value; }
    }

    public Vector N
    {
        get { return _n; }
        set { _n = value; }
    }

    public Vector UV1
    {
        get { return _uv1; }
        set { _uv1 = value; }
    }

    public Vector UV2
    {
        get { return _uv2; }
        set { _uv2 = value; }
    }

    public Vector UV3
    {
        get { return _uv3; }
        set { _uv3 = value; }
    }

    public Vector? N1
    {
        get { return _n1; }
        set { _n1 = value; }
    }

    public Vector? N2
    {
        get { return _n2; }
        set { _n2 = value; }
    }

    public Vector? N3
    {
        get { return _n3; }
        set { _n3 = value; }
    }

    public Triangle()
    {
        V1 = new Vector(0, 0, 0);
        V2 = new Vector(1, 0, 0);
        V3 = new Vector(0, 1, 0);

        UV1 = new Vector(0, 0, 0);
        UV2 = new Vector(1, 0, 0);
        UV3 = new Vector(0, 1, 0);

        Material = null;

        A = new Vector(0, 0, 0);
        D = new Vector(128, 128, 128);
        S = new Vector(255, 255, 255);
        Shiny = 50;
    }

    public Triangle(Vector vertex1, Vector vertex2, Vector vertex3)
    {
        V1 = new Vector(vertex1.X, vertex1.Y, vertex1.Z);
        V2 = new Vector(vertex2.X, vertex2.Y, vertex2.Z);
        V3 = new Vector(vertex3.X, vertex3.Y, vertex3.Z);

        // Default UV coordinates
        UV1 = new Vector(0, 0, 0);
        UV2 = new Vector(1, 0, 0);
        UV3 = new Vector(0, 1, 0);

        Material = null;

        A = new Vector(0, 0, 0);
        D = new Vector(128, 128, 128);
        S = new Vector(255, 255, 255);
        Shiny = 50;
    }

    public Triangle(Vector vertex1, Vector vertex2, Vector vertex3, Material? material = null)
    {
        V1 = new Vector(vertex1.X, vertex1.Y, vertex1.Z);
        V2 = new Vector(vertex2.X, vertex2.Y, vertex2.Z);
        V3 = new Vector(vertex3.X, vertex3.Y, vertex3.Z);

        UV1 = new Vector(0, 0, 0);
        UV2 = new Vector(1, 0, 0);
        UV3 = new Vector(0, 1, 0);

        Material = material;

        A = new Vector(0, 0, 0);
        D = new Vector(128, 128, 128);
        S = new Vector(255, 255, 255);
        Shiny = 50;
    }

    /// <summary>
    /// Constructor with UV coordinates.
    /// </summary>
    public Triangle(Vector vertex1, Vector vertex2, Vector vertex3,
                   Vector uv1, Vector uv2, Vector uv3, Material? material = null)
    {
        V1 = new Vector(vertex1.X, vertex1.Y, vertex1.Z);
        V2 = new Vector(vertex2.X, vertex2.Y, vertex2.Z);
        V3 = new Vector(vertex3.X, vertex3.Y, vertex3.Z);

        UV1 = uv1;
        UV2 = uv2;
        UV3 = uv3;

        N1 = null;
        N2 = null;
        N3 = null;

        Material = material;

        A = new Vector(0, 0, 0);
        D = new Vector(128, 128, 128);
        S = new Vector(255, 255, 255);
        Shiny = 50;
    }

    /// <summary>
    /// Constructor with UV coordinates and vertex normals for smooth shading.
    /// </summary>
    public Triangle(Vector vertex1, Vector vertex2, Vector vertex3,
                   Vector uv1, Vector uv2, Vector uv3,
                   Vector? n1, Vector? n2, Vector? n3,
                   Material? material = null)
    {
        V1 = new Vector(vertex1.X, vertex1.Y, vertex1.Z);
        V2 = new Vector(vertex2.X, vertex2.Y, vertex2.Z);
        V3 = new Vector(vertex3.X, vertex3.Y, vertex3.Z);

        UV1 = uv1;
        UV2 = uv2;
        UV3 = uv3;

        if (n1 != null)
        {
            Vector temp1 = new Vector(n1.X, n1.Y, n1.Z);
            Vector.Normalize(ref temp1);
            N1 = temp1;
        }
        else
        {
            N1 = null;
        }

        if (n2 != null)
        {
            Vector temp2 = new Vector(n2.X, n2.Y, n2.Z);
            Vector.Normalize(ref temp2);
            N2 = temp2;
        }
        else
        {
            N2 = null;
        }

        if (n3 != null)
        {
            Vector temp3 = new Vector(n3.X, n3.Y, n3.Z);
            Vector.Normalize(ref temp3);
            N3 = temp3;
        }
        else
        {
            N3 = null;
        }

        Material = material;

        A = new Vector(0, 0, 0);
        D = new Vector(128, 128, 128);
        S = new Vector(255, 255, 255);
        Shiny = 50;
    }

    public override float Hit(Ray r)
    {
        const float EPSILON = 0.0000001f;

        Vector edge1 = V2 - V1;
        Vector edge2 = V3 - V1;

        Vector h = Vector.Cross(r.Direction, edge2);
        float a = (float)Vector.Dot(edge1, h);

        if (a > -EPSILON && a < EPSILON)
            return float.PositiveInfinity;

        float f = 1.0f / a;
        Vector s = r.Origin - V1;
        float u = f * (float)Vector.Dot(s, h);

        if (u < 0.0 || u > 1.0)
            return float.PositiveInfinity;

        Vector q = Vector.Cross(s, edge1);
        float v = f * (float)Vector.Dot(r.Direction, q);

        if (v < 0.0 || u + v > 1.0)
            return float.PositiveInfinity;

        float t = f * (float)Vector.Dot(edge2, q);

        if (t > EPSILON)
        {
            return t;
        }
        else
            return float.PositiveInfinity;
    }

    public override Vector Normal(Vector p)
    {
        Vector edge1 = V2 - V1;
        Vector edge2 = V3 - V1;

        Vector normal = Vector.Cross(edge1, edge2);

        double magnitude = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
        if (magnitude > 0)
        {
            normal = new Vector(normal.X / magnitude, normal.Y / magnitude, normal.Z / magnitude);
        }

        return normal;
    }

    /// <summary>
    /// Computes the barycentric coordinates of a point within the triangle.
    /// Used for interpolating UV coordinates.
    /// </summary>
    public (double u, double v, double w) GetBarycentricCoordinates(Vector p)
    {
        Vector v0 = V2 - V1;
        Vector v1 = V3 - V1;
        Vector v2 = p - V1;

        double d00 = Vector.Dot(v0, v0);
        double d01 = Vector.Dot(v0, v1);
        double d11 = Vector.Dot(v1, v1);
        double d20 = Vector.Dot(v2, v0);
        double d21 = Vector.Dot(v2, v1);

        double invDenom = 1.0 / (d00 * d11 - d01 * d01);

        double v = (d11 * d20 - d01 * d21) * invDenom;
        double w = (d00 * d21 - d01 * d20) * invDenom;
        double u = 1.0 - v - w;

        u = Math.Clamp(u, 0.0, 1.0);
        v = Math.Clamp(v, 0.0, 1.0);
        w = Math.Clamp(w, 0.0, 1.0);

        double sum = u + v + w;
        if (sum > 0)
        {
            u /= sum;
            v /= sum;
            w /= sum;
        }

        return (u, v, w);
    }

    /// <summary>
    /// Gets the interpolated UV coordinates at a point on the triangle surface.
    /// </summary>
    public (double u, double v) GetUVCoordinates(Vector p)
    {
        var (bary_u, bary_v, bary_w) = GetBarycentricCoordinates(p);

        double u = bary_u * UV1.X + bary_v * UV2.X + bary_w * UV3.X;
        double v = bary_u * UV1.Y + bary_v * UV2.Y + bary_w * UV3.Y;

        u = u - Math.Floor(u);
        v = v - Math.Floor(v);

        return (u, v);
    }

    /// <summary>
    /// Gets the interpolated normal at a point on the triangle surface using barycentric coordinates.
    /// This is used for smooth shading (Phong/Gouraud shading).
    /// Returns null if vertex normals are not available.
    /// </summary>
    public Vector? GetInterpolatedNormal(Vector p)
    {
        // Check if we have vertex normals
        if (N1 == null || N2 == null || N3 == null)
            return null;

        // Get barycentric coordinates
        var (u, v, w) = GetBarycentricCoordinates(p);

        // Interpolate normals: N = (1-u-v)*N1 + u*N2 + v*N3
        // Which is the same as: N = w*N1 + u*N2 + v*N3
        Vector interpolatedNormal = u * N1 + v * N2 + w * N3;

        // Normalize the interpolated normal
        Vector.Normalize(ref interpolatedNormal);

        return interpolatedNormal;
    }

    /// <summary>
    /// Gets the material's diffuse color at a specific point on the triangle.
    /// If the material has a texture, samples it at the interpolated UV coordinates.
    /// </summary>
    public Vector GetMaterialColor(Vector p)
    {
        if (Material == null)
        {
            return new Vector(128, 128, 128);
        }

        if (Material.HasTexture)
        {
            var (u, v) = GetUVCoordinates(p);
            return Material.GetDiffuseColor(u, v);
        }
        else
        {
            return Material.GetDiffuseColor();
        }
    }

    /// <summary>
    /// Gets the material's ambient color at a specific point.
    /// </summary>
    public Vector GetAmbientColor(Vector p)
    {
        if (Material == null)
        {
            return new Vector(10, 10, 10);
        }

        if (Material.HasTexture)
        {
            var (u, v) = GetUVCoordinates(p);
            return Material.GetAmbientColor(u, v);
        }
        else
        {
            return Material.GetAmbientColor();
        }
    }

    // Keep existing methods that don't need point parameter
    public Vector GetMaterialColor()
    {
        if (Material == null)
        {
            return new Vector(128, 128, 128);
        }
        return Material.GetDiffuseColor();
    }

    public Vector GetAmbientColor()
    {
        if (Material == null)
        {
            return new Vector(10, 10, 10);
        }
        return Material.GetAmbientColor();
    }

    public Vector GetSpecularColor()
    {
        if (Material == null)
        {
            return new Vector(255, 255, 255);
        }
        return Material.GetSpecularColor();
    }

    public double GetShininess()
    {
        if (Material == null)
        {
            return 50.0;
        }
        return Material.Shininess;
    }

    public AABB GetBoundingBox()
    {
        AABB box = new AABB();
        box.ExpandToInclude(V1);
        box.ExpandToInclude(V2);
        box.ExpandToInclude(V3);
        return box;
    }


}