using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

/// <summary>
/// Represents a texture image that can be sampled using UV coordinates.
/// Supports bilinear filtering for smooth texture interpolation.
/// Author: Charlie Ratliff
/// </summary>
public class Texture
{
    private SixLabors.ImageSharp.Image<Rgba32>? _image;
    private string _filePath;

    public int Width => _image?.Width ?? 0;
    public int Height => _image?.Height ?? 0;
    public bool IsLoaded => _image != null;

    /// <summary>
    /// Creates a texture from an image file.
    /// </summary>
    /// <param name="filePath">Path to the texture image file (PNG, JPG, etc.)</param>
    public Texture(string filePath)
    {
        _filePath = filePath;
        LoadImage();
    }

    /// <summary>
    /// Loads the image from disk.
    /// </summary>
    private void LoadImage()
    {
        _image = SixLabors.ImageSharp.Image.Load<Rgba32>(_filePath);
    }

    /// <summary>
    /// Samples the texture at the given UV coordinates using bilinear interpolation.
    /// </summary>
    /// <param name="u">Horizontal texture coordinate (0-1)</param>
    /// <param name="v">Vertical texture coordinate (0-1)</param>
    /// <returns>RGB color vector (0-255 range)</returns>
    public Vector Sample(double u, double v)
    {
        if (_image == null)
        {
            return new Vector(255, 0, 255);
        }

        u = Math.Clamp(u, 0.0, 1.0);
        v = Math.Clamp(v, 0.0, 1.0);
        v = 1.0 - v;

        double x = u * (Width - 1);
        double y = v * (Height - 1);

        int x0 = (int)Math.Floor(x);
        int y0 = (int)Math.Floor(y);
        int x1 = Math.Min(x0 + 1, Width - 1);
        int y1 = Math.Min(y0 + 1, Height - 1);

        double fx = x - x0;
        double fy = y - y0;

        Rgba32 c00 = _image[x0, y0];
        Rgba32 c10 = _image[x1, y0];
        Rgba32 c01 = _image[x0, y1];
        Rgba32 c11 = _image[x1, y1];

        double r = BilinearInterpolate(c00.R, c10.R, c01.R, c11.R, fx, fy);
        double g = BilinearInterpolate(c00.G, c10.G, c01.G, c11.G, fx, fy);
        double b = BilinearInterpolate(c00.B, c10.B, c01.B, c11.B, fx, fy);

        return new Vector(r, g, b);
    }

    /// <summary>
    /// Performs bilinear interpolation between four values.
    /// </summary>
    private double BilinearInterpolate(double v00, double v10, double v01, double v11, double fx, double fy)
    {
        double v0 = v00 * (1 - fx) + v10 * fx;
        double v1 = v01 * (1 - fx) + v11 * fx;

        return v0 * (1 - fy) + v1 * fy;
    }
}