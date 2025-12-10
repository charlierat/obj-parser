using System.Globalization;

/// <summary>
/// Parser for Material Template Library (MTL) files.
/// This class reads MTL files and creates Material objects with the parsed data.
/// Author: Charlie Ratliff
/// Date: 11/20/25
/// </summary>
public static class MtlParser
{
    /// <summary>
    /// Parses an MTL file and returns a dictionary of materials keyed by material name.
    /// </summary>
    /// <param name="filePath">Path to the MTL file</param>
    /// <returns>Dictionary of materials where key is material name and value is Material object</returns>
    /// <exception cref="FileNotFoundException">Thrown when the MTL file is not found</exception>
    /// <exception cref="FormatException">Thrown when the MTL file contains invalid data</exception>
    public static Dictionary<string, Material> ParseMtlFile(string filePath)
    {
        var materials = new Dictionary<string, Material>();

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"MTL file not found: {filePath}");
        }

        Material? currentMaterial = null;
        string[] lines = File.ReadAllLines(filePath);
        string mtlDirectory = Path.GetDirectoryName(filePath) ?? "";
        string projectRoot = Path.GetDirectoryName(mtlDirectory) ?? "";

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            string[] tokens = trimmedLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
                continue;

            string command = tokens[0].ToLower();

            switch (command)
            {
                case "newmtl":
                    if (currentMaterial != null)
                    {
                        materials[currentMaterial.Name] = currentMaterial;
                    }

                    currentMaterial = new Material();
                    currentMaterial.Name = tokens[1];
                    break;

                case "ka":
                    currentMaterial!.AmbientColor = ParseColor(tokens, trimmedLine);
                    break;

                case "kd":
                    currentMaterial!.DiffuseColor = ParseColor(tokens, trimmedLine);
                    break;

                case "ks":
                    currentMaterial!.SpecularColor = ParseColor(tokens, trimmedLine);
                    break;

                case "ns":
                    if (currentMaterial != null && tokens.Length >= 2)
                    {
                        if (double.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double shininess))
                        {
                            currentMaterial.Shininess = shininess;
                        }
                    }
                    break;

                case "map_kd":
                    if (currentMaterial != null && tokens.Length >= 2)
                    {
                        string textureFileName = tokens[tokens.Length - 1];

                        string texturePath = Path.Combine(projectRoot, "textures", textureFileName);

                        if (File.Exists(texturePath))
                        {
                            currentMaterial.DiffuseTexture = new Texture(texturePath);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        if (currentMaterial != null)
        {
            materials[currentMaterial.Name] = currentMaterial;
        }

        return materials;
    }

    /// <summary>
    /// Parses RGB color values from MTL file tokens.
    /// </summary>
    /// <param name="tokens">Array of tokens from the MTL line</param>
    /// <param name="originalLine">Original line for error reporting</param>
    /// <returns>Vector representing the RGB color (values between 0.0 and 1.0)</returns>
    /// <exception cref="FormatException">Thrown when color values are invalid</exception>
    private static Vector ParseColor(string[] tokens, string originalLine)
    {
        double r = double.Parse(tokens[1], CultureInfo.InvariantCulture);
        double g = double.Parse(tokens[2], CultureInfo.InvariantCulture);
        double b = double.Parse(tokens[3], CultureInfo.InvariantCulture);

        r = Math.Max(0.0, Math.Min(1.0, r));
        g = Math.Max(0.0, Math.Min(1.0, g));
        b = Math.Max(0.0, Math.Min(1.0, b));

        return new Vector(r, g, b);
    }
}