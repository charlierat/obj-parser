using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Bounding Volume Hierarchy for accelerating ray-triangle intersection tests.
/// Organizes triangles into a binary tree structure based on spatial subdivision.
/// </summary>
public class BVH
{
    private BVHNode? _root;
    private const int MAX_TRIANGLES_PER_LEAF = 4;

    /// <summary>
    /// Builds the BVH from a list of triangles.
    /// </summary>
    public void Build(List<Triangle> triangles)
    {
        _root = BuildRecursive(triangles, 0);
    }

    /// <summary>
    /// Recursively builds the BVH tree.
    /// </summary>
    private BVHNode BuildRecursive(List<Triangle> triangles, int depth)
    {
        BVHNode node = new BVHNode();

        foreach (Triangle tri in triangles)
        {
            node.BoundingBox.ExpandToInclude(tri.GetBoundingBox());
        }

        if (triangles.Count <= MAX_TRIANGLES_PER_LEAF)
        {
            node.Triangles = triangles;
            return node;
        }

        int axis = node.BoundingBox.LongestAxis();

        triangles.Sort((a, b) =>
        {
            double centroidA = GetCentroid(a, axis);
            double centroidB = GetCentroid(b, axis);
            return centroidA.CompareTo(centroidB);
        });

        int mid = triangles.Count / 2;
        List<Triangle> leftTriangles = triangles.GetRange(0, mid);
        List<Triangle> rightTriangles = triangles.GetRange(mid, triangles.Count - mid);

        node.Left = BuildRecursive(leftTriangles, depth + 1);
        node.Right = BuildRecursive(rightTriangles, depth + 1);

        return node;
    }

    /// <summary>
    /// Gets the centroid of a triangle along a specific axis.
    /// </summary>
    private double GetCentroid(Triangle tri, int axis)
    {
        Vector center = (tri.V1 + tri.V2 + tri.V3) * (1.0 / 3.0);

        switch (axis)
        {
            case 0: return center.X;
            case 1: return center.Y;
            case 2: return center.Z;
            default: return center.X;
        }
    }

    /// <summary>
    /// Finds the closest triangle intersection along the ray.
    /// Returns the triangle and distance, or null if no hit.
    /// </summary>
    public (Triangle? triangle, float distance) Intersect(Ray ray)
    {
        if (_root == null)
            return (null, float.PositiveInfinity);

        return IntersectNode(_root, ray, float.PositiveInfinity);
    }

    /// <summary>
    /// Recursively traverses the BVH to find intersections.
    /// </summary>
    private (Triangle? triangle, float distance) IntersectNode(BVHNode node, Ray ray, float closestDistance)
    {
        if (!node.BoundingBox.Intersect(ray))
            return (null, closestDistance);

        if (node.IsLeaf)
        {
            Triangle? closestTriangle = null;

            foreach (Triangle tri in node.Triangles!)
            {
                float t = tri.Hit(ray);
                if (t < closestDistance)
                {
                    closestDistance = t;
                    closestTriangle = tri;
                }
            }

            return (closestTriangle, closestDistance);
        }

        var leftResult = IntersectNode(node.Left!, ray, closestDistance);
        if (leftResult.triangle != null)
            closestDistance = leftResult.distance;

        var rightResult = IntersectNode(node.Right!, ray, closestDistance);
        if (rightResult.triangle != null)
            closestDistance = rightResult.distance;

        if (leftResult.triangle != null && rightResult.triangle != null)
        {
            return leftResult.distance < rightResult.distance ? leftResult : rightResult;
        }
        else if (leftResult.triangle != null)
        {
            return leftResult;
        }
        else
        {
            return rightResult;
        }
    }
}