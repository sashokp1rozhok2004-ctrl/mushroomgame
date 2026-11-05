using UnityEngine;

/// <summary>
/// Simple particle effect for mushroom collection
/// </summary>
public class MushroomCollectEffect : MonoBehaviour
{
    public static void PlayAt(Vector3 position)
    {
        var go = new GameObject("Collect Effect");
        go.transform.position = position;

        var ps = go.AddComponent<ParticleSystem>();
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        var shader = Shader.Find("Particles/Standard Unlit");
        if (shader != null)
        {
            renderer.material = new Material(shader) { color = new Color(0.9f, 0.7f, 0.35f, 1f) };
        }
        var main = ps.main;
        main.duration = 0.8f;
        main.startLifetime = 0.6f;
        main.startSpeed = 2f;
        main.startSize = 0.15f;
        main.startColor = new Color(0.8f, 0.6f, 0.3f, 1f);
        main.gravityModifier = 0.3f;
        main.maxParticles = 30;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 25)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.9f, 0.7f, 0.4f), 0f),
                new GradientColorKey(new Color(0.7f, 0.5f, 0.2f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        var curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0.2f));
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);

        ps.Play();
        Destroy(go, main.duration + main.startLifetime.constantMax);
    }
}
