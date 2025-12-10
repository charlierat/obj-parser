
public class TriangleMesh : Shape
{
    private List<Triangle> _triangles = null!;

    public List<Triangle> Triangles
    {
        get { return _triangles; }
        set { _triangles = value; }
    }

    public TriangleMesh(List<Triangle> triangles)
    {
        Triangles = triangles;
    }

    public override float Hit(Ray ray)
    {
        float closest = float.PositiveInfinity;

        foreach (Triangle triangle in Triangles)
        {
            float t = triangle.Hit(ray);

            if (t < closest)
            {
                closest = t;
            }
        }

        return closest;
    }

    public override Vector Normal(Vector point)
    {
        float closestDistance = float.MaxValue;
        Vector closestNormal = new Vector(0, 1, 0);

        foreach (Triangle triangle in Triangles)
        {
            Vector triangleCenter = (triangle.V1 + triangle.V2 + triangle.V3) * (1.0 / 3.0);
            double distance = ~(point - triangleCenter);

            if (distance < closestDistance)
            {
                closestDistance = (float)distance;
                closestNormal = triangle.Normal(point);
            }
        }

        return closestNormal;
    }
}