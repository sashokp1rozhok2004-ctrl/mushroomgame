using UnityEngine;

/// <summary>
/// Procedural mesh generation utilities for organic shapes
/// </summary>
public static class ProceduralMeshGenerator
{
    /// <summary>
    /// Creates a stylized mushroom with organic cap and stem
    /// </summary>
    public static Mesh CreateMushroomCap(float radius, int segments, float height, float bulgeFactor)
    {
        var mesh = new Mesh();
        var verts = new Vector3[(segments + 1) * (segments / 2 + 1)];
        var uvs = new Vector2[verts.Length];
        var tris = new int[segments * (segments / 2) * 6];

        int vertIndex = 0;
        for (int lat = 0; lat <= segments / 2; lat++)
        {
            float latAngle = Mathf.PI * lat / (segments / 2);
            float y = Mathf.Cos(latAngle) * height;
            float currentRadius = Mathf.Sin(latAngle) * radius;

            // Add bulge to make it more organic
            if (lat > segments / 4 && lat < segments / 2)
            {
                currentRadius *= (1f + bulgeFactor * Mathf.Sin((lat - segments / 4f) / (segments / 4f) * Mathf.PI));
            }

            for (int lon = 0; lon <= segments; lon++)
            {
                float lonAngle = 2f * Mathf.PI * lon / segments;
                float x = Mathf.Cos(lonAngle) * currentRadius;
                float z = Mathf.Sin(lonAngle) * currentRadius;

                verts[vertIndex] = new Vector3(x, y, z);
                uvs[vertIndex] = new Vector2((float)lon / segments, (float)lat / (segments / 2));
                vertIndex++;
            }
        }

        int triIndex = 0;
        for (int lat = 0; lat < segments / 2; lat++)
        {
            for (int lon = 0; lon < segments; lon++)
            {
                int current = lat * (segments + 1) + lon;
                int next = current + segments + 1;

                tris[triIndex++] = current;
                tris[triIndex++] = next;
                tris[triIndex++] = current + 1;

                tris[triIndex++] = current + 1;
                tris[triIndex++] = next;
                tris[triIndex++] = next + 1;
            }
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Creates mushroom stem with slight irregularity
    /// </summary>
    public static Mesh CreateMushroomStem(float radius, float height, int segments, float irregularity)
    {
        var mesh = new Mesh();
        var verts = new Vector3[(segments + 1) * 2];
        var uvs = new Vector2[verts.Length];
        var tris = new int[segments * 6];

        System.Random rng = new System.Random(42);

        for (int i = 0; i <= segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            float noise = 1f + (float)(rng.NextDouble() - 0.5) * irregularity;
            float r = radius * noise;

            float x = Mathf.Cos(angle) * r;
            float z = Mathf.Sin(angle) * r;

            verts[i] = new Vector3(x, 0, z);
            verts[i + segments + 1] = new Vector3(x * 0.92f, height, z * 0.92f);

            uvs[i] = new Vector2((float)i / segments, 0);
            uvs[i + segments + 1] = new Vector2((float)i / segments, 1);
        }

        int triIndex = 0;
        for (int i = 0; i < segments; i++)
        {
            tris[triIndex++] = i;
            tris[triIndex++] = i + segments + 1;
            tris[triIndex++] = i + 1;

            tris[triIndex++] = i + 1;
            tris[triIndex++] = i + segments + 1;
            tris[triIndex++] = i + segments + 2;
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Creates a low-poly rock with random deformations
    /// </summary>
    public static Mesh CreateRock(float size, int seed)
    {
        System.Random rng = new System.Random(seed);
        int segments = 8 + rng.Next(4);
        var mesh = new Mesh();
        var verts = new Vector3[segments + 2];
        var tris = new int[segments * 6];

        verts[0] = Vector3.up * size * 0.7f;
        verts[1] = Vector3.down * size * 0.4f;

        for (int i = 0; i < segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            float radiusNoise = 0.7f + (float)rng.NextDouble() * 0.5f;
            float heightNoise = -0.1f + (float)rng.NextDouble() * 0.3f;

            float x = Mathf.Cos(angle) * size * radiusNoise;
            float z = Mathf.Sin(angle) * size * radiusNoise;
            float y = heightNoise * size;

            verts[i + 2] = new Vector3(x, y, z);
        }

        int triIndex = 0;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            tris[triIndex++] = 0;
            tris[triIndex++] = i + 2;
            tris[triIndex++] = next + 2;

            tris[triIndex++] = 1;
            tris[triIndex++] = next + 2;
            tris[triIndex++] = i + 2;
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Creates a low-poly tree trunk with organic variations
    /// </summary>
    public static Mesh CreateTreeTrunk(float radius, float height, int segments, int heightSegments, float irregularity)
    {
        System.Random rng = new System.Random(123);
        var mesh = new Mesh();
        int vertCount = (segments + 1) * (heightSegments + 1);
        var verts = new Vector3[vertCount];
        var uvs = new Vector2[vertCount];
        var tris = new int[segments * heightSegments * 6];

        for (int h = 0; h <= heightSegments; h++)
        {
            float heightRatio = (float)h / heightSegments;
            float currentRadius = radius * (1.0f - heightRatio * 0.15f); // Taper slightly
            float y = height * heightRatio;

            for (int i = 0; i <= segments; i++)
            {
                float angle = 2f * Mathf.PI * i / segments;
                float noise = 1f + (float)(rng.NextDouble() - 0.5) * irregularity;
                float r = currentRadius * noise;

                float x = Mathf.Cos(angle) * r;
                float z = Mathf.Sin(angle) * r;

                int vertIndex = h * (segments + 1) + i;
                verts[vertIndex] = new Vector3(x, y, z);
                uvs[vertIndex] = new Vector2((float)i / segments, heightRatio);
            }
        }

        int triIndex = 0;
        for (int h = 0; h < heightSegments; h++)
        {
            for (int i = 0; i < segments; i++)
            {
                int current = h * (segments + 1) + i;
                int next = current + segments + 1;

                tris[triIndex++] = current;
                tris[triIndex++] = next;
                tris[triIndex++] = current + 1;

                tris[triIndex++] = current + 1;
                tris[triIndex++] = next;
                tris[triIndex++] = next + 1;
            }
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
