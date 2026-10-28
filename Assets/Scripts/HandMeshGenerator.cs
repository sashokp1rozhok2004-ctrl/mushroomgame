using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates low-poly hand meshes with fingers
/// </summary>
public static class HandMeshGenerator
{
    /// <summary>
    /// Creates a stylized hand with 5 fingers (left hand by default, mirror for right)
    /// </summary>
    public static GameObject CreateHand(bool isLeftHand, Material skinMaterial)
    {
        var root = new GameObject(isLeftHand ? "Left Hand" : "Right Hand");

        // Palm
        var palm = CreatePalm(isLeftHand);
        var palmObj = new GameObject("Palm");
        palmObj.transform.SetParent(root.transform);
        palmObj.AddComponent<MeshFilter>().mesh = palm;
        palmObj.AddComponent<MeshRenderer>().sharedMaterial = skinMaterial;

        // Fingers
        CreateFinger(root.transform, "Thumb", new Vector3(isLeftHand ? -0.08f : 0.08f, 0.02f, 0.08f),
            Quaternion.Euler(0, 0, isLeftHand ? -25f : 25f), 0.024f, 0.1f, skinMaterial, isLeftHand);

        CreateFinger(root.transform, "Index", new Vector3(isLeftHand ? -0.05f : 0.05f, 0.01f, 0.14f),
            Quaternion.identity, 0.018f, 0.14f, skinMaterial, isLeftHand);

        CreateFinger(root.transform, "Middle", new Vector3(isLeftHand ? -0.015f : 0.015f, 0.01f, 0.15f),
            Quaternion.identity, 0.019f, 0.15f, skinMaterial, isLeftHand);

        CreateFinger(root.transform, "Ring", new Vector3(isLeftHand ? 0.02f : -0.02f, 0.005f, 0.14f),
            Quaternion.identity, 0.017f, 0.13f, skinMaterial, isLeftHand);

        CreateFinger(root.transform, "Pinky", new Vector3(isLeftHand ? 0.055f : -0.055f, 0f, 0.12f),
            Quaternion.identity, 0.015f, 0.11f, skinMaterial, isLeftHand);

        return root;
    }

    static Mesh CreatePalm(bool isLeftHand)
    {
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        var tris = new List<int>();

        // Simple box-like palm shape with slight curvature
        float width = 0.09f;
        float length = 0.12f;
        float thickness = 0.03f;

        // Bottom vertices
        verts.Add(new Vector3(-width, -thickness, 0));
        verts.Add(new Vector3(width, -thickness, 0));
        verts.Add(new Vector3(width, -thickness, length));
        verts.Add(new Vector3(-width, -thickness, length));

        // Top vertices (slightly curved)
        verts.Add(new Vector3(-width, thickness * 0.7f, 0));
        verts.Add(new Vector3(width, thickness * 0.7f, 0));
        verts.Add(new Vector3(width, thickness, length));
        verts.Add(new Vector3(-width, thickness, length));

        AddQuad(tris, 0, 3, 2, 1); // Bottom
        AddQuad(tris, 4, 5, 6, 7); // Top
        AddQuad(tris, 3, 7, 6, 2); // Front
        AddQuad(tris, 1, 5, 4, 0); // Back
        AddQuad(tris, 0, 4, 7, 3); // Left
        AddQuad(tris, 2, 6, 5, 1); // Right

        if (!isLeftHand)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                verts[i] = new Vector3(-v.x, v.y, v.z);
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    static void CreateFinger(Transform parent, string name, Vector3 basePos, Quaternion baseRot,
        float radius, float totalLength, Material mat, bool isLeftHand)
    {
        var fingerRoot = new GameObject(name);
        fingerRoot.transform.SetParent(parent);
        fingerRoot.transform.localPosition = basePos;
        fingerRoot.transform.localRotation = baseRot;

        int segments = name == "Thumb" ? 2 : 3;
        float segmentLength = totalLength / segments;

        Transform currentParent = fingerRoot.transform;
        for (int i = 0; i < segments; i++)
        {
            var segment = new GameObject($"{name}_Segment{i + 1}");
            segment.transform.SetParent(currentParent);
            segment.transform.localPosition = i == 0 ? Vector3.zero : new Vector3(0, 0, segmentLength);
            segment.transform.localRotation = Quaternion.identity;

            float segmentRadius = radius * (1f - i * 0.15f);
            var mesh = CreateFingerSegment(segmentRadius, segmentLength, 8);

            segment.AddComponent<MeshFilter>().mesh = mesh;
            segment.AddComponent<MeshRenderer>().sharedMaterial = mat;

            currentParent = segment.transform;
        }
    }

    static Mesh CreateFingerSegment(float radius, float length, int sides)
    {
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        var tris = new List<int>();

        // Bottom circle
        for (int i = 0; i < sides; i++)
        {
            float angle = 2f * Mathf.PI * i / sides;
            verts.Add(new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
        }

        // Top circle
        for (int i = 0; i < sides; i++)
        {
            float angle = 2f * Mathf.PI * i / sides;
            float topRadius = radius * 0.92f;
            verts.Add(new Vector3(Mathf.Cos(angle) * topRadius, Mathf.Sin(angle) * topRadius, length));
        }

        // Side faces
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            AddQuad(tris, i, i + sides, next + sides, next);
        }

        // Caps
        AddCap(verts, tris, Vector3.zero, radius, sides, true);
        AddCap(verts, tris, new Vector3(0, 0, length), radius * 0.92f, sides, false);

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    static void AddCap(List<Vector3> verts, List<int> tris, Vector3 center, float radius, int sides, bool flipNormal)
    {
        int centerIndex = verts.Count;
        verts.Add(center);

        int startIndex = verts.Count;
        for (int i = 0; i < sides; i++)
        {
            float angle = 2f * Mathf.PI * i / sides;
            verts.Add(center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
        }

        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            if (flipNormal)
            {
                tris.Add(centerIndex);
                tris.Add(startIndex + next);
                tris.Add(startIndex + i);
            }
            else
            {
                tris.Add(centerIndex);
                tris.Add(startIndex + i);
                tris.Add(startIndex + next);
            }
        }
    }

    static void AddQuad(List<int> tris, int a, int b, int c, int d)
    {
        tris.Add(a);
        tris.Add(b);
        tris.Add(c);

        tris.Add(a);
        tris.Add(c);
        tris.Add(d);
    }

    public static Mesh CreateForearm(float radius, float length, int segments)
    {
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        for (int h = 0; h <= 10; h++)
        {
            float t = h / 10f;
            float y = t * length;
            float currentRadius = radius * (1f + t * 0.1f);

            for (int i = 0; i <= segments; i++)
            {
                float angle = 2f * Mathf.PI * i / segments;
                verts.Add(new Vector3(Mathf.Cos(angle) * currentRadius, Mathf.Sin(angle) * currentRadius, y));
                uvs.Add(new Vector2((float)i / segments, t));
            }
        }

        for (int h = 0; h < 10; h++)
        {
            for (int i = 0; i < segments; i++)
            {
                int current = h * (segments + 1) + i;
                int next = current + segments + 1;

                tris.Add(current);
                tris.Add(next);
                tris.Add(current + 1);

                tris.Add(current + 1);
                tris.Add(next);
                tris.Add(next + 1);
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
