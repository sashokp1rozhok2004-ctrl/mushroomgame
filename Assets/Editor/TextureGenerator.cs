using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// Generates improved procedural textures for game objects
/// </summary>
public static class TextureGenerator
{
    public static Texture2D CreateMushroomCapTexture(int resolution, Color baseColor, Color spotColor, int seed)
    {
        var tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, true);
        var pixels = new Color[resolution * resolution];
        System.Random rng = new System.Random(seed);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float nx = (float)x / resolution;
                float ny = (float)y / resolution;

                float noise1 = Mathf.PerlinNoise(nx * 4f + seed, ny * 4f + seed);
                float noise2 = Mathf.PerlinNoise(nx * 8f + seed * 2, ny * 8f + seed * 2);
                float combined = noise1 * 0.7f + noise2 * 0.3f;

                Color baseVariation = Color.Lerp(baseColor * 0.7f, baseColor * 1.2f, combined);

                float spotNoise = Mathf.PerlinNoise(nx * 12f + seed * 3, ny * 12f + seed * 3);
                if (spotNoise > 0.65f)
                {
                    float spotStrength = (spotNoise - 0.65f) / 0.35f;
                    baseVariation = Color.Lerp(baseVariation, spotColor, spotStrength * 0.6f);
                }

                float centerDist = Vector2.Distance(new Vector2(nx, ny), new Vector2(0.5f, 0.5f));
                float vignette = Mathf.Clamp01(1f - centerDist * 1.2f);
                baseVariation *= 0.7f + vignette * 0.3f;

                pixels[y * resolution + x] = baseVariation;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D CreateSkinTexture(int resolution, Color skinTone)
    {
        var tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, true);
        var pixels = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float nx = (float)x / resolution;
                float ny = (float)y / resolution;

                float noise = 0;
                noise += Mathf.PerlinNoise(nx * 16f, ny * 16f) * 0.5f;
                noise += Mathf.PerlinNoise(nx * 32f, ny * 32f) * 0.3f;
                noise += Mathf.PerlinNoise(nx * 64f, ny * 64f) * 0.2f;

                Color variation = Color.Lerp(skinTone * 0.85f, skinTone * 1.15f, noise);

                float detail = Mathf.PerlinNoise(nx * 128f, ny * 128f);
                variation *= 0.98f + detail * 0.04f;

                pixels[y * resolution + x] = variation;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D CreateBarkTexture(int resolution, Color barkColor)
    {
        var tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, true);
        var pixels = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float nx = (float)x / resolution;
                float ny = (float)y / resolution;

                float verticalNoise = Mathf.PerlinNoise(nx * 24f, ny * 2f);
                float crackPattern = Mathf.Sin(nx * 40f + verticalNoise * 3f);
                crackPattern = crackPattern * 0.5f + 0.5f;

                float noise = Mathf.PerlinNoise(nx * 8f, ny * 8f) * 0.6f +
                              Mathf.PerlinNoise(nx * 16f, ny * 16f) * 0.4f;

                Color barkVar = Color.Lerp(barkColor * 0.6f, barkColor * 1.1f, noise);
                barkVar = Color.Lerp(barkVar, barkVar * 0.7f, crackPattern * 0.3f);

                pixels[y * resolution + x] = barkVar;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static void SaveTexture(Texture2D tex, string path)
    {
        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
    }
}
