using System;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents a camera for 3D rendering with support for both perspective and orthographic projections.
/// Handles ray generation and image rendering with customizable viewport and frustum parameters.
/// Author: Charlie Ratliff
/// Date: 9/23/25
/// </summary>
public class Camera
{
    /// <summary>
    /// Specifies the type of projection used by the camera.
    /// </summary>
    public enum Projection
    {
        /// <summary>
        /// Perspective projection creates a realistic view with foreshortening.
        /// Objects farther away appear smaller.
        /// </summary>
        Perspective,
        /// <summary>
        /// Orthographic projection maintains parallel lines and uniform scaling.
        /// Objects maintain their size regardless of distance.
        /// </summary>
        Orthographic
    }

    private Projection _projection;
    private Vector _eye = null!;
    private Vector _lookAt = null!;
    private Vector _up = null!;
    private double _near;
    private double _far;
    private double _width;
    private double _height;
    private double _left;
    private double _right;
    private double _bottom;
    private double _top;

    /// <summary>
    /// Gets or sets the position of the camera's eye point in world coordinates.
    /// This determines where the camera is located in 3D space.
    /// </summary>
    /// <value>A Vector representing the camera's position.</value>
    public Vector Eye
    {
        get { return _eye; }
        set { _eye = value; }
    }

    /// <summary>
    /// Gets or sets the target point that the camera is looking at in world coordinates.
    /// This determines the direction the camera is facing.
    /// </summary>
    /// <value>A Vector representing the look-at target position.</value>
    public Vector LookAt
    {
        get { return _lookAt; }
        set { _lookAt = value; }
    }

    /// <summary>
    /// Gets or sets the camera's up vector, defining the orientation of the camera.
    /// This vector determines which direction is "up" for the camera.
    /// </summary>
    /// <value>A Vector representing the up direction, typically (0, 1, 0).</value>
    public Vector Up
    {
        get { return _up; }
        set { _up = value; }
    }

    /// <summary>
    /// Gets or sets the distance from the camera's eye point to the near clipping plane.
    /// Objects closer than this distance will not be rendered.
    /// </summary>
    /// <value>A positive double value representing the near plane distance.</value>
    public double Near
    {
        get { return _near; }
        set { _near = value; }
    }

    /// <summary>
    /// Gets or sets the distance from the camera's eye point to the far clipping plane.
    /// Objects farther than this distance will not be rendered.
    /// </summary>
    /// <value>A positive double value representing the far plane distance.</value>
    public double Far
    {
        get { return _far; }
        set { _far = value; }
    }

    /// <summary>
    /// Gets or sets the width of the camera's viewport in pixels.
    /// This determines the horizontal resolution of the rendered image.
    /// </summary>
    /// <value>A positive double value representing the viewport width.</value>
    public double Width
    {
        get { return _width; }
        set { _width = value; }
    }

    /// <summary>
    /// Gets or sets the height of the camera's viewport in pixels.
    /// This determines the vertical resolution of the rendered image.
    /// </summary>
    /// <value>A positive double value representing the viewport height.</value>
    public double Height
    {
        get { return _height; }
        set { _height = value; }
    }

    /// <summary>
    /// Gets or sets the left boundary of the camera's viewing frustum.
    /// Used for defining the horizontal extent of the viewing volume.
    /// </summary>
    /// <value>A double value representing the left frustum boundary.</value>
    public double Left
    {
        get { return _left; }
        set { _left = value; }
    }

    /// <summary>
    /// Gets or sets the right boundary of the camera's viewing frustum.
    /// Used for defining the horizontal extent of the viewing volume.
    /// </summary>
    /// <value>A double value representing the right frustum boundary.</value>
    public double Right
    {
        get { return _right; }
        set { _right = value; }
    }

    /// <summary>
    /// Gets or sets the bottom boundary of the camera's viewing frustum.
    /// Used for defining the vertical extent of the viewing volume.
    /// </summary>
    /// <value>A double value representing the bottom frustum boundary.</value>
    public double Bottom
    {
        get { return _bottom; }
        set { _bottom = value; }
    }

