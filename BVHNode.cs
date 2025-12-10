using System;
using System.Collections.Generic;

/// <summary>
/// A node in the Bounding Volume Hierarchy tree.
/// Each node contains either child nodes (internal) or triangles (leaf).
/// </summary>
public class BVHNode
{
    public AABB BoundingBox { get; set; }
    public BVHNode? Left { get; set; }
    public BVHNode? Right { get; set; }
    public List<Triangle>? Triangles { get; set; }

    public bool IsLeaf => Triangles != null;

    public BVHNode()
    {
        BoundingBox = new AABB();
    }
}