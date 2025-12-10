using System;

/// <summary>
/// Represents a material with ambient, diffuse, and specular properties.
/// Now supports texture mapping for diffuse color.
/// Author: Charlie Ratliff
/// </summary>
public class Material
{
    private string _name = "";
    private Vector _ambientColor = null!;
    private Vector _diffuseColor = null!;
    private Vector _specularColor = null!;
    private double _shininess;
    private Texture? _diffuseTexture = null;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public Vector AmbientColor
    {
        get { return _ambientColor; }
        set { _ambientColor = value; }
    }

    public Vector DiffuseColor
    {
        get { return _diffuseColor; }
        set { _diffuseColor = value; }
    }

    public Vector SpecularColor
    {
        get { return _specularColor; }
        set { _specularColor = value; }
    }

    public double Shininess
    {
        get { return _shininess; }
        set { _shininess = value; }
    }

    public Texture? DiffuseTexture
    {
        get { return _diffuseTexture; }
        set { _diffuseTexture = value; }
    }

    public bool HasTexture => _diffuseTexture != null && _diffuseTexture.IsLoaded;

    public Material()
    {
        Name = "";
        AmbientColor = new Vector(0.1, 0.1, 0.1);
        DiffuseColor = new Vector(0.8, 0.8, 0.8);
        SpecularColor = new Vector(1, 1, 1);
        Shininess = 50.0;
    }

    public Material(string name, Vector ambient, Vector diffuse, Vector specular, double shininess = 50.0)
    {
        Name = name;
        AmbientColor = ambient;
        DiffuseColor = diffuse;
        SpecularColor = specular;
        Shininess = shininess;
    }

    public Vector GetDiffuseColor(double u = 0, double v = 0)
    {
        if (HasTexture)
        {
            return _diffuseTexture!.Sample(u, v);
        }
        else
        {
            return DiffuseColor * 255.0;
        }
    }

    public Vector GetSpecularColor()
    {
        return SpecularColor * 255.0;
    }

    public Vector GetAmbientColor()
    {
        return DiffuseColor * 255.0 * 0.05;
    }

    public Vector GetAmbientColor(double u = 0, double v = 0)
    {
        if (HasTexture)
        {
            return _diffuseTexture!.Sample(u, v) * 0.05;
        }
        else
        {
            return DiffuseColor * 255.0 * 0.05;
        }
    }
}