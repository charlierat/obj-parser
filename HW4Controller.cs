public class HW4Controller
{
    static void Main(string[] args)
    {
        Camera c = new Camera(Camera.Projection.Orthographic, new Vector(0f, 0f, 10.0f), new Vector(0f, 0f, 0f),
        new Vector(0.0f, 1f, 0f), 0.1f, 600f, 2048, 2048, -3f, 3f, 0f, 6f);

        Scene scene = new Scene();
        scene.Light = new Vector(0f, 0f, 10f);

        string objFilePath = "objs/mercedes.obj";

        TriangleMesh mesh = ObjParser.ParseObjFile(objFilePath);

        foreach (Triangle triangle in mesh.Triangles)
        {
            Shape triangleShape = triangle;
            scene.AddShape(ref triangleShape);
        }

        scene.BuildBVH();

        c.RenderImage("mercedes.bmp", scene);
    }
}