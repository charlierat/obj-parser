using System.Globalization;

/// <summary>
/// Enhanced OBJ parser that supports materials, textures, and proper face parsing.
/// Author: Charlie Ratliff
/// Date: 11/20/25
/// </summary>
public class ObjParser
{
    public static TriangleMesh ParseObjFile(string filePath)
    {
        List<Vector> vertices = new List<Vector>();
        List<Vector> textureCoords = new List<Vector>();
        List<Vector> normals = new List<Vector>();
        List<Triangle> triangles = new List<Triangle>();
        Dictionary<string, Material> materials = new Dictionary<string, Material>();
        Material? currentMaterial = null;

        string objDirectory = Path.GetDirectoryName(filePath) ?? "";

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            switch (parts[0].ToLower())
            {
                case "mtllib":
                    string projectRoot = Path.GetDirectoryName(objDirectory) ?? "";
                    string mtlPath = Path.Combine(projectRoot, "mtls", parts[1]);

                    var loadedMaterials = MtlParser.ParseMtlFile(mtlPath);
                    foreach (var material in loadedMaterials)
                    {
                        materials[material.Key] = material.Value;
                    }

                    break;

                case "usemtl":
                    currentMaterial = materials[parts[1]];
                    break;

                case "v":
                    float x = ParseFloat(parts[1]);
                    float y = ParseFloat(parts[2]);
                    float z = ParseFloat(parts[3]);
                    vertices.Add(new Vector(x, y, z));

                    break;

                case "vt":
                    float u = ParseFloat(parts[1]);
                    float v = ParseFloat(parts[2]);
                    textureCoords.Add(new Vector(u, v, 0));

                    break;

                case "vn":
                    float x1 = ParseFloat(parts[1]);
                    float y1 = ParseFloat(parts[2]);
                    float z1 = ParseFloat(parts[3]);
                    normals.Add(new Vector(x1, y1, z1));

                    break;

                case "f":
                    List<FaceVertex> faceVertices = new List<FaceVertex>();

                    for (int i = 1; i < parts.Length; i++)
                    {
                        var faceVertex = ParseFaceVertex(parts[i], vertices.Count, textureCoords.Count, normals.Count);
                        faceVertices.Add(faceVertex);
                    }

                    for (int i = 1; i < faceVertices.Count - 1; i++)
                    {
                        var v1 = faceVertices[0];
                        var v2 = faceVertices[i];
                        var v3 = faceVertices[i + 1];

                        Vector vertex1 = vertices[v1.VertexIndex];
                        Vector vertex2 = vertices[v2.VertexIndex];
                        Vector vertex3 = vertices[v3.VertexIndex];

                        Vector uv1 = new Vector(0, 0, 0);
                        Vector uv2 = new Vector(0, 0, 0);
                        Vector uv3 = new Vector(0, 0, 0);

                        if (v1.TextureIndex >= 0 && v1.TextureIndex < textureCoords.Count)
                            uv1 = textureCoords[v1.TextureIndex];
                        if (v2.TextureIndex >= 0 && v2.TextureIndex < textureCoords.Count)
                            uv2 = textureCoords[v2.TextureIndex];
                        if (v3.TextureIndex >= 0 && v3.TextureIndex < textureCoords.Count)
                            uv3 = textureCoords[v3.TextureIndex];

                        // Extract vertex normals if available
                        Vector? n1 = null;
                        Vector? n2 = null;
                        Vector? n3 = null;

                        if (v1.NormalIndex >= 0 && v1.NormalIndex < normals.Count)
                            n1 = normals[v1.NormalIndex];
                        if (v2.NormalIndex >= 0 && v2.NormalIndex < normals.Count)
                            n2 = normals[v2.NormalIndex];
                        if (v3.NormalIndex >= 0 && v3.NormalIndex < normals.Count)
                            n3 = normals[v3.NormalIndex];

                        Triangle triangle = new Triangle(vertex1, vertex2, vertex3, uv1, uv2, uv3, n1, n2, n3, currentMaterial);
                        triangles.Add(triangle);
                    }
                    break;

                default:
                    break;
            }
        }

        return new TriangleMesh(triangles);
    }

    /// <summary>
    /// Represents a vertex reference in a face definition.
    /// </summary>
    private struct FaceVertex
    {
        public int VertexIndex;
        public int TextureIndex;
        public int NormalIndex;

        public FaceVertex(int vertexIndex, int textureIndex, int normalIndex)
        {
            VertexIndex = vertexIndex;
            TextureIndex = textureIndex;
            NormalIndex = normalIndex;
        }
    }

    /// <summary>
    /// Parses a face vertex specification like "v/vt/vn" or "v//vn" or "v/vt" or "v".
    /// </summary>
    /// <param name="faceVertexString">The face vertex string</param>
    /// <param name="vertexCount">Total number of vertices for bounds checking</param>
    /// <param name="textureCount">Total number of texture coordinates for bounds checking</param>
    /// <param name="normalCount">Total number of normals for bounds checking</param>
    /// <returns>Parsed face vertex with 0-based indices</returns>
    private static FaceVertex ParseFaceVertex(string faceVertexString, int vertexCount, int textureCount, int normalCount)
    {
        string[] indices = faceVertexString.Split('/');

        int vertexIndex = -1;
        int textureIndex = -1;
        int normalIndex = -1;

        if (indices.Length > 0 && !string.IsNullOrEmpty(indices[0]))
        {
            int parsedIndex = int.Parse(indices[0]);
            vertexIndex = parsedIndex > 0 ? parsedIndex - 1 : vertexCount + parsedIndex;
        }

        if (indices.Length > 1 && !string.IsNullOrEmpty(indices[1]))
        {
            int parsedIndex = int.Parse(indices[1]);
            textureIndex = parsedIndex > 0 ? parsedIndex - 1 : textureCount + parsedIndex;
        }

        if (indices.Length > 2 && !string.IsNullOrEmpty(indices[2]))
        {
            int parsedIndex = int.Parse(indices[2]);
            normalIndex = parsedIndex > 0 ? parsedIndex - 1 : normalCount + parsedIndex;
        }

        return new FaceVertex(vertexIndex, textureIndex, normalIndex);
    }

    private static float ParseFloat(string value)
    {
        return float.Parse(value, CultureInfo.InvariantCulture);
    }
}