    /// <summary>
    /// Gets or sets the top boundary of the camera's viewing frustum.
    /// Used for defining the vertical extent of the viewing volume.
    /// </summary>
    /// <value>A double value representing the top frustum boundary.</value>
    public double Top
    {
        get { return _top; }
        set { _top = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class with default orthographic settings.
    /// Creates a generic orthographic camera centered at the origin with a 512x512 pixel viewport
    /// and a 2x2 unit viewing frustum.
    /// </summary>
    public Camera()
    {
        _projection = Projection.Orthographic;
        Eye = new Vector(0, 0, 1);
        LookAt = new Vector(0, 0, 0);
        Up = new Vector(0, 1, 0);
        Near = 0.1;
        Far = 10;
        Width = 512;
        Height = 512;
        Left = -1;
        Right = 1;
        Bottom = -1;
        Top = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class with specified parameters.
    /// Allows full customization of camera properties including projection type, position, and frustum settings.
    /// </summary>
    /// <param name="projection">The projection type for the camera (Perspective or Orthographic).</param>
    /// <param name="eye">The position of the camera's eye point in world coordinates.</param>
    /// <param name="lookAt">The location the camera is looking at in world coordinates.</param>
    /// <param name="up">The camera's up vector defining orientation.</param>
    /// <param name="near">The distance from the camera's eye point to the near clipping plane. Default is 0.1.</param>
    /// <param name="far">The distance from the camera's eye point to the far clipping plane. Default is 10.</param>
    /// <param name="width">The width of the camera's viewport in pixels. Default is 512.</param>
    /// <param name="height">The height of the camera's viewport in pixels. Default is 512.</param>
    /// <param name="left">The left boundary of the camera's viewing frustum. Default is -1.0.</param>
    /// <param name="right">The right boundary of the camera's viewing frustum. Default is 1.0.</param>
    /// <param name="bottom">The bottom boundary of the camera's viewing frustum. Default is -1.0.</param>
    /// <param name="top">The top boundary of the camera's viewing frustum. Default is 1.0.</param>
    public Camera(Projection projection, Vector eye, Vector lookAt, Vector up, double near = 0.1, double far = 10, double width = 512, double height = 512,
                    double left = -1.0, double right = 1.0, double bottom = -1.0, double top = 1.0)
    {
        _projection = projection;
        Eye = eye;
        LookAt = lookAt;
        Up = up;
        Near = near;
        Far = far;
        Width = width;
        Height = height;
        Left = left;
        Right = right;
        Bottom = bottom;
        Top = top;
    }

    /// <summary>
    /// Renders an image using the camera's current settings and saves it to the specified file.
    /// Creates a simple gradient effect based on the projection type - horizontal gradient for orthographic
    /// projection and vertical gradient for perspective projection.
    /// </summary>
    /// <param name="fileName">The filename where the rendered image will be saved.</param>
    /// <remarks>
    /// The rendering process generates rays for each pixel and applies color based on ray properties.
    /// For orthographic projection, the color is determined by the normalized X component of the ray origin.
    /// For perspective projection, the color is determined by the Y component of the ray direction.
    /// </remarks>
    public void RenderImage(String fileName, Scene scene)
    {
        Image image = new Image((int)Width, (int)Height, 1.8f);

        double[,] depthBuffer = new double[(int)Width, (int)Height];

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                depthBuffer[i, j] = float.MaxValue;
            }
        }

        Parallel.For(0, (int)Width, i =>
        {
            for (int j = 0; j < Height; j++)
            {
                Ray ray = GetRay(i, j);

                var (hitShape, t) = scene.Intersect(ray);

                if (t >= Near && t <= Far && t != float.PositiveInfinity)
                {
                    depthBuffer[i, j] = t;
                }

                if (depthBuffer[i, j] != float.MaxValue)
                {
                    Vector shapeVertex = ray.At(depthBuffer[i, j]);

                    Vector ambientColor, diffuseColor, specularColor;
                    double shininess;

                    if (hitShape is Triangle triangle && triangle.Material != null)
                    {
                        ambientColor = triangle.GetAmbientColor(shapeVertex);
                        diffuseColor = triangle.GetMaterialColor(shapeVertex);
                        specularColor = triangle.GetSpecularColor();
                        shininess = triangle.GetShininess();
                    }
                    else
                    {
                        ambientColor = hitShape!.A;
                        diffuseColor = hitShape.D;
                        specularColor = hitShape.S;
                        shininess = hitShape.Shiny;
                    }

                    Vector lightDirection = scene.Light - shapeVertex;
                    Vector.Normalize(ref lightDirection);

                    Vector vertexToCamera = Eye - shapeVertex;
                    Vector.Normalize(ref vertexToCamera);
                    Vector bisector = vertexToCamera + lightDirection;
                    Vector.Normalize(ref bisector);

                    Vector smoothNormal = SmoothedNormal(hitShape as Triangle, shapeVertex);
                    Vector.Normalize(ref smoothNormal);

                    Vector viewDirection = Eye - shapeVertex;
                    Vector.Normalize(ref viewDirection);
                    if (Vector.Dot(smoothNormal, viewDirection) < 0)
                    {
                        smoothNormal = -smoothNormal;
                    }

                    double diffuseDot = Math.Max(0, Vector.Dot(lightDirection, smoothNormal));
                    double specularDot = Math.Max(0, Vector.Dot(bisector, smoothNormal));

                    Vector lightColor = ambientColor + diffuseColor * diffuseDot + specularColor * Math.Pow(specularDot, shininess);

                    image.Paint(i, j, lightColor);
                }
                else
                {
                    image.Paint(i, j, new Vector(0, 0, 0));
                }
            }
        });

        image.SaveImage(fileName);
    }

    /// <summary>
    /// Generates a ray for the specified pixel coordinates based on the camera's projection type.
    /// This method handles the mathematical conversion from 2D pixel coordinates to 3D ray representation.
    /// </summary>
    /// <param name="i">The horizontal pixel coordinate (0 to Width-1).</param>
    /// <param name="j">The vertical pixel coordinate (0 to Height-1).</param>
    /// <returns>A <see cref="Ray"/> object representing the ray from the camera through the specified pixel.</returns>
    /// <remarks>
    /// For orthographic projection, rays are parallel and originate from points on the near plane.
    /// For perspective projection, all rays originate from the camera's eye position and diverge outward.
    /// The method maps pixel coordinates to normalized device coordinates within the viewing frustum.
    /// </remarks>
    private Ray GetRay(int i, int j)
    {
        double u = Left + (Right - Left) * i / Width;
        double v = Bottom + (Top - Bottom) * j / Height;

        Vector w = Eye - LookAt;
        Vector.Normalize(ref w);

        Vector U = Vector.Cross(Up, w);
        Vector.Normalize(ref U);

        Vector V = Vector.Cross(w, U);
        Vector.Normalize(ref V);

        if (_projection == Projection.Orthographic)
        {
            Vector origin = Eye + u * U + v * V;

            Vector direction = -w;

            return new Ray(origin, direction);
        }
        else
        {
            Vector origin = Eye;

            Vector direction = u * U + v * V - w;

            return new Ray(origin, direction);
        }
    }

    private bool isShadow(Vector q, double t, Vector l, Vector lightDirection, Shape hitShape, Scene scene)
    {
        Vector p = q + t * l;
        Ray shadowRay = new Ray(p, lightDirection);

        double distanceToLight = ~(scene.Light - p);

        foreach (Shape s in scene.Shapes)
        {
            float hit = s.Hit(shadowRay);
            if (hit != float.PositiveInfinity && hit > 0 && hit < distanceToLight && s != hitShape)
            {
                return true;
            }
        }
        return false;
    }

    private Vector SmoothedNormal(Triangle? t, Vector p)
    {
        if (t == null || t.N1 == null || t.N2 == null || t.N3 == null)
        {
            return t?.Normal(p) ?? new Vector(0, 1, 0);
        }

        Vector? interpolatedNormal = t.GetInterpolatedNormal(p);

        if (interpolatedNormal != null)
        {
            return interpolatedNormal;
        }

        return t.Normal(p);
    }

}