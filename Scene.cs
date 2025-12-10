using System.Collections.Generic;

/// <summary>
/// Represents a scene that contains a collection of shapes.
/// Now with BVH acceleration for triangle meshes.
/// </summary>
public class Scene
{
    private List<Shape> _shapes = new List<Shape>();
    private Vector _light = null!;
    private BVH? _bvh = null;
    private List<Triangle> _triangles = new List<Triangle>();
    private List<Shape> _nonTriangleShapes = new List<Shape>();

    public List<Shape> Shapes
    {
        get { return _shapes; }
        set { _shapes = value; }
    }

    public Vector Light
    {
        get { return _light; }
        set { _light = value; }
    }

    public void AddShape(ref Shape shape)
    {
        Shapes.Add(shape);

        if (shape is Triangle triangle)
        {
            _triangles.Add(triangle);
        }
        else
        {
            _nonTriangleShapes.Add(shape);
        }
    }

    /// <summary>
    /// Call this after adding all shapes to build the BVH acceleration structure.
    /// </summary>
    public void BuildBVH()
    {
        _bvh = new BVH();
        _bvh.Build(_triangles);
    }

    /// <summary>
    /// Finds the closest shape intersected by the ray.
    /// Uses BVH for triangles, brute force for other shapes.
    /// </summary>
    public (Shape? shape, float distance) Intersect(Ray ray)
    {
        Shape? closestShape = null;
        float closestDistance = float.PositiveInfinity;

        if (_bvh != null)
        {
            var (triangle, distance) = _bvh.Intersect(ray);
            if (triangle != null && distance < closestDistance)
            {
                closestDistance = distance;
                closestShape = triangle;
            }
        }

        foreach (Shape shape in _nonTriangleShapes)
        {
            float t = shape.Hit(ray);
            if (t < closestDistance)
            {
                closestDistance = t;
                closestShape = shape;
            }
        }

        return (closestShape, closestDistance);
    }
}