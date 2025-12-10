using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Numerics;

/// <summary>
/// Represents an image buffer that can be painted on using RGB color values
/// and saved with gamma correction applied.
/// </summary>
public class Image
{
    private Image<Rgba32> _solution;
    private double _gamma;
    private int _height;
    private int _width;

    /// <summary>
    /// Gets or sets the width of the image in pixels.
    /// </summary>
    public int Width
    {
        get { return _width; }
        set { _width = value; }
    }

    /// <summary>
    /// Gets or sets the height of the image in pixels.
    /// </summary>
    public int Height
    {
        get { return _height; }
        set { _height = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Image"/> class
    /// with the specified dimensions and gamma correction factor.
    /// </summary>
    /// <param name="_width">The width of the image in pixels. Default is 512.</param>
    /// <param name="_height">The height of the image in pixels. Default is 512.</param>
    /// <param name="_gamma">The gamma correction value. Default is 1.8.</param>
    public Image(int _width = 512, int _height = 512, float _gamma = 1.8f)
    {
        Width = _width;
        Height = _height;
        this._gamma = _gamma;

        _solution = new Image<Rgba32>(Width, Height);
        _solution.Mutate(ctx => ctx.BackgroundColor(Color.Black));
    }

    /// <summary>
    /// Paints a pixel at the specified coordinates with the given color and alpha transparency.
    /// </summary>
    /// <param name="i">The x-coordinate of the pixel.</param>
    /// <param name="j">The y-coordinate of the pixel.</param>
    /// <param name="colors">The RGB vector representing the pixel color.</param>
    /// <param name="alpha">The alpha transparency (0–255). Default is 255 (opaque).</param>
    public void Paint(int i, int j, Vector colors, double alpha = 255)
    {
        if (i < 0 || j < 0 || i >= Width || j >= Height)
        {
            return;
        }
        int inverseJ = Height - 1 - j;

        byte r = Clamp(colors.X, 0, 255);
        byte g = Clamp(colors.Y, 0, 255);
        byte b = Clamp(colors.Z, 0, 255);
        byte a = (byte)alpha;

        _solution[i, inverseJ] = new Rgba32(r, g, b, a);
    }

    /// <summary>
    /// Saves the image to a file, applying gamma correction before writing.
    /// </summary>
    /// <param name="fileName">The path and file name where the image will be saved.</param>
    public void SaveImage(string fileName)
    {
        var corrected = _solution.Clone();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Rgba32 original = _solution[i, j];

                double oldR = original.R / 255.0;
                double oldG = original.G / 255.0;
                double oldB = original.B / 255.0;

                byte r = (byte)(255 * Math.Pow(oldR, 1 / _gamma));
                byte g = (byte)(255 * Math.Pow(oldG, 1 / _gamma));
                byte b = (byte)(255 * Math.Pow(oldB, 1 / _gamma));

                corrected[i, j] = new Rgba32(Clamp(r, 0, 255), Clamp(g, 0, 255), Clamp(b, 0, 255), original.A);
            }
        }

        corrected.Save(fileName);
    }

    public byte Clamp(double color, double min, double max)
    {
        if (color < min)
        {
            color = min;
        }
        if (color > max)
        {
            color = max;
        }
        return (byte)color;
    }

}